using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Extensions;
using Abp.Json;
using Abp.Net.Mail;
using Abp.Timing;
using Abp.Zero.Configuration;
using Eaf.Configuration.Host.Dto;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Configuration.Host.Dto;
using Eaf.Middleware.Core.Authentication;
using Eaf.Middleware.Ldap.Configuration;
using Eaf.Middleware.Security;
using Eaf.Middleware.Timing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Configuration.Host
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de HostSettings.
    /// </summary>
    [AbpAllowAnonymous]
    public class HostSettingsAppService : SettingsAppServiceBase, IHostSettingsAppService
    {
        private readonly IEafMiddlewareAzureActiveDirectoryModuleConfig _azureActiveDirectoryModuleConfig;
        private readonly IEafMiddlewareLdapModuleConfig _ldapModuleConfig;
        private readonly ISettingDefinitionManager _settingDefinitionManager;
        private readonly ITimeZoneService _timeZoneService;

        /// <summary>
        /// HostSettingsAppService.
        /// </summary>
        /// <param name="emailSender">Parâmetro emailSender.</param>
        /// <param name="timeZoneService">Parâmetro timeZoneService.</param>
        /// <param name="settingDefinitionManager">Parâmetro settingDefinitionManager.</param>
        /// <param name="azureActiveDirectoryModuleConfig">Parâmetro azureActiveDirectoryModuleConfig.</param>
        /// <param name="ldapModuleConfig">Parâmetro ldapModuleConfig.</param>
        /// <returns>Resultado da operação.</returns>
        public HostSettingsAppService(
            IEmailSender emailSender,
            ITimeZoneService timeZoneService,
            ISettingDefinitionManager settingDefinitionManager,
            IEafMiddlewareAzureActiveDirectoryModuleConfig azureActiveDirectoryModuleConfig,
            IEafMiddlewareLdapModuleConfig ldapModuleConfig
        ) : base(emailSender)
        {
            _timeZoneService = timeZoneService;
            _settingDefinitionManager = settingDefinitionManager;
            _azureActiveDirectoryModuleConfig = azureActiveDirectoryModuleConfig;
            _ldapModuleConfig = ldapModuleConfig;
        }

        #region Get Settings

        [AbpAllowAnonymous]
        public async Task<HostSettingsEditDto> GetAllSettingsAnonymous()
        {
            var settings = new HostSettingsEditDto
            {
                General = await GetGeneralSettingsAsync(),
                UserManagement = await GetUserManagementAsync(),
                Email = await GetEmailSettingsAsync(false),
                Security = await GetSecuritySettingsAsync(),
                Google = await GetGoogleSettingsAsync(),
                ExternalLoginProviderSettings = await GetExternalLoginProviderSettings(false),
            };

            settings.AzureActiveDirectory = await GetAzureActiveDirectorySettingsAsync(false);
            settings.Ldap = await GetLdapSettingsAsync(false);
            settings.LogDeleter = await GetLogDeleterAsync();
            settings.LoginImpersonator = await GetLoginImpersonatorAsync();

            return settings;
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Settings)]
        public async Task<HostSettingsEditDto> GetAllSettings()
        {
            var settings = new HostSettingsEditDto
            {
                General = await GetGeneralSettingsAsync(),
                UserManagement = await GetUserManagementAsync(),
                Email = await GetEmailSettingsAsync(true),
                Security = await GetSecuritySettingsAsync(),
                Google = await GetGoogleSettingsAsync(),
                ExternalLoginProviderSettings = await GetExternalLoginProviderSettings(true),
            };

            settings.AzureActiveDirectory = await GetAzureActiveDirectorySettingsAsync(true);
            settings.Ldap = await GetLdapSettingsAsync(true);
            settings.LogDeleter = await GetLogDeleterAsync();
            settings.LoginImpersonator = await GetLoginImpersonatorAsync();

            return settings;
        }

        private async Task<AzureActiveDirectorySettingsEditDto> GetAzureActiveDirectorySettingsAsync(bool isAdmin)
        {
            return new AzureActiveDirectorySettingsEditDto
            {
                IsModuleEnabled = true,
                IsEnabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(AzureActiveDirectorySettingNames.IsEnabled),
                Tenant = isAdmin ? await SettingManager.GetSettingValueForApplicationAsync(AzureActiveDirectorySettingNames.Tenant) : "",
                ClientId = isAdmin ? await SettingManager.GetSettingValueForApplicationAsync(AzureActiveDirectorySettingNames.ClientId) : "",
                ClientSecret = isAdmin ? await SettingManager.GetSettingValueForApplicationAsync(AzureActiveDirectorySettingNames.ClientSecret) : "",
            };
        }

        private async Task<EmailSettingsEditDto> GetEmailSettingsAsync(bool isAdmin)
        {
            var smtpPassword = await SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Password);

            return new EmailSettingsEditDto
            {
                DefaultFromAddress = isAdmin ? await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromAddress) : "",
                DefaultFromDisplayName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromDisplayName),
                SmtpHost = isAdmin ? await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Host) : "",
                SmtpPort = isAdmin ? await SettingManager.GetSettingValueForApplicationAsync<int>(EmailSettingNames.Smtp.Port) : 0,
                SmtpUserName = isAdmin ? await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.UserName) : "",
                SmtpPassword = isAdmin ? smtpPassword : "",
                SmtpDomain = isAdmin ? await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Domain) : "",
                SmtpEnableSsl = await SettingManager.GetSettingValueForApplicationAsync<bool>(EmailSettingNames.Smtp.EnableSsl),
                SmtpUseDefaultCredentials = await SettingManager.GetSettingValueForApplicationAsync<bool>(EmailSettingNames.Smtp.UseDefaultCredentials)
            };
        }

        private async Task<ExternalLoginProviderSettingsEditDto> GetExternalLoginProviderSettings(bool isAdmin)
        {
            try
            {
                var googleSettings = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.ExternalLoginProvider.Host.Google);
                var microsoftSettings = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.ExternalLoginProvider.Host.Microsoft);
                var authZeroSettings = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.ExternalLoginProvider.Host.AuthZero);
                var openIdConnectSettings = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.ExternalLoginProvider.Host.OpenIdConnect);
                var openIdConnectMapperClaims = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.ExternalLoginProvider.OpenIdConnectMappedClaims);

                bool OpenIdConnectEnabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsEnabled);
                bool MicrosoftEnabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled);
                bool GoogleEnabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.ExternalLoginProvider.Tenant.Google_IsEnabled);

                bool AuthZeroEnabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.ExternalLoginProvider.Tenant.AuthZero_IsEnabled);

                if (isAdmin)
                {
                    return new ExternalLoginProviderSettingsEditDto
                    {
                        Google_IsEnabled = GoogleEnabled,
                        Google = googleSettings.IsNullOrWhiteSpace()
                            ? new GoogleExternalLoginProviderSettings()
                            : googleSettings.FromJsonString<GoogleExternalLoginProviderSettings>(),

                        Microsoft_IsEnabled = MicrosoftEnabled,
                        Microsoft = microsoftSettings.IsNullOrWhiteSpace()
                            ? new MicrosoftExternalLoginProviderSettings()
                            : microsoftSettings.FromJsonString<MicrosoftExternalLoginProviderSettings>(),

                        OpenIdConnect_IsEnabled = OpenIdConnectEnabled,
                        OpenIdConnect = openIdConnectSettings.IsNullOrWhiteSpace()
                            ? new OpenIdConnectExternalLoginProviderSettings()
                            : openIdConnectSettings.FromJsonString<OpenIdConnectExternalLoginProviderSettings>(),
                        OpenIdConnectClaimsMapping = openIdConnectMapperClaims.IsNullOrWhiteSpace()
                            ? new List<JsonClaimMapDto>()
                            : openIdConnectMapperClaims.FromJsonString<List<JsonClaimMapDto>>(),
                        AuthZero_IsEnabled = AuthZeroEnabled,
                        AuthZero = authZeroSettings.IsNullOrWhiteSpace()
                            ? new AuthZeroExternalLoginProviderSettings()
                            : authZeroSettings.FromJsonString<AuthZeroExternalLoginProviderSettings>(),
                    };
                }
                else
                {
                    return new ExternalLoginProviderSettingsEditDto
                    {
                        Google_IsEnabled = GoogleEnabled,
                        Google = new GoogleExternalLoginProviderSettings(),
                        Microsoft_IsEnabled = MicrosoftEnabled,
                        Microsoft = new MicrosoftExternalLoginProviderSettings(),
                        OpenIdConnect_IsEnabled = OpenIdConnectEnabled,
                        OpenIdConnect = new OpenIdConnectExternalLoginProviderSettings(),
                        OpenIdConnectClaimsMapping = new List<JsonClaimMapDto>(),
                        AuthZero_IsEnabled = AuthZeroEnabled,
                        AuthZero = new AuthZeroExternalLoginProviderSettings()
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "GetExternalLoginProviderSettings {0}", ex.Message);
                var externalLoginProviderSettingsEditDto = new ExternalLoginProviderSettingsEditDto
                {
                    Google_IsEnabled = false,
                    Google = new GoogleExternalLoginProviderSettings(),
                    Microsoft_IsEnabled = false,
                    Microsoft = new MicrosoftExternalLoginProviderSettings(),
                    OpenIdConnect_IsEnabled = false,
                    OpenIdConnect = new OpenIdConnectExternalLoginProviderSettings(),
                    OpenIdConnectClaimsMapping = new List<JsonClaimMapDto>(),
                    AuthZero_IsEnabled = false,
                    AuthZero = new AuthZeroExternalLoginProviderSettings()
                };
                try
                {
                    //Replace a config with error for new clear config
                    await UpdateExternalLoginSettingsAsync(externalLoginProviderSettingsEditDto);
                }
                catch
                {
                    //Igonre
                }
                return externalLoginProviderSettingsEditDto;
            }
        }

        private async Task<GeneralSettingsEditDto> GetGeneralSettingsAsync()
        {
            var timezone = await SettingManager.GetSettingValueForApplicationAsync(TimingSettingNames.TimeZone);
            var settings = new GeneralSettingsEditDto
            {
                Timezone = timezone,
                TimezoneForComparison = timezone
            };

            var defaultTimeZoneId = await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Application, AbpSession.TenantId);
            if (settings.Timezone == defaultTimeZoneId)
            {
                settings.Timezone = string.Empty;
            }

            return settings;
        }

        private async Task<GoogleSettingsEditDto> GetGoogleSettingsAsync()
        {
            return new GoogleSettingsEditDto
            {
                Analytics = await SettingManager.GetSettingValueForApplicationAsync(EafMiddlewareSettingNames.Google.Analytics),
                Tag = await SettingManager.GetSettingValueForApplicationAsync(EafMiddlewareSettingNames.Google.TagManager),
                RecaptchaSiteKey = await SettingManager.GetSettingValueForApplicationAsync(EafMiddlewareSettingNames.Google.RecaptchaSiteKey)
            };
        }

        private async Task<LdapSettingsEditDto> GetLdapSettingsAsync(bool isAdmin)
        {
            return new LdapSettingsEditDto
            {
                IsModuleEnabled = true,
                IsEnabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(LdapSettingNames.IsEnabled),
                Domain = isAdmin ? await SettingManager.GetSettingValueForApplicationAsync(LdapSettingNames.Domain) : "",
                UserName = isAdmin ? await SettingManager.GetSettingValueForApplicationAsync(LdapSettingNames.UserName) : "",
                Password = isAdmin ? await SettingManager.GetSettingValueForApplicationAsync(LdapSettingNames.Password) : "",
            };
        }

        private async Task<bool> GetOneConcurrentLoginPerUserSetting()
        {
            return await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.UserManagement
                .AllowOneConcurrentLoginPerUser);
        }

        private async Task<SecuritySettingsEditDto> GetSecuritySettingsAsync()
        {
            var passwordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit = await SettingManager.GetSettingValueForApplicationAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit),
                RequireLowercase = await SettingManager.GetSettingValueForApplicationAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase),
                RequireNonAlphanumeric = await SettingManager.GetSettingValueForApplicationAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric),
                RequireUppercase = await SettingManager.GetSettingValueForApplicationAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase),
                RequiredLength = await SettingManager.GetSettingValueForApplicationAsync<int>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength)
            };

            var defaultPasswordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit = Convert.ToBoolean(_settingDefinitionManager.GetSettingDefinition(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit).DefaultValue),
                RequireLowercase = Convert.ToBoolean(_settingDefinitionManager.GetSettingDefinition(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase).DefaultValue),
                RequireNonAlphanumeric = Convert.ToBoolean(_settingDefinitionManager.GetSettingDefinition(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric).DefaultValue),
                RequireUppercase = Convert.ToBoolean(_settingDefinitionManager.GetSettingDefinition(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase).DefaultValue),
                RequiredLength = Convert.ToInt32(_settingDefinitionManager.GetSettingDefinition(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength).DefaultValue)
            };

            return new SecuritySettingsEditDto
            {
                UseDefaultPasswordComplexitySettings = passwordComplexitySetting.Equals(defaultPasswordComplexitySetting),
                PasswordComplexity = passwordComplexitySetting,
                DefaultPasswordComplexity = defaultPasswordComplexitySetting,
                UserLockOut = await GetUserLockOutSettingsAsync(),
                TwoFactorLogin = await GetTwoFactorLoginSettingsAsync(),
                AllowOneConcurrentLoginPerUser = await GetOneConcurrentLoginPerUserSetting()
            };
        }

        private async Task<TwoFactorLoginSettingsEditDto> GetTwoFactorLoginSettingsAsync()
        {
            var twoFactorLoginSettingsEditDto = new TwoFactorLoginSettingsEditDto
            {
                IsEnabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.UserManagement
                    .TwoFactorLogin.IsEnabled),
                IsEmailProviderEnabled =
                    await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.UserManagement.TwoFactorLogin
                        .IsEmailProviderEnabled),
                IsSmsProviderEnabled =
                    await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.UserManagement.TwoFactorLogin
                        .IsSmsProviderEnabled),
                IsRememberBrowserEnabled =
                    await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.UserManagement.TwoFactorLogin
                        .IsRememberBrowserEnabled)
            };
            return twoFactorLoginSettingsEditDto;
        }

        private async Task<UserLockOutSettingsEditDto> GetUserLockOutSettingsAsync()
        {
            return new UserLockOutSettingsEditDto
            {
                IsEnabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled),
                MaxFailedAccessAttemptsBeforeLockout = await SettingManager.GetSettingValueForApplicationAsync<int>(AbpZeroSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout),
                DefaultAccountLockoutSeconds = await SettingManager.GetSettingValueForApplicationAsync<int>(AbpZeroSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds)
            };
        }

        private async Task<HostUserManagementSettingsEditDto> GetUserManagementAsync()
        {
            return new HostUserManagementSettingsEditDto
            {
                IsRegisterRequiredForLogin = await SettingManager.GetSettingValueForApplicationAsync<bool>(EafMiddlewareSettingNames.UserManagement.IsRegisterRequiredForLogin),
                IsEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueForApplicationAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin),
                IsCookieConsentEnabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.UserManagement.IsCookieConsentEnabled),
                TokenExpiration = await SettingManager.GetSettingValueForApplicationAsync<int>(AppSettings.UserManagement.TokenExpiration),
                StoreExternalTokenInformation = await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.UserManagement.StoreExternalTokenInformation),
                AllowOneConcurrentLoginPerUser = await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser),
                UseCaptchaOnLogin = await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.UserManagement.UseCaptchaOnLogin)
            };
        }

        private async Task<ExpiredEntityLoginImpersonatorSettingsEditDto> GetLoginImpersonatorAsync()
        {
            try
            {
                return new ExpiredEntityLoginImpersonatorSettingsEditDto
                {
                    Enabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(EafMiddlewareSettingNames.LoginImpersonator.IsEnabled),
                };
            }
            catch
            {
                return new ExpiredEntityLoginImpersonatorSettingsEditDto
                {
                    Enabled = true,
                };
            }
        }

        private async Task<ExpiredEntityLogDeleterSettingsEditDto> GetLogDeleterAsync()
        {
            try
            {
                return new ExpiredEntityLogDeleterSettingsEditDto
                {
                    DeletedQuantity = await SettingManager.GetSettingValueForApplicationAsync<int>(EafMiddlewareSettingNames.LogDeleter.DeletedQuantity),
                    Enabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(EafMiddlewareSettingNames.LogDeleter.IsEnabled),
                    ExpiredDays = await SettingManager.GetSettingValueForApplicationAsync<int>(EafMiddlewareSettingNames.LogDeleter.ExpiredDays),
                };
            }
            catch
            {
                return new ExpiredEntityLogDeleterSettingsEditDto
                {
                    DeletedQuantity = 30000,
                    Enabled = true,
                    ExpiredDays = 3
                };
            }
        }

        #endregion Get Settings

        #region Update Settings

        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Settings)]
        public async Task UpdateAllSettings(HostSettingsEditDto input)
        {
            if (input == null) return;
            await UpdateGeneralSettingsAsync(input.General);
            await UpdateUserManagementSettingsAsync(input.UserManagement);
            await UpdateSecuritySettingsAsync(input.Security);
            await UpdateEmailSettingsAsync(input.Email);
            await UpdateGoogleSettingsAsync(input.Google);
            await UpdateExternalLoginSettingsAsync(input.ExternalLoginProviderSettings);

            if (_azureActiveDirectoryModuleConfig.IsEnabled)
                await UpdateAzureActiveDirectorySettingsAsync(input.AzureActiveDirectory);

            if (_ldapModuleConfig.IsEnabled)
                await UpdateLdapSettingsAsync(input.Ldap);

            await UpdateLogDeleterSettingsAsync(input.LogDeleter);
            await UpdateLoginAdmDBSettingsAsync(input.LoginImpersonator);
        }

        private async Task DeleteAllUsersByAuthSourceAsync(string authSource)
        {
            var users = UserManager.Users.Where(u => u.AuthenticationSource == authSource).ToList();
            foreach (var user in users)
            {
                await UserManager.DeleteAsync(user);
            }
        }

        private async Task UpdateAzureActiveDirectorySettingsAsync(AzureActiveDirectorySettingsEditDto input)
        {
            if (input == null) return;
            await SettingManager.ChangeSettingForApplicationAsync(AzureActiveDirectorySettingNames.IsEnabled, input.IsEnabled.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForApplicationAsync(AzureActiveDirectorySettingNames.ClientId, input.ClientId.IsNullOrWhiteSpace() ? null : input.ClientId);
            await SettingManager.ChangeSettingForApplicationAsync(AzureActiveDirectorySettingNames.Tenant, input.Tenant.IsNullOrWhiteSpace() ? null : input.Tenant);
            await SettingManager.ChangeSettingForApplicationAsync(AzureActiveDirectorySettingNames.ClientSecret, input.ClientSecret.IsNullOrWhiteSpace() ? null : input.ClientSecret);

            if (!input.IsEnabled)
                await DeleteAllUsersByAuthSourceAsync(AzureActiveDirectorySettingNames.ActiveDirectoryProvider);
        }

        private async Task UpdateEmailSettingsAsync(EmailSettingsEditDto settings)
        {
            if (settings == null) return;
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.DefaultFromAddress, settings.DefaultFromAddress);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.DefaultFromDisplayName, settings.DefaultFromDisplayName);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Host, settings.SmtpHost);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Port, settings.SmtpPort.ToString(CultureInfo.InvariantCulture));
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.UserName, settings.SmtpUserName);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Password, settings.SmtpPassword);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Domain, settings.SmtpDomain);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.EnableSsl, settings.SmtpEnableSsl.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.UseDefaultCredentials, settings.SmtpUseDefaultCredentials.ToString().ToLowerInvariant());
        }

        private async Task UpdateExternalLoginSettingsAsync(ExternalLoginProviderSettingsEditDto input)
        {
            if (input == null) return;

            bool openIdConnectEnabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsEnabled);
            bool microsoftEnabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled);
            bool googleEnabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.ExternalLoginProvider.Tenant.Google_IsEnabled);
            bool authZeroEnabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.ExternalLoginProvider.Tenant.AuthZero_IsEnabled);

            await SetExternalLoginToggleAsync(AppSettings.ExternalLoginProvider.Tenant.AuthZero_IsEnabled, authZeroEnabled, input.AuthZero, input.AuthZero_IsEnabled);
            await SetExternalLoginToggleAsync(AppSettings.ExternalLoginProvider.Tenant.Google_IsEnabled, googleEnabled, input.Google, input.Google_IsEnabled);
            await SetExternalLoginToggleAsync(AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled, microsoftEnabled, input.Microsoft, input.Microsoft_IsEnabled);
            await SetExternalLoginToggleAsync(AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsEnabled, openIdConnectEnabled, input.OpenIdConnect, input.OpenIdConnect_IsEnabled);

            await SetExternalLoginJsonAsync(AppSettings.ExternalLoginProvider.Host.AuthZero, input.AuthZero);
            await SetExternalLoginJsonAsync(AppSettings.ExternalLoginProvider.Host.Google, input.Google);
            await SetExternalLoginJsonAsync(AppSettings.ExternalLoginProvider.Host.Microsoft, input.Microsoft);
            await SetExternalLoginJsonAsync(AppSettings.ExternalLoginProvider.Host.OpenIdConnect, input.OpenIdConnect);

            await SetExternalLoginClaimsMappingAsync(AppSettings.ExternalLoginProvider.OpenIdConnectMappedClaims, input.OpenIdConnectClaimsMapping);
        }

        private async Task SetExternalLoginToggleAsync<T>(string settingName, bool currentEnabled, T provider, bool isEnabled) where T : class, IExternalLoginProviderSettings
        {
            var value = provider == null || !provider.IsValid()
                ? currentEnabled.ToString().ToLower()
                : isEnabled.ToString().ToLower();
            await SettingManager.ChangeSettingForApplicationAsync(settingName, value);
        }

        private async Task SetExternalLoginJsonAsync<T>(string settingName, T provider) where T : class, IExternalLoginProviderSettings
        {
            var value = provider == null || !provider.IsValid()
                ? _settingDefinitionManager.GetSettingDefinition(settingName).DefaultValue
                : provider.ToJsonString();
            await SettingManager.ChangeSettingForApplicationAsync(settingName, value);
        }

        private async Task SetExternalLoginClaimsMappingAsync(string settingName, List<JsonClaimMapDto> mapping)
        {
            var value = mapping.IsNullOrEmpty()
                ? _settingDefinitionManager.GetSettingDefinition(settingName).DefaultValue
                : mapping.ToJsonString();
            await SettingManager.ChangeSettingForApplicationAsync(settingName, value);
        }

        private async Task UpdateGeneralSettingsAsync(GeneralSettingsEditDto settings)
        {
            if (settings == null) return;
            if (Clock.SupportsMultipleTimezone)
            {
                if (settings.Timezone.IsNullOrEmpty())
                {
                    var defaultValue = await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Application, AbpSession.TenantId);
                    await SettingManager.ChangeSettingForApplicationAsync(TimingSettingNames.TimeZone, defaultValue);
                }
                else
                {
                    await SettingManager.ChangeSettingForApplicationAsync(TimingSettingNames.TimeZone, settings.Timezone);
                }
            }
        }

        private async Task UpdateGoogleSettingsAsync(GoogleSettingsEditDto input)
        {
            if (input == null) return;
            await SettingManager.ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.Google.TagManager, input.Tag.IsNullOrWhiteSpace() ? null : input.Tag);
            await SettingManager.ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.Google.Analytics, input.Analytics.IsNullOrWhiteSpace() ? null : input.Analytics);
            await SettingManager.ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.Google.RecaptchaSiteKey, input.RecaptchaSiteKey.IsNullOrWhiteSpace() ? null : input.RecaptchaSiteKey);
        }

        private async Task UpdateLogDeleterSettingsAsync(ExpiredEntityLogDeleterSettingsEditDto input)
        {
            if (input == null) return;
            await SettingManager.ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.LogDeleter.ExpiredDays, input.ExpiredDays == null ? "180" : input.ExpiredDays.ToString());
            await SettingManager.ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.LogDeleter.DeletedQuantity, input.DeletedQuantity == null ? "30000" : input.DeletedQuantity.ToString());
            await SettingManager.ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.LogDeleter.IsEnabled, input.Enabled == null ? "true" : input.Enabled.ToString().ToLower());
        }

        private async Task UpdateLoginAdmDBSettingsAsync(ExpiredEntityLoginImpersonatorSettingsEditDto input)
        {
            if (input == null) return;
            await SettingManager.ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.LoginImpersonator.IsEnabled, input.Enabled == null ? "true" : input.Enabled.ToString().ToLower());
        }

        private async Task UpdateLdapSettingsAsync(LdapSettingsEditDto input)
        {
            if (input == null) return;
            await SettingManager.ChangeSettingForApplicationAsync(LdapSettingNames.IsEnabled, input.IsEnabled.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForApplicationAsync(LdapSettingNames.Domain, input.Domain.IsNullOrWhiteSpace() ? null : input.Domain);
            await SettingManager.ChangeSettingForApplicationAsync(LdapSettingNames.UserName, input.UserName.IsNullOrWhiteSpace() ? null : input.UserName);
            await SettingManager.ChangeSettingForApplicationAsync(LdapSettingNames.Password, input.Password.IsNullOrWhiteSpace() ? null : input.Password);

            if (!input.IsEnabled)
                await DeleteAllUsersByAuthSourceAsync(LdapSettingNames.LdapProvider);
        }

        private async Task UpdateOneConcurrentLoginPerUserSettingAsync(bool allowOneConcurrentLoginPerUser)
        {
            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.AllowOneConcurrentLoginPerUser, allowOneConcurrentLoginPerUser.ToString());
        }

        private async Task UpdatePasswordComplexitySettingsAsync(PasswordComplexitySetting settings)
        {
            if (settings == null) return;
            await SettingManager.ChangeSettingForApplicationAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit,
                settings.RequireDigit.ToString()
            );

            await SettingManager.ChangeSettingForApplicationAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase,
                settings.RequireLowercase.ToString()
            );

            await SettingManager.ChangeSettingForApplicationAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric,
                settings.RequireNonAlphanumeric.ToString()
            );

            await SettingManager.ChangeSettingForApplicationAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase,
                settings.RequireUppercase.ToString()
            );

            await SettingManager.ChangeSettingForApplicationAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength,
                settings.RequiredLength.ToString()
            );
        }

        private async Task UpdateSecuritySettingsAsync(SecuritySettingsEditDto settings)
        {
            if (settings == null) return;
            if (settings.UseDefaultPasswordComplexitySettings)
            {
                await UpdatePasswordComplexitySettingsAsync(settings.DefaultPasswordComplexity);
            }
            else
            {
                await UpdatePasswordComplexitySettingsAsync(settings.PasswordComplexity);
            }

            await UpdateUserLockOutSettingsAsync(settings.UserLockOut);
            await UpdateTwoFactorLoginSettingsAsync(settings.TwoFactorLogin);
            await UpdateOneConcurrentLoginPerUserSettingAsync(settings.AllowOneConcurrentLoginPerUser);
        }

        private async Task UpdateTwoFactorLoginSettingsAsync(TwoFactorLoginSettingsEditDto settings)
        {
            if (settings == null) return;
            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.TwoFactorLogin.IsEnabled,
                settings.IsEnabled.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.TwoFactorLogin.IsEmailProviderEnabled,
                settings.IsEmailProviderEnabled.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.TwoFactorLogin.IsSmsProviderEnabled,
                settings.IsSmsProviderEnabled.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled,
                settings.IsRememberBrowserEnabled.ToString().ToLowerInvariant());
        }

        private async Task UpdateUserLockOutSettingsAsync(UserLockOutSettingsEditDto settings)
        {
            if (settings == null) return;
            await SettingManager.ChangeSettingForApplicationAsync(AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled, settings.IsEnabled.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForApplicationAsync(AbpZeroSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds, settings.DefaultAccountLockoutSeconds.ToString());
            await SettingManager.ChangeSettingForApplicationAsync(AbpZeroSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout, settings.MaxFailedAccessAttemptsBeforeLockout.ToString());
        }

        private async Task UpdateUserManagementSettingsAsync(HostUserManagementSettingsEditDto settings)
        {
            if (settings == null) return;
            await SettingManager.ChangeSettingForApplicationAsync(
                EafMiddlewareSettingNames.UserManagement.IsRegisterRequiredForLogin,
                settings.IsRegisterRequiredForLogin.ToString().ToLowerInvariant()
            );
            await SettingManager.ChangeSettingForApplicationAsync(
                AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin,
                settings.IsEmailConfirmationRequiredForLogin.ToString().ToLowerInvariant()
            );
            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.IsCookieConsentEnabled,
                settings.IsCookieConsentEnabled.ToString().ToLowerInvariant()
            );
            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.StoreExternalTokenInformation,
                settings.StoreExternalTokenInformation.ToString().ToLowerInvariant()
            );
            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.TokenExpiration,
                settings.TokenExpiration.ToString().ToLowerInvariant()
            );
            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.UseCaptchaOnLogin,
                settings.UseCaptchaOnLogin.ToString().ToLowerInvariant()
            );
            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.AllowOneConcurrentLoginPerUser,
                settings.AllowOneConcurrentLoginPerUser.ToString().ToLowerInvariant()
            );
        }

        #endregion Update Settings
    }
}