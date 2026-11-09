using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.DynamicEntityProperties;
using Eaf.DynamicEntityProperties.Application.Dto;
using Eaf.DynamicEntityProperties.Domain;
using Eaf.DynamicEntityProperties.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.DynamicEntityProperties.Application
{
    /// <summary>
    /// Application service for managing dynamic property definitions.
    /// </summary>
    [AbpAuthorize(EafDynamicEntityPropertiesPermissions.DynamicProperties)]
    public class DynamicPropertyAppService : ApplicationService
    {
        private readonly IDynamicPropertyManager _dynamicPropertyManager;
        private readonly IDynamicPropertyStore _dynamicPropertyStore;
        private readonly IDynamicPropertyValueManager _dynamicPropertyValueManager;
        private readonly IDynamicPropertyMetadataManager _metadataManager;

        /// <summary>
        /// Creates a new instance of <see cref="DynamicPropertyAppService"/>.
        /// </summary>
        public DynamicPropertyAppService(
            IDynamicPropertyManager dynamicPropertyManager,
            IDynamicPropertyStore dynamicPropertyStore,
            IDynamicPropertyValueManager dynamicPropertyValueManager,
            IDynamicPropertyMetadataManager metadataManager)
        {
            _dynamicPropertyManager = dynamicPropertyManager;
            _dynamicPropertyStore = dynamicPropertyStore;
            _dynamicPropertyValueManager = dynamicPropertyValueManager;
            _metadataManager = metadataManager;
        }

        /// <summary>
        /// Gets a dynamic property by identifier.
        /// </summary>
        public async Task<DynamicPropertyDto> GetAsync(int id)
        {
            var property = await _dynamicPropertyManager.GetAsync(id);
            var values = await _dynamicPropertyValueManager.GetAllValuesOfDynamicPropertyAsync(id);
            property.DynamicPropertyValues = values;

            var dto = ObjectMapper.Map<DynamicPropertyDto>(property);
            dto.IsMandatory = await _metadataManager.IsMandatoryAsync(AbpSession.TenantId, property.PropertyName);
            return dto;
        }

        /// <summary>
        /// Lists all dynamic properties.
        /// </summary>
        public async Task<ListResultDto<DynamicPropertyDto>> GetAllAsync()
        {
            var properties = await _dynamicPropertyStore.GetAllAsync();
            var dtos = ObjectMapper.Map<List<DynamicPropertyDto>>(properties);
            foreach (var dto in dtos)
            {
                dto.IsMandatory = await _metadataManager.IsMandatoryAsync(AbpSession.TenantId, dto.PropertyName);
            }
            return new ListResultDto<DynamicPropertyDto>(dtos);
        }

        /// <summary>
        /// Creates a dynamic property with its predefined values.
        /// </summary>
        [AbpAuthorize(EafDynamicEntityPropertiesPermissions.DynamicProperties_Create)]
        public async Task<DynamicPropertyDto> CreateAsync(CreateOrUpdateDynamicPropertyInput input)
        {
            var property = new DynamicProperty
            {
                PropertyName = input.PropertyName,
                DisplayName = input.DisplayName,
                InputType = input.InputType,
                Permission = input.Permission,
                TenantId = AbpSession.TenantId
            };

            await _dynamicPropertyManager.AddAsync(property);
            await _metadataManager.SetMandatoryAsync(AbpSession.TenantId, property.PropertyName, input.IsMandatory);
            await AddValuesAsync(property.Id, input.Values);

            return ObjectMapper.Map<DynamicPropertyDto>(property);
        }

        /// <summary>
        /// Updates a dynamic property and replaces its predefined values.
        /// </summary>
        [AbpAuthorize(EafDynamicEntityPropertiesPermissions.DynamicProperties_Edit)]
        public async Task<DynamicPropertyDto> UpdateAsync(CreateOrUpdateDynamicPropertyInput input)
        {
            var property = await _dynamicPropertyManager.GetAsync(input.Id);

            property.PropertyName = input.PropertyName;
            property.DisplayName = input.DisplayName;
            property.InputType = input.InputType;
            property.Permission = input.Permission;

            await _dynamicPropertyManager.UpdateAsync(property);
            await _metadataManager.SetMandatoryAsync(AbpSession.TenantId, property.PropertyName, input.IsMandatory);
            await _dynamicPropertyValueManager.CleanValuesAsync(property.Id);
            await AddValuesAsync(property.Id, input.Values);

            return ObjectMapper.Map<DynamicPropertyDto>(property);
        }

        /// <summary>
        /// Deletes a dynamic property and its values.
        /// </summary>
        [AbpAuthorize(EafDynamicEntityPropertiesPermissions.DynamicProperties_Delete)]
        public async Task DeleteAsync(int id)
        {
            var property = await _dynamicPropertyManager.GetAsync(id);
            await _metadataManager.RemoveAsync(AbpSession.TenantId, property?.PropertyName);
            await _dynamicPropertyManager.DeleteAsync(id);
        }

        private async Task AddValuesAsync(int dynamicPropertyId, List<DynamicPropertyValueDto> values)
        {
            if (values == null)
            {
                return;
            }

            foreach (var value in values)
            {
                var dynamicPropertyValue = new DynamicPropertyValue
                {
                    DynamicPropertyId = dynamicPropertyId,
                    Value = value.Value,
                    TenantId = AbpSession.TenantId
                };

                await _dynamicPropertyValueManager.AddAsync(dynamicPropertyValue);
            }
        }
    }
}
