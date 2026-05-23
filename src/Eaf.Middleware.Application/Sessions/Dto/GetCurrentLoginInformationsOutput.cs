using Eaf.Middleware.UiCustomization.Dto;

namespace Eaf.Middleware.Sessions.Dto
{
    /// <summary>
    /// Representa a classe GetCurrentLoginInformationsOutput.
    /// </summary>
    public class GetCurrentLoginInformationsOutput
    {
        /// <summary>
        /// Obtém ou define Application.
        /// </summary>
        public ApplicationInfoDto Application { get; set; }
        /// <summary>
        /// Obtém ou define Tenant.
        /// </summary>
        public TenantLoginInfoDto Tenant { get; set; }
        /// <summary>
        /// Obtém ou define Theme.
        /// </summary>
        public UiCustomizationSettingsDto Theme { get; set; }
        /// <summary>
        /// Obtém ou define User.
        /// </summary>
        public UserLoginInfoDto User { get; set; }
    }
}