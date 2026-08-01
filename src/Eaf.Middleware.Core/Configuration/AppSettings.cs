namespace Eaf.Middleware.Configuration
{
    /// <summary>
    /// Representa a classe AppSettings.
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Representa a classe CacheKeys.
        /// </summary>
        public static class CacheKeys
        {
            public const string TenantRegistrationCache = "TenantRegistrationCache";
        }

        /// <summary>
        /// Representa a classe ExternalLoginProvider.
        /// </summary>
        public static class ExternalLoginProvider
        {
            public const string OpenIdConnectMappedClaims = "ExternalLoginProvider.OpenIdConnect.MappedClaims";

            /// <summary>
            /// Representa a classe Host.
            /// </summary>
            public static class Host
            {
                public const string Google = "ExternalLoginProvider.Google";
                public const string AuthZero = "ExternalLoginProvider.AuthZero";
                public const string Microsoft = "ExternalLoginProvider.Microsoft";
                public const string OpenIdConnect = "ExternalLoginProvider.OpenIdConnect";
            }

            /// <summary>
            /// Representa a classe Tenant.
            /// </summary>
            public static class Tenant
            {
                public const string Google = "ExternalLoginProvider.Google.Tenant";
                public const string Google_IsEnabled = "ExternalLoginProvider.Google.IsEnabled";
                public const string AuthZero = "ExternalLoginProvider.AuthZero.Tenant";
                public const string AuthZero_IsEnabled = "ExternalLoginProvider.AuthZero.IsEnabled";
                public const string Microsoft = "ExternalLoginProvider.Microsoft.Tenant";
                public const string Microsoft_IsEnabled = "ExternalLoginProvider.Microsoft.IsEnabled";
                public const string OpenIdConnect = "ExternalLoginProvider.OpenIdConnect.Tenant";
                public const string OpenIdConnect_IsEnabled = "ExternalLoginProvider.OpenIdConnect.IsEnabled";
            }
        }

        /// <summary>
        /// Representa a classe UiManagement.
        /// </summary>
        public static class UiManagement
        {
            public const string ContentSkin = "App.UiManagement.ContentSkin";
            public const string LayoutType = "App.UiManagement.LayoutType";
            public const string Theme = "App.UiManagement.Theme";
            public const string ThemeColor = "App.UiManagement.ThemeColor";

            /// <summary>
            /// Representa a classe Header.
            /// </summary>
            public static class Header
            {
                public const string DesktopFixedHeader = "App.UiManagement.Header.DesktopFixedHeader";
                public const string MobileFixedHeader = "App.UiManagement.Header.MobileFixedHeader";
                public const string Skin = "App.UiManagement.Header.Skin";
            }

            /// <summary>
            /// Representa a classe LeftAside.
            /// </summary>
            public static class LeftAside
            {
                public const string AllowAsideHiding = "App.UiManagement.Left.AllowAsideHiding";
                public const string AllowAsideMinimizing = "App.UiManagement.Left.AllowAsideMinimizing";
                public const string AsideSkin = "App.UiManagement.Left.AsideSkin";
                public const string DefaultHiddenAside = "App.UiManagement.Left.DefaultHiddenAside";
                public const string DefaultMinimizedAside = "App.UiManagement.Left.DefaultMinimizedAside";
                public const string FixedAside = "App.UiManagement.Left.FixedAside";
                public const string Position = "App.UiManagement.Left.Position";
            }
        }

        /// <summary>
        /// Representa as configurações de pagamento.
        /// </summary>
        public static class Payment
        {
            public const string DefaultGateway = "App.Payment.DefaultGateway";

            public static class Stripe
            {
                public const string SecretKey = "App.Payment.Stripe.SecretKey";
                public const string PublishableKey = "App.Payment.Stripe.PublishableKey";
                public const string WebhookSecret = "App.Payment.Stripe.WebhookSecret";
            }

            public static class PayPal
            {
                public const string ClientId = "App.Payment.PayPal.ClientId";
                public const string ClientSecret = "App.Payment.PayPal.ClientSecret";
                public const string WebhookId = "App.Payment.PayPal.WebhookId";
            }

            public static class MercadoPago
            {
                public const string AccessToken = "App.Payment.MercadoPago.AccessToken";
                public const string PublicKey = "App.Payment.MercadoPago.PublicKey";
            }

            public static class PagSeguro
            {
                public const string Token = "App.Payment.PagSeguro.Token";
                public const string Email = "App.Payment.PagSeguro.Email";
            }
        }

        /// <summary>
        /// Representa as configurações de gerenciamento de tenants.
        /// </summary>
        public static class TenantManagement
        {
            public const string AllowSelfRegistration = "App.TenantManagement.AllowSelfRegistration";
            public const string AllowTenantCreation = "App.TenantManagement.AllowTenantCreation";
            public const string AllowJoinRequests = "App.TenantManagement.AllowJoinRequests";
        }

        /// <summary>
        /// Representa a classe UserManagement.
        /// </summary>
        public static class UserManagement
        {
            public const string AllowOneConcurrentLoginPerUser = "App.UserManagement.AllowOneConcurrentLoginPerUser";
            public const string IsCookieConsentEnabled = "App.UserManagement.IsCookieConsentEnabled";
            public const string IsEmailConfirmationRequiredForLogin = "App.UserManagement.IsEmailConfirmationRequiredForLogin";
            public const string StoreExternalTokenInformation = "App.UserManagement.StoreExternalTokenInformation";
            public const string TokenExpiration = "App.UserManagement.TokenExpiration";
            public const string RefreshTokenExpirationInDays = "App.UserManagement.RefreshTokenExpirationInDays";
            public const string UseCaptchaOnLogin = "App.UserManagement.UseCaptchaOnLogin";

            /// <summary>
            /// Representa a classe TwoFactorLogin.
            /// </summary>
            public static class TwoFactorLogin
            {
                public const string IsEmailProviderEnabled = "App.UserManagement.TwoFactorLogin.IsEmailProviderEnabled";
                public const string IsEnabled = "App.UserManagement.TwoFactorLogin.IsEnabled";
                public const string IsRememberBrowserEnabled = "App.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled";
                public const string IsSmsProviderEnabled = "App.UserManagement.TwoFactorLogin.IsSmsProviderEnabled";
            }
        }
    }
}