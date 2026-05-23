using Abp.Application.Services.Dto;
using System;

namespace Eaf.Middleware.Sessions.Dto
{
    /// <summary>
    /// Representa a classe TenantLoginInfoDto.
    /// </summary>
    public class TenantLoginInfoDto : EntityDto
    {
        /// <summary>
        /// Obtém ou define CreationTime.
        /// </summary>
        public DateTime CreationTime { get; set; }
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