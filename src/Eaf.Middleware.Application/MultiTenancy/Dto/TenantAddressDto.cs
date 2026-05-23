using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.MultiTenancy.Dto
{
    /// <summary>
    /// Representa a classe TenantAddressDto.
    /// </summary>
    public class TenantAddressDto : FullAuditedEntityDto<int>, IExtendableObject
    {
        /// <summary>
        /// Cep
        /// </summary>
        [Required]
        [StringLength(10)]
        public string ZipCode { get; set; }

        /// <summary>
        /// logradouro
        /// </summary>
        [Required]
        [StringLength(512)]
        public string Street { get; set; }

        /// <summary>
        /// bairro
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Neighborhood { get; set; }

        /// <summary>
        /// localidade
        /// </summary>
        [Required]
        [StringLength(256)]
        public string City { get; set; }

        /// <summary>
        /// uf
        /// </summary>
        [Required]
        [StringLength(4)]
        public string State { get; set; }

        /// <summary>
        /// complemento
        /// </summary>
        [StringLength(256)]
        public string Complement { get; set; }

        /// <summary>
        /// observação
        /// </summary>
        [StringLength(512)]
        public string Observation { get; set; }

        /// <summary>
        /// Nome da Pessoa
        /// </summary>
        [StringLength(512)]
        public string Email { get; set; }

        /// <summary>
        /// Documento
        /// </summary>
        [StringLength(256)]
        public string Document { get; set; }

        /// <summary>
        /// Obtém ou define TenantId.
        /// </summary>
        public int TenantId { get; set; }

        [StringLength(2000)]
        public string ExtensionData { get; set; }
    }
}