using Abp;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Logging;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.UI;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security;
using System.Threading.Tasks;

namespace Eaf.Middleware.AzureActiveDirectory.Authentication
{
    /// <summary>
    /// Implements <see cref="IExternalAuthenticationSource{TTenant,TUser}"/> to authenticate users
    /// from AzureActiveDirectory. Extend this class using application's User and Tenant classes as
    /// type parameters. Also, all needed methods can be overridden and changed upon your needs.
    /// </summary>
    /// <typeparam name="TTenant">Tenant type</typeparam>
    /// <typeparam name="TUser">User type</typeparam>
    public abstract class AzureActiveDirectoryAuthenticationSource<TTenant, TUser> : DefaultExternalAuthenticationSource<TTenant, TUser>, ITransientDependency
        where TTenant : AbpTenant<TUser>
        where TUser : AbpUserBase, new()
    {
        /// <summary>
        /// AzureActiveDirectory
        /// </summary>
        public const string SourceName = "ActiveDirectory";

        private readonly IEafMiddlewareAzureActiveDirectoryModuleConfig _AzureActiveDirectoryModuleConfig;
        private readonly IAzureActiveDirectorySettings _settings;
        private string _signInToken;

        /// <summary>
        /// Inicializa uma nova instância da classe AzureActiveDirectoryAuthenticationSource.
        /// </summary>
        /// <param name="settings">Configurações do Azure Active Directory</param>
        /// <param name="AzureActiveDirectoryModuleConfig">Configuração do módulo Azure AD</param>
        protected AzureActiveDirectoryAuthenticationSource(
            IAzureActiveDirectorySettings settings,
            IEafMiddlewareAzureActiveDirectoryModuleConfig AzureActiveDirectoryModuleConfig
        )
        {
            _settings = settings;
            _AzureActiveDirectoryModuleConfig = AzureActiveDirectoryModuleConfig;
        }

        /// <summary>
        /// Obtém o nome da fonte de autenticação.
        /// </summary>
        public override string Name => SourceName;

        /// <summary>
        /// Cria um novo usuário baseado nas informações do Azure Active Directory.
        /// </summary>
        /// <param name="userNameOrEmailAddress">Nome de usuário ou endereço de email</param>
        /// <param name="tenant">Tenant ao qual o usuário pertence</param>
        /// <returns>Nova instância do usuário criado</returns>
        public override async Task<TUser> CreateUserAsync(string userNameOrEmailAddress, TTenant tenant)
        {
            TUser result = await base.CreateUserAsync(userNameOrEmailAddress, tenant);
            try
            {
                var userPrincipal = await GetUserAsync(userNameOrEmailAddress);
                UpdateUserFromAzureActiveDirectory(result, userPrincipal);
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error("CreateUserAsync : UserName : " + result.UserName, ex);
            }

            result.IsEmailConfirmed = true;
            result.IsActive = true;

            return result;
        }

        /// <summary>
        /// Obtém informações de um usuário específico do Azure Active Directory.
        /// </summary>
        /// <param name="userNameOrEmailAddress">Nome de usuário ou endereço de email</param>
        /// <returns>Instância do usuário encontrado ou null se não encontrado</returns>
        public async Task<TUser> GetUserAsync(string userNameOrEmailAddress)
        {
            if (string.IsNullOrEmpty(userNameOrEmailAddress))
                return null;

            if (!_AzureActiveDirectoryModuleConfig.IsEnabled || !(await _settings.GetIsEnabled()))
                return null;

            string azureTenant = await _settings.GetTenant();

            if (!userNameOrEmailAddress.Contains(azureTenant))
                userNameOrEmailAddress = $"{userNameOrEmailAddress}@{azureTenant}".ToLower();

            try
            {
                var graphClient = await CreateGraphServiceClient();

                var query = $"userPrincipalName eq '{userNameOrEmailAddress}'";
                var user = new TUser();
                var result = await graphClient.Users.Request().Filter(query).GetAsync();

                if (result.Count > 0)
                {
                    var item = result[0];

                    user.UserName = item.UserPrincipalName.Contains("@") ? item.UserPrincipalName.Split('@')[0] : item.UserPrincipalName;
                    user.Name = item.DisplayName;
                    user.Surname = item.Surname;

                    if (!string.IsNullOrEmpty(item.Mail))
                        user.EmailAddress = item.Mail.Contains("@") ? item.Mail : $"{user.UserName}@{azureTenant}";
                    else
                        user.EmailAddress = $"{user.UserName}@{azureTenant}";
                }

                return user;
            }
            catch (Exception exception)
            {
                throw new AbpException($"AzureActiveDirectory Authentication - Error GetUser : {exception.Message}", exception);
            }
        }

        /// <summary>
        /// GetUsersAsync.
        /// </summary>
        /// <param name="userNameOrEmailAddress">Parâmetro userNameOrEmailAddress.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<List<TUser>> GetUsersAsync(string userNameOrEmailAddress)
        {
            List<TUser> users = new List<TUser>();

            if (string.IsNullOrEmpty(userNameOrEmailAddress))
                return users;

            if (!_AzureActiveDirectoryModuleConfig.IsEnabled || !(await _settings.GetIsEnabled()))
                return users;

            var tenantName = await _settings.GetTenant();

            try
            {
                var graphClient = await CreateGraphServiceClient();

                var query = $"startswith(displayName,'{userNameOrEmailAddress}') or startswith(userPrincipalName,'{userNameOrEmailAddress}')";

                var result = await graphClient.Users.Request()
                    .Top(10)
                    .Filter(query)
                    .GetAsync();

                foreach (var item in result)
                {
                    var login = item.UserPrincipalName.Contains("@") ? item.UserPrincipalName.Split('@')[0] : item.UserPrincipalName;
                    var mail = "";
                    if (!string.IsNullOrEmpty(item.Mail))
                        mail = item.Mail.Contains("@") ? item.Mail : $"{login}@{tenantName}";
                    else
                        mail = $"{login}@{tenantName}";

                    users.Add(new TUser
                    {
                        UserName = login,
                        Name = item.DisplayName,
                        Surname = item.Surname,
                        EmailAddress = mail
                    });
                }

                return users;
            }
            catch (AbpException exception)
            {
                throw new AbpException($"AzureActiveDirectory Authentication - Error GetUsers '{userNameOrEmailAddress}'", exception);
            }
        }

        /// <summary>
        /// Tenta autenticar um usuário usando credenciais do Azure Active Directory.
        /// </summary>
        /// <param name="userNameOrEmailAddress">Nome de usuário ou endereço de email</param>
        /// <param name="plainPassword">Senha em texto plano</param>
        /// <param name="tenant">Tenant para autenticação</param>
        /// <returns>True se a autenticação foi bem-sucedida; caso contrário, false</returns>
        public override async Task<bool> TryAuthenticateAsync(string userNameOrEmailAddress, string plainPassword, TTenant tenant)
        {
            if (!_AzureActiveDirectoryModuleConfig.IsEnabled || !(await _settings.GetIsEnabled()))
            {
                return false;
            }

            string azureTenant = await _settings.GetTenant();

            if (!userNameOrEmailAddress.Contains(azureTenant))
                userNameOrEmailAddress = $"{userNameOrEmailAddress}@{azureTenant}".ToLower();

            string[] scopes = new string[] { "user.read" };
            var app = await CreateAzureApplication();
            try
            {
                var securePassword = new SecureString();
                foreach (char c in plainPassword)
                    securePassword.AppendChar(c);

                var result = await app.AcquireTokenByUsernamePassword(scopes, userNameOrEmailAddress, securePassword).ExecuteAsync();
                if (result?.Account != null)
                {
                    _signInToken = result.AccessToken;
                    return true;
                }
                else
                    return false;
            }
            catch (MsalException ex)
            {
                if (ex.ErrorCode.Contains("expired"))
                    throw new UserFriendlyException(AbpLoginResultType.UserIsNotActive.GetHashCode(), "LoginFailed", "UserExpiredPassword");
                else if (ex.ErrorCode.Contains("blocked"))
                    throw new UserFriendlyException(AbpLoginResultType.LockedOut.GetHashCode(), "LoginFailed", "UserLockedOutMessage");
                else if (ex.ErrorCode.Contains("invalid_grant") || ex.Message.Contains("AADS"))
                    throw new UserFriendlyException(AbpLoginResultType.UnknownExternalLogin.GetHashCode(), "LoginFailed", ex.Message);

                LogHelper.Logger.Error("TryAuthenticateAsync : UserName : " + userNameOrEmailAddress + " : " + ex.Message, ex);

                return false;
            }
        }

        /// <summary>
        /// UpdateUserAsync.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        /// <param name="tenant">Parâmetro tenant.</param>
        public override async Task UpdateUserAsync(TUser user, TTenant tenant)
        {
            await base.UpdateUserAsync(user, tenant);

            try
            {
                var userPrincipal = await GetUserAsync(user.UserName);
                UpdateUserFromAzureActiveDirectory(user, userPrincipal);
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error("UpdateUserAsync : UserName : " + user.UserName, ex);
            }
        }

        protected virtual async Task CheckIsEnabled(TTenant tenant)
        {
            if (!_AzureActiveDirectoryModuleConfig.IsEnabled)
            {
                throw new AbpException("AzureActiveDirectory Authentication module is disabled globally!");
            }

            var tenantId = tenant?.Id;
            if (!await _settings.GetIsEnabled())
            {
                throw new AbpException("AzureActiveDirectory Authentication is disabled for given tenant (id:" + tenantId + ")! You can enable it by setting '" + AzureActiveDirectorySettingNames.IsEnabled + "' to true");
            }
        }

        protected virtual async Task<IPublicClientApplication> CreateAzureApplication()
        {
            string clientId = SimpleStringCipher.Instance.Decrypt(await _settings.GetClientId());
            string azureTenant = await _settings.GetTenant();
            string authority = $"https://login.microsoftonline.com/{azureTenant}";

            return PublicClientApplicationBuilder.Create(clientId).WithAuthority(authority).Build();
        }

        protected virtual async Task<IConfidentialClientApplication> CreateAzureConfidential()
        {
            string clientId = SimpleStringCipher.Instance.Decrypt(await _settings.GetClientId());
            string clientSecret = SimpleStringCipher.Instance.Decrypt(await _settings.GetClientSecret());
            string azureTenant = await _settings.GetTenant();
            string authority = $"https://login.microsoftonline.com/{azureTenant}";

            return ConfidentialClientApplicationBuilder
                    .Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority(new Uri(authority))
                    .Build();
        }

        protected virtual async Task<GraphServiceClient> CreateGraphServiceClient()
        {
            var app = await CreateAzureConfidential();

            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var authenticationResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();

            return new GraphServiceClient(new DelegateAuthenticationProvider(requestMessage =>
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", authenticationResult.AccessToken);
                return Task.FromResult(0);
            }));
        }

        protected virtual void UpdateUserFromAzureActiveDirectory(TUser user, TUser userPrincipal)
        {
            user.UserName = userPrincipal?.UserName ?? user.UserName;
            user.Name = userPrincipal?.Name ?? user.Name;
            user.Surname = userPrincipal?.Surname ?? user.Surname;
            user.EmailAddress = string.IsNullOrEmpty(userPrincipal?.EmailAddress)
                ? user.EmailAddress.ToLower()
                : userPrincipal.EmailAddress.ToLower();
            user.IsActive = true;
            LogHelper.Logger.DebugFormat("UpdateUserFromAzureActiveDirectory: {0}", user.UserName);
        }
    }
}