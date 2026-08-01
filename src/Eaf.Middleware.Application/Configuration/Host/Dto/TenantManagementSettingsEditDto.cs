namespace Eaf.Middleware.Configuration.Host.Dto
{
    /// <summary>
    /// Configurações de gerenciamento de registro público de tenants.
    /// </summary>
    public class TenantManagementSettingsEditDto
    {
        /// <summary>
        /// Permite o cadastro público de usuários (registro independente de tenant).
        /// </summary>
        public bool AllowSelfRegistration { get; set; }

        /// <summary>
        /// Permite que novos tenants sejam criados durante o cadastro público.
        /// </summary>
        public bool AllowTenantCreation { get; set; }

        /// <summary>
        /// Permite que usuários solicitem ingresso em tenants existentes.
        /// </summary>
        public bool AllowJoinRequests { get; set; }
    }
}
