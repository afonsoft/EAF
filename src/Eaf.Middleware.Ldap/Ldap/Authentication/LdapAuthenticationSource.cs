using Abp;
using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Logging;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Eaf.Middleware.Ldap.Configuration;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace Eaf.Middleware.Ldap.Authentication
{
    /// <summary>
    /// Implements <see cref="IExternalAuthenticationSource{TTenant,TUser}"/> to authenticate users
    /// from LDAP. Extend this class using application's User and Tenant classes as type parameters.
    /// Also, all needed methods can be overridden and changed upon your needs.
    /// </summary>
    /// <typeparam name="TTenant">Tenant type</typeparam>
    /// <typeparam name="TUser">User type</typeparam>
    /// <summary>
    /// Fonte de autenticação LdapAuthenticationSource.
    /// </summary>
    public abstract class LdapAuthenticationSource<TTenant, TUser> : DefaultExternalAuthenticationSource<TTenant, TUser>, ITransientDependency
        where TTenant : AbpTenant<TUser>
        where TUser : AbpUserBase, new()
    {
        private const string DomainComponentSeparator = ", DC=";

        /// <summary>
        /// LDAP
        /// </summary>
        public const string SourceName = "LDAP";

        private readonly IEafMiddlewareLdapModuleConfig _ldapModuleConfig;
        private readonly ILdapSettings _settings;

        protected LdapAuthenticationSource(ILdapSettings settings, IEafMiddlewareLdapModuleConfig ldapModuleConfig)
        {
            _settings = settings;
            _ldapModuleConfig = ldapModuleConfig;
        }

        public override string Name => SourceName;

        [UnitOfWork]
        public override async Task<TUser> CreateUserAsync(string userNameOrEmailAddress, TTenant tenant)
        {
            await CheckIsEnabled(tenant);

            if (userNameOrEmailAddress.IndexOf("@") != -1)
                userNameOrEmailAddress = userNameOrEmailAddress.Split("@")[0];

            var user = await base.CreateUserAsync(userNameOrEmailAddress, tenant);

            if (OperatingSystem.IsWindows() && !_ldapModuleConfig.UseNovellProvider)
            {
                #region Windows

                using (var principalContext = await CreatePrincipalContext(tenant))
                {
                    var userPrincipal = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, userNameOrEmailAddress);

                    if (userPrincipal != null)
                    {
                        UpdateUserFromPrincipal(user, userPrincipal);
                        user.IsEmailConfirmed = true;
                        user.IsActive = true;
                    }
                }

                #endregion Windows
            }
            else
            {
                #region NoWindows

                var principalContext = await CreateLdapContext(tenant);
                string container = SimpleStringCipher.Instance.Decrypt(await _settings.GetDomain(tenant?.Id));
                if (container.Contains(".") && !container.Contains("DC="))
                    container = "DC=" + string.Join(DomainComponentSeparator, container.Split("."));

                string[] attrib = { "samAccountName", "displayName", "userPrincipalName", "mail" };
                var filter1 = $"(&(objectClass=user)(SAMAccountName={userNameOrEmailAddress}))";
                var searcher1 = await principalContext.SearchAsync(container, LdapConnection.ScopeSub, filter1, attrib, false);
                var ldapEntry = FillUsersLdap(searcher1).Result.Item1.FirstOrDefault();

                if (ldapEntry != null)
                {
                    UpdateUserFromLdap(user, ldapEntry);
                    user.IsEmailConfirmed = true;
                    user.IsActive = true;
                }
                principalContext.Disconnect();

                #endregion NoWindows
            }
            return user;
        }

        /// <summary>
        /// GetUsersAsync.
        /// </summary>
        /// <param name="userNameOrEmailAddress">Parâmetro userNameOrEmailAddress.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<List<TUser>> GetUsersAsync(string userNameOrEmailAddress)
        {
            if (string.IsNullOrEmpty(userNameOrEmailAddress))
                return new List<TUser>();

            if (OperatingSystem.IsWindows() && !_ldapModuleConfig.UseNovellProvider)
                return await GetUsersFromActiveDirectoryAsync(userNameOrEmailAddress);

            return await GetUsersFromLdapAsync(userNameOrEmailAddress);
        }

        private async Task<List<TUser>> GetUsersFromActiveDirectoryAsync(string userNameOrEmailAddress)
        {
            using (var principalContext = await CreatePrincipalContext(null))
            {
                var searchString = string.Format("*{0}*", userNameOrEmailAddress);

                using (var searchMaskDisplayname = new UserPrincipal(principalContext) { DisplayName = searchString, Enabled = true })
                using (var searchMaskUsername = new UserPrincipal(principalContext) { SamAccountName = searchString, Enabled = true })
                using (var searchMaskEmail = new UserPrincipal(principalContext) { EmailAddress = searchString, Enabled = true })
                using (var searcherDisplayname = new PrincipalSearcher(searchMaskDisplayname))
                using (var searcherUsername = new PrincipalSearcher(searchMaskUsername))
                using (var searcherEmail = new PrincipalSearcher(searchMaskEmail))
#pragma warning disable CA1416 // Already guarded by OperatingSystem.IsWindows()
                using (var taskDisplayname = Task.Run(() => SearchWithLimit(searcherDisplayname, 10)))
                using (var taskUsername = Task.Run(() => SearchWithLimit(searcherUsername, 10)))
                using (var taskEmail = Task.Run(() => SearchWithLimit(searcherEmail, 10)))
#pragma warning restore CA1416
                {
                    var users = new List<TUser>();
                    foreach (Principal result in (await taskDisplayname).Union(await taskUsername).Union(await taskEmail))
                    {
                        users.Add(new TUser
                        {
                            UserName = result.SamAccountName,
                            Name = result.DisplayName,
                            EmailAddress = (result is UserPrincipal principal) ? principal.EmailAddress : ""
                        });
                    }

                    return users.DistinctBy(o => o.UserName).Take(10).ToList();
                }
            }
        }

        private async Task<List<TUser>> GetUsersFromLdapAsync(string userNameOrEmailAddress)
        {
            string container = NormalizeLdapContainer(SimpleStringCipher.Instance.Decrypt(await _settings.GetDomain(null)));
            string userName = NormalizeLdapUserName(userNameOrEmailAddress);
            string[] attributes = { "samAccountName", "displayName", "userPrincipalName", "mail" };

            var results = await Task.WhenAll(
                ExecuteLdapSearchAsync(container, $"(&(objectClass=user)(samAccountName={userName}))", attributes),
                ExecuteLdapSearchAsync(container, $"(&(objectClass=user)(mail={userNameOrEmailAddress}))", attributes),
                ExecuteLdapSearchAsync(container, $"(&(objectClass=user)(displayName={userName}*))", attributes),
                ExecuteLdapSearchAsync(container, $"(&(objectClass=user)(userPrincipalName={userName}*))", attributes)
            );

            var users = results.SelectMany(r => r.Item1).ToList();
            var exceptions = results.SelectMany(r => r.Item2).ToList();

            ThrowIfNoUsersAndHasExceptions(users, exceptions);

            return users.DistinctBy(o => o.Logins).Take(10).ToList();
        }

        private async Task<Tuple<List<TUser>, List<Exception>>> ExecuteLdapSearchAsync(string container, string filter, string[] attributes)
        {
            using (var principalContext = await CreateLdapContext(null) as IDisposable)
            {
                var ldapContext = (ILdapConnection)principalContext;
                var searcher = await ldapContext.SearchAsync(container, LdapConnection.ScopeSub, filter, attributes, false);
                return await FillUsersLdap(searcher);
            }
        }

        private static string NormalizeLdapContainer(string container)
        {
            if (container.Contains(".") && !container.Contains("DC="))
                return "DC=" + string.Join(DomainComponentSeparator, container.Split("."));
            return container;
        }

        private static string NormalizeLdapUserName(string userNameOrEmailAddress)
        {
            if (userNameOrEmailAddress.IndexOf("@") != -1)
                return userNameOrEmailAddress.Split("@")[0];
            return userNameOrEmailAddress;
        }

        private static void ThrowIfNoUsersAndHasExceptions(List<TUser> users, List<Exception> exceptions)
        {
            if (exceptions != null && exceptions.Any() && !users.Any())
                throw new AggregateException(exceptions);
        }

        private async Task<Tuple<List<TUser>, List<Exception>>> FillUsersLdap(ILdapSearchResults serach)
        {
            List<TUser> users = new();
            List<Exception> exceptions = new();
            int limit = 0;
            while (await serach.HasMoreAsync() && limit < 100)
            {
                limit++;
                try
                {
                    LdapEntry nextEntry = null;
                    try
                    {
                        nextEntry = await serach.NextAsync();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Logger.WarnFormat(ex, "Error serach.Next() in FillUsersLdap {0}", ex.Message);
                        continue;
                    }

                    var usr = new TUser
                    {
                        UserName = GetAttribute(nextEntry, "SamAccountName"),
                        Name = GetAttribute(nextEntry, "DisplayName"),
                        EmailAddress = GetAttribute(nextEntry, "mail")
                    };

                    if (string.IsNullOrEmpty(usr.EmailAddress))
                        usr.EmailAddress = GetAttribute(nextEntry, "UserPrincipalName");

                    usr.Name = usr.Name.Split(" ")[0];
                    usr.Surname = usr.Name.Replace(usr.Name, "").Trim();

                    users.Add(usr);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    LogHelper.Logger.WarnFormat(ex, "Error on fill TUser in FillUsersLdap {0}", ex.Message);
                }
            }
            return new Tuple<List<TUser>, List<Exception>>(users, exceptions);
        }

        private static string GetAttribute(LdapEntry entry, string attrName)
        {
            if (entry.GetAttributeSet().ContainsKey(attrName))
            {
                var attr = entry.GetStringValueOrDefault(attrName);
                if (!string.IsNullOrEmpty(attr))
                    return attr;
            }
            return "";
        }

        /// <inheritdoc/>
        public override async Task<bool> TryAuthenticateAsync(string userNameOrEmailAddress, string plainPassword, TTenant tenant)
        {
            if (!_ldapModuleConfig.IsEnabled || !(await _settings.GetIsEnabled(tenant?.Id)))
            {
                return false;
            }

            if (userNameOrEmailAddress.IndexOf("@") != -1)
                userNameOrEmailAddress = userNameOrEmailAddress.Split("@")[0];

            if (OperatingSystem.IsWindows() && !_ldapModuleConfig.UseNovellProvider)
            {
                #region Windows

                using (var principalContext = await CreatePrincipalContext(tenant))
                {
                    UserPrincipal user = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, userNameOrEmailAddress);
                    if (user == null) return false;

                    bool initialValidation;

                    // maybe validation failed because "user must change password at next logon". let's
                    // see if that is the case.
                    if (user.LastPasswordSet == null)
                    {
                        // the user must change his password at next logon. So this might be why
                        // validation returned false uncheck the "change password" checkbox and attempt
                        // validation again

                        var deUser = user.GetUnderlyingObject() as DirectoryEntry;
                        var property = deUser.Properties["pwdLastSet"];
                        property.Value = -1;
                        deUser.CommitChanges();

                        // property was unset, retry validation
                        initialValidation = ValidateCredentials(principalContext, user.SamAccountName, plainPassword);

                        // re check the checkbox
                        property.Value = 0;
                        deUser.CommitChanges();
                    }
                    else
                    {
                        initialValidation = ValidateCredentials(principalContext, user.SamAccountName, plainPassword);
                    }

                    return initialValidation;
                }

                #endregion Windows
            }
            else
            {
                #region NoWindows

                var ldap = await CreateLdapContext(tenant, userNameOrEmailAddress, plainPassword);
                return ldap.Connected;

                #endregion NoWindows
            }
        }

        [UnitOfWork]
        public override async Task UpdateUserAsync(TUser user, TTenant tenant)
        {
            await CheckIsEnabled(tenant);

            await base.UpdateUserAsync(user, tenant);
            try
            {
                if (OperatingSystem.IsWindows() && !_ldapModuleConfig.UseNovellProvider)
                {
                    #region Windows

                    using (var principalContext = await CreatePrincipalContext(tenant))
                    {
                        var userPrincipal = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, user.UserName);

                        if (userPrincipal != null)
                            UpdateUserFromPrincipal(user, userPrincipal);
                    }

                    #endregion Windows
                }
                else
                {
                    #region NoWindows

                    var principalContext = await CreateLdapContext(tenant);
                    string container = SimpleStringCipher.Instance.Decrypt(await _settings.GetDomain(tenant?.Id));
                    if (container.Contains(".") && !container.Contains("DC="))
                        container = "DC=" + string.Join(DomainComponentSeparator, container.Split("."));

                    string[] attrib = { "samAccountName", "displayName", "userPrincipalName", "mail" };
                    var filter1 = $"(&(objectClass=user)(SAMAccountName={user.UserName}))";
                    var searcher1 = await principalContext.SearchAsync(container, LdapConnection.ScopeSub, filter1, attrib, false);
                    var ldapEntry = FillUsersLdap(searcher1).Result.Item1.FirstOrDefault();

                    if (ldapEntry != null)
                    {
                        UpdateUserFromLdap(user, ldapEntry);
                    }
                    principalContext.Disconnect();

                    #endregion NoWindows
                }
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error("UpdateUserAsync : UserName : " + user.UserName, ex);
            }
        }

        protected static string ConvertToNullIfEmpty(string str)
        {
            return str.IsNullOrWhiteSpace()
                ? null
                : str;
        }

        protected virtual async Task CheckIsEnabled(TTenant tenant)
        {
            if (!_ldapModuleConfig.IsEnabled)
            {
                throw new AbpException("Ldap Authentication module is disabled globally!");
            }

            var tenantId = tenant?.Id;
            if (!await _settings.GetIsEnabled(tenantId))
            {
                throw new AbpException("Ldap Authentication is disabled for given tenant (id:" + tenantId + ")! You can enable it by setting '" + LdapSettingNames.IsEnabled + "' to true");
            }
        }

        #region NoWindows

        protected virtual async Task<ILdapConnection> CreateLdapContext(TTenant tenant)
        {
            return await CreateLdapContext(tenant, null, null);
        }

        protected virtual async Task<ILdapConnection> CreateLdapContext(TTenant tenant, string userNameOrEmailAddress, string plainPassword)
        {
            string container = SimpleStringCipher.Instance.Decrypt(await _settings.GetContainer(tenant?.Id));
            if (container.IsNullOrEmpty() || !container.Contains("DC="))
                container = SimpleStringCipher.Instance.Decrypt(await _settings.GetDomain(tenant?.Id));
            if (!container.IsNullOrEmpty() && container.Contains(".") && !container.Contains("DC="))
                container = "DC=" + string.Join(DomainComponentSeparator, container.Split("."));

            string domain = await _settings.GetDomain(tenant?.Id);

            string userName = userNameOrEmailAddress ?? ConvertToNullIfEmpty(await _settings.GetUserName(tenant?.Id));
            string pwd = plainPassword ?? ConvertToNullIfEmpty(await _settings.GetPassword(tenant?.Id));

            if (userName != null && !userName.Contains(domain) && !userName.Contains("\\") && !domain.Contains("DC=") && !domain.Contains("."))
                userName = domain + "\\" + userName;

            var ldapConn = new LdapConnection();
            ldapConn.UserDefinedServerCertValidationDelegate += (sender, certificate, chain, sslPolicyErrors) => true;

            try
            {
                await ldapConn.ConnectAsync(domain, LdapConnection.DefaultPort);
            }
            catch (Exception ex)
            {
                try
                {
                    await ldapConn.ConnectAsync(domain, LdapConnection.DefaultSslPort);
                }
                catch (Exception ex2)
                {
                    throw new AbpException(ex.Message, ex2);
                }
            }

            if (ldapConn.Connected)
            {
                try
                {
                    await ldapConn.BindAsync(userName, pwd);
                }
                catch (Exception ex)
                {
                    try
                    {
                        await ldapConn.BindAsync("uid=" + userName + ", " + container, pwd);
                    }
                    catch (Exception ex2)
                    {
                        throw new AbpException(ex.Message, ex2);
                    }
                }
            }

            LdapSearchConstraints cons = ldapConn.SearchConstraints ?? new LdapSearchConstraints();
            cons.MaxResults = 100;
            cons.ServerTimeLimit = 30;
            cons.ReferralFollowing = false;
            ldapConn.Constraints = cons;
            ldapConn.ConnectionTimeout = 30000;

            return (ILdapConnection)ldapConn;
        }

        protected virtual void UpdateUserFromLdap(TUser user, TUser userPrincipal)
        {
            if (!string.IsNullOrEmpty(userPrincipal?.UserName))
            {
                user.UserName = userPrincipal.UserName.ToLower();
            }

            user.Name = userPrincipal?.Name ?? user.Name;
            user.Surname = userPrincipal?.Surname ?? user.Surname;

            var mail = userPrincipal?.EmailAddress;

            user.EmailAddress = string.IsNullOrEmpty(mail)
                ? user.EmailAddress.ToLower()
                : mail.ToLower();

            user.IsActive = true;
            LogHelper.Logger.DebugFormat("UpdateUserFromPrincipal: {0} / {1}", user.UserName, userPrincipal?.UserName);
        }

        #endregion NoWindows

        #region Windows

        protected virtual async Task<PrincipalContext> CreatePrincipalContext(TTenant tenant)
        {
            return await CreatePrincipalContext(tenant, null, null);
        }

        protected virtual async Task<PrincipalContext> CreatePrincipalContext(TTenant tenant, string userNameOrEmailAddress, string plainPassword)
        {
            if (!OperatingSystem.IsWindows())
                throw new NotImplementedException("This Method is only supported on: 'windows'");

            string container = SimpleStringCipher.Instance.Decrypt(await _settings.GetContainer(tenant?.Id));
            if (container.IsNullOrEmpty() || !container.Contains("DC="))
                container = SimpleStringCipher.Instance.Decrypt(await _settings.GetDomain(tenant?.Id));
            if (!container.IsNullOrEmpty() && container.Contains(".") && !container.Contains("DC="))
                container = "DC=" + string.Join(DomainComponentSeparator, container.Split("."));

            var contextType = await _settings.GetContextType(tenant?.Id);
            contextType ??= ContextType.Domain;

            ContextOptions options = ContextOptions.Negotiate | ContextOptions.ServerBind;

            PrincipalContext principalContext = new(
                (ContextType)contextType,
                ConvertToNullIfEmpty(await _settings.GetDomain(tenant?.Id)),
                ConvertToNullIfEmpty(await _settings.GetContainer(tenant?.Id) ?? container),
                options,
                userNameOrEmailAddress ?? ConvertToNullIfEmpty(await _settings.GetUserName(tenant?.Id)),
                plainPassword ?? ConvertToNullIfEmpty(await _settings.GetPassword(tenant?.Id)));

            return principalContext;
        }

        protected virtual void UpdateUserFromPrincipal(TUser user, UserPrincipal userPrincipal)
        {
            if (!OperatingSystem.IsWindows())
                return;

            if (!string.IsNullOrEmpty(userPrincipal?.SamAccountName))
            {
                user.UserName = userPrincipal.SamAccountName.ToLower();
            }

            user.Name = userPrincipal?.GivenName ?? user.Name;
            user.Surname = userPrincipal?.Surname ?? user.Surname;
            user.EmailAddress = string.IsNullOrEmpty(userPrincipal?.EmailAddress)
                ? user.EmailAddress.ToLower()
                : userPrincipal.EmailAddress.ToLower();

            LogHelper.Logger.DebugFormat("UpdateUserFromPrincipal: {0} / {1}", user.UserName, userPrincipal?.SamAccountName);
            if (userPrincipal?.Enabled != null)
            {
                user.IsActive = userPrincipal.Enabled.Value;
            }
        }

        protected virtual bool ValidateCredentials(PrincipalContext principalContext, string userNameOrEmailAddress, string plainPassword)
        {
            if (!OperatingSystem.IsWindows())
                return false;

            ContextOptions options = ContextOptions.Negotiate | ContextOptions.ServerBind;
            bool validate = principalContext.ValidateCredentials(userNameOrEmailAddress, plainPassword, options);

            if (!validate)
            {
                //if false check with ldap
                string path = $"LDAP://{principalContext.ConnectedServer}/{principalContext.Container}";
                using (DirectoryEntry adsEntry = new(path, userNameOrEmailAddress, plainPassword))
                {
                    using (DirectorySearcher adsSearcher = new(adsEntry))
                    {
                        adsSearcher.Filter = "(sAMAccountName=" + userNameOrEmailAddress + ")";
                        try
                        {
                            //Valida se loga no ldap
                            SearchResult adsSearchResult = adsSearcher.FindOne();
                            if (adsSearchResult != null)
                                validate = true;
                        }
                        catch (DirectoryServicesCOMException ex)
                        {
                            //Se ter erro vai mostar qual o motivo no ExtendedErrorMessage
                            validate = false;
                            throw new AbpException(ex.Message + Environment.NewLine + ex.ExtendedErrorMessage, ex);
                        }
                    }
                }
            }
            return validate;
        }

        #endregion Windows

        /// <summary>
        /// Executes a <see cref="PrincipalSearcher"/> with a size limit on its underlying <see cref="DirectorySearcher"/>.
        /// Extracted to isolate the Windows-only API call from the caller.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private static PrincipalSearchResult<Principal> SearchWithLimit(PrincipalSearcher searcher, int sizeLimit)
        {
            if (searcher.GetUnderlyingSearcher() is DirectorySearcher ds)
                ds.SizeLimit = sizeLimit;

            return searcher.FindAll();
        }
    }
}