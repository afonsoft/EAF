using Abp.Application.Services.Dto;

namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa um tenant disponível para solicitação de ingresso.
    /// </summary>
    public class AvailableTenantDto : EntityDto<int>
    {
        /// <summary>
        /// Nome técnico do tenant.
        /// </summary>
        public string TenancyName { get; set; }

        /// <summary>
        /// Nome de exibição do tenant.
        /// </summary>
        public string Name { get; set; }
    }
}
