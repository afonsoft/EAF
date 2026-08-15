using Abp.AutoMapper;
using Abp.DynamicEntityProperties;
using AutoMapper;
using Eaf.DynamicEntityProperties.Application.Dto;

namespace Eaf.DynamicEntityProperties.Application
{
    /// <summary>
    /// AutoMapper configuration for dynamic entity property DTOs.
    /// </summary>
    public static class DynamicEntityPropertiesDtoMapper
    {
        /// <summary>
        /// Configures mapping between ABP dynamic entity property entities and EAF DTOs.
        /// </summary>
        /// <param name="configuration">Mapper configuration expression.</param>
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<DynamicProperty, DynamicPropertyDto>()
                .ForMember(d => d.Values, o => o.MapFrom(s => s.DynamicPropertyValues));

            configuration.CreateMap<DynamicPropertyValue, DynamicPropertyValueDto>();
            configuration.CreateMap<DynamicEntityProperty, DynamicEntityPropertyDto>();
            configuration.CreateMap<DynamicEntityPropertyValue, DynamicEntityPropertyValueDto>();
        }
    }
}
