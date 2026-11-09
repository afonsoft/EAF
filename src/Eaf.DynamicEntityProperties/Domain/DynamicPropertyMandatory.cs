using Abp;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Eaf.DynamicEntityProperties.Domain
{
    /// <summary>
    /// Metadados adicionais de uma dynamic property (a entidade DynamicProperty em si
    /// pertence ao pacote ABP e não pode ser estendida).
    /// </summary>
    [Table("EafDynamicPropertyMetadata")]
    public class DynamicPropertyMetadata : FullAuditedEntity<int>, IMayHaveTenant
    {
        /// <summary>Tamanho máximo do nome da propriedade.</summary>
        public const int MaxPropertyNameLength = 256;

        /// <summary>Nome único da dynamic property (<see cref="Abp.DynamicEntityProperties.DynamicProperty.PropertyName"/>).</summary>
        [Required]
        [StringLength(MaxPropertyNameLength)]
        public string PropertyName { get; set; }

        /// <summary>Tenant da definição.</summary>
        public int? TenantId { get; set; }

        /// <summary>Se o valor é obrigatório em writes de entidades.</summary>
        public bool IsMandatory { get; set; }
    }

    /// <summary>
    /// Acesso aos metadados de dynamic properties.
    /// </summary>
    public interface IDynamicPropertyMetadataManager
    {
        /// <summary>Verdadeiro se a propriedade é obrigatória no tenant.</summary>
        Task<bool> IsMandatoryAsync(int? tenantId, string propertyName);

        /// <summary>Define (upsert) o flag obrigatório da propriedade.</summary>
        Task SetMandatoryAsync(int? tenantId, string propertyName, bool isMandatory);

        /// <summary>Remove os metadados da propriedade (ao excluir a definição).</summary>
        Task RemoveAsync(int? tenantId, string propertyName);
    }

    /// <summary>
    /// Implementação de <see cref="IDynamicPropertyMetadataManager"/>.
    /// </summary>
    public class DynamicPropertyMetadataManager : DomainService, IDynamicPropertyMetadataManager
    {
        private readonly IRepository<DynamicPropertyMetadata, int> _repository;

        /// <summary>
        /// DynamicPropertyMetadataManager.
        /// </summary>
        public DynamicPropertyMetadataManager(IRepository<DynamicPropertyMetadata, int> repository)
        {
            _repository = repository;
        }

        /// <inheritdoc />
        public async Task<bool> IsMandatoryAsync(int? tenantId, string propertyName)
        {
            var meta = await _repository.FirstOrDefaultAsync(
                m => m.TenantId == tenantId && m.PropertyName == propertyName);
            return meta?.IsMandatory == true;
        }

        /// <inheritdoc />
        public async Task SetMandatoryAsync(int? tenantId, string propertyName, bool isMandatory)
        {
            var meta = await _repository.FirstOrDefaultAsync(
                m => m.TenantId == tenantId && m.PropertyName == propertyName);
            if (meta == null)
            {
                meta = new DynamicPropertyMetadata
                {
                    TenantId = tenantId,
                    PropertyName = propertyName
                };
            }
            meta.IsMandatory = isMandatory;
            await _repository.InsertOrUpdateAsync(meta);
        }

        /// <inheritdoc />
        public async Task RemoveAsync(int? tenantId, string propertyName)
        {
            var meta = await _repository.FirstOrDefaultAsync(
                m => m.TenantId == tenantId && m.PropertyName == propertyName);
            if (meta != null)
            {
                await _repository.DeleteAsync(meta);
            }
        }
    }
}
