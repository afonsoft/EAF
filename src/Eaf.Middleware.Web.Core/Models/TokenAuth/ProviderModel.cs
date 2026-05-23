namespace Eaf.Middleware.Web.Models.TokenAuth
{
    /// <summary>
    /// Representa a classe ProviderModel.
    /// </summary>
    public class ProviderModel
    {
        /// <summary>
        /// Obtém ou define UsernameOrEmailAddress.
        /// </summary>
        public string UsernameOrEmailAddress { get; set; }
        /// <summary>
        /// Obtém ou define AuthenticationSource.
        /// </summary>
        public string AuthenticationSource { get; set; }
        /// <summary>
        /// Obtém ou define Tenant.
        /// </summary>
        public TenantModal Tenant { get; set; }
    }

    /// <summary>
    /// Representa a classe TenantModal.
    /// </summary>
    public class TenantModal
    {
        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Obtém ou define TenancyName.
        /// </summary>
        public string TenancyName { get; set; }
        /// <summary>
        /// Obtém ou define Id.
        /// </summary>
        public int Id { get; set; }
    }
}