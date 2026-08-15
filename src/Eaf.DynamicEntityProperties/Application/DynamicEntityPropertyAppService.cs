using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.DynamicEntityProperties;
using Eaf.DynamicEntityProperties.Application.Dto;
using Eaf.DynamicEntityProperties.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.DynamicEntityProperties.Application
{
    /// <summary>
    /// Application service for binding dynamic properties to entity types.
    /// </summary>
    [AbpAuthorize(EafDynamicEntityPropertiesPermissions.DynamicProperties)]
    public class DynamicEntityPropertyAppService : ApplicationService
    {
        private readonly IDynamicEntityPropertyManager _dynamicEntityPropertyManager;

        /// <summary>
        /// Creates a new instance of <see cref="DynamicEntityPropertyAppService"/>.
        /// </summary>
        public DynamicEntityPropertyAppService(IDynamicEntityPropertyManager dynamicEntityPropertyManager)
        {
            _dynamicEntityPropertyManager = dynamicEntityPropertyManager;
        }

        /// <summary>
        /// Gets a dynamic entity property by identifier.
        /// </summary>
        public async Task<DynamicEntityPropertyDto> GetAsync(int id)
        {
            var entityProperty = await _dynamicEntityPropertyManager.GetAsync(id);
            return ObjectMapper.Map<DynamicEntityPropertyDto>(entityProperty);
        }

        /// <summary>
        /// Lists dynamic entity properties optionally filtered by entity full name.
        /// </summary>
        public async Task<ListResultDto<DynamicEntityPropertyDto>> GetAllAsync(string entityFullName)
        {
            var entityProperties = string.IsNullOrWhiteSpace(entityFullName)
                ? await _dynamicEntityPropertyManager.GetAllAsync()
                : await _dynamicEntityPropertyManager.GetAllAsync(entityFullName);

            return new ListResultDto<DynamicEntityPropertyDto>(ObjectMapper.Map<List<DynamicEntityPropertyDto>>(entityProperties));
        }

        /// <summary>
        /// Binds a dynamic property to an entity type.
        /// </summary>
        [AbpAuthorize(EafDynamicEntityPropertiesPermissions.DynamicProperties_Create)]
        public async Task<DynamicEntityPropertyDto> CreateAsync(CreateDynamicEntityPropertyInput input)
        {
            var entityProperty = new DynamicEntityProperty
            {
                EntityFullName = input.EntityFullName,
                DynamicPropertyId = input.DynamicPropertyId,
                TenantId = AbpSession.TenantId
            };

            await _dynamicEntityPropertyManager.AddAsync(entityProperty);
            return ObjectMapper.Map<DynamicEntityPropertyDto>(entityProperty);
        }

        /// <summary>
        /// Removes the binding between a dynamic property and an entity type.
        /// </summary>
        [AbpAuthorize(EafDynamicEntityPropertiesPermissions.DynamicProperties_Delete)]
        public async Task DeleteAsync(int id)
        {
            await _dynamicEntityPropertyManager.DeleteAsync(id);
        }
    }
}
