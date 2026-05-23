using Abp.Application.Services.Dto;

namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    //### This class is mapped in CustomDtoMapper ###
    /// <summary>
    /// Representa a classe CurrentTenantInfoDto.
    /// </summary>
    public class CurrentTenantInfoDto : EntityDto
    {
        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Obtém ou define TenancyName.
        /// </summary>
        public string TenancyName { get; set; }
    }
}