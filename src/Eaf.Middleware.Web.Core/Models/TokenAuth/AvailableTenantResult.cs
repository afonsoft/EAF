namespace Eaf.Middleware.Web.Models.TokenAuth
{
    /// <summary>
    /// Tenant disponível para o usuário host fazer login.
    /// </summary>
    public class AvailableTenantResult
    {
        /// <summary>
        /// Identificador do tenant.
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// Nome de exibição do tenant.
        /// </summary>
        public string TenantName { get; set; }

        /// <summary>
        /// Nome técnico do tenant (tenancy name).
        /// </summary>
        public string TenancyName { get; set; }

        /// <summary>
        /// Indica se este é o tenant padrão para o usuário.
        /// </summary>
        public bool IsDefault { get; set; }
    }
}
