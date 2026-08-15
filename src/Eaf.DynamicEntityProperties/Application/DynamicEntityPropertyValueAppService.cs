using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.DynamicEntityProperties;
using Abp.UI;
using Eaf.DynamicEntityProperties.Application.Dto;
using Eaf.DynamicEntityProperties.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.DynamicEntityProperties.Application
{
    /// <summary>
    /// Application service for managing dynamic entity property values.
    /// </summary>
    [AbpAuthorize(EafDynamicEntityPropertiesPermissions.DynamicProperties)]
    public class DynamicEntityPropertyValueAppService : ApplicationService
    {
        private readonly IDynamicEntityPropertyValueManager _dynamicEntityPropertyValueManager;

        /// <summary>
        /// Creates a new instance of <see cref="DynamicEntityPropertyValueAppService"/>.
        /// </summary>
        public DynamicEntityPropertyValueAppService(IDynamicEntityPropertyValueManager dynamicEntityPropertyValueManager)
        {
            _dynamicEntityPropertyValueManager = dynamicEntityPropertyValueManager;
        }

        /// <summary>
        /// Gets a dynamic entity property value by identifier.
        /// </summary>
        public async Task<DynamicEntityPropertyValueDto> GetAsync(long id)
        {
            var value = await _dynamicEntityPropertyValueManager.GetAsync(id);
            return ObjectMapper.Map<DynamicEntityPropertyValueDto>(value);
        }

        /// <summary>
        /// Lists dynamic entity property values using the provided filters.
        /// </summary>
        public async Task<ListResultDto<DynamicEntityPropertyValueDto>> GetAllAsync(GetDynamicEntityPropertyValuesInput input)
        {
            List<DynamicEntityPropertyValue> values;

            if (input.DynamicEntityPropertyId > 0 && !string.IsNullOrWhiteSpace(input.EntityId))
            {
                values = await _dynamicEntityPropertyValueManager.GetValuesAsync(input.DynamicEntityPropertyId, input.EntityId);
            }
            else if (!string.IsNullOrWhiteSpace(input.EntityFullName) && !string.IsNullOrWhiteSpace(input.EntityId))
            {
                if (input.DynamicPropertyId > 0)
                {
                    values = await _dynamicEntityPropertyValueManager.GetValuesAsync(input.EntityFullName, input.EntityId, input.DynamicPropertyId);
                }
                else if (!string.IsNullOrWhiteSpace(input.PropertyName))
                {
                    values = await _dynamicEntityPropertyValueManager.GetValuesAsync(input.EntityFullName, input.EntityId, input.PropertyName);
                }
                else
                {
                    values = await _dynamicEntityPropertyValueManager.GetValuesAsync(input.EntityFullName, input.EntityId);
                }
            }
            else
            {
                throw new UserFriendlyException("DynamicEntityPropertyId and EntityId or EntityFullName and EntityId are required.");
            }

            return new ListResultDto<DynamicEntityPropertyValueDto>(ObjectMapper.Map<List<DynamicEntityPropertyValueDto>>(values));
        }

        /// <summary>
        /// Creates a dynamic entity property value.
        /// </summary>
        [AbpAuthorize(EafDynamicEntityPropertiesPermissions.DynamicProperties_Create)]
        public async Task<DynamicEntityPropertyValueDto> CreateAsync(CreateOrUpdateDynamicEntityPropertyValueInput input)
        {
            var value = new DynamicEntityPropertyValue
            {
                EntityId = input.EntityId,
                DynamicEntityPropertyId = input.DynamicEntityPropertyId,
                Value = input.Value,
                TenantId = AbpSession.TenantId
            };

            await _dynamicEntityPropertyValueManager.AddAsync(value);
            return ObjectMapper.Map<DynamicEntityPropertyValueDto>(value);
        }

        /// <summary>
        /// Updates a dynamic entity property value.
        /// </summary>
        [AbpAuthorize(EafDynamicEntityPropertiesPermissions.DynamicProperties_Edit)]
        public async Task<DynamicEntityPropertyValueDto> UpdateAsync(CreateOrUpdateDynamicEntityPropertyValueInput input)
        {
            var value = await _dynamicEntityPropertyValueManager.GetAsync(input.Id);
            value.Value = input.Value;

            await _dynamicEntityPropertyValueManager.UpdateAsync(value);
            return ObjectMapper.Map<DynamicEntityPropertyValueDto>(value);
        }

        /// <summary>
        /// Deletes a dynamic entity property value.
        /// </summary>
        [AbpAuthorize(EafDynamicEntityPropertiesPermissions.DynamicProperties_Delete)]
        public async Task DeleteAsync(long id)
        {
            await _dynamicEntityPropertyValueManager.DeleteAsync(id);
        }
    }
}
