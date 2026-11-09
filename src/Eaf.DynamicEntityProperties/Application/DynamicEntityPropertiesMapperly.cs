using Abp.Dependency;
using Abp.DynamicEntityProperties;
using Abp.ObjectMapping;
using AutoMapper;
using Eaf.DynamicEntityProperties.Application.Dto;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eaf.DynamicEntityProperties.Application
{
    /// <summary>
    /// Source-generated Mapperly mapper for dynamic entity property DTOs.
    /// Replaces the runtime reflection-based AutoMapper profile for this module.
    /// </summary>
    [Mapper]
    public partial class DynamicEntityPropertiesMapperly
    {
        /// <summary>Maps a dynamic property definition to its DTO (Values &lt;- DynamicPropertyValues).</summary>
        [MapProperty(nameof(DynamicProperty.DynamicPropertyValues), nameof(DynamicPropertyDto.Values))]
        public partial DynamicPropertyDto ToDto(DynamicProperty source);

        /// <summary>Maps a dynamic property value to its DTO.</summary>
        public partial DynamicPropertyValueDto ToDto(DynamicPropertyValue source);

        /// <summary>Maps a dynamic entity property to its DTO.</summary>
        public partial DynamicEntityPropertyDto ToDto(DynamicEntityProperty source);

        /// <summary>Maps a dynamic entity property value to its DTO.</summary>
        public partial DynamicEntityPropertyValueDto ToDto(DynamicEntityPropertyValue source);

        /// <summary>Maps a list of dynamic properties.</summary>
        public partial List<DynamicPropertyDto> ToDtoList(List<DynamicProperty> source);

        /// <summary>Maps a list of dynamic entity properties.</summary>
        public partial List<DynamicEntityPropertyDto> ToDtoList(List<DynamicEntityProperty> source);

        /// <summary>Maps a list of dynamic entity property values.</summary>
        public partial List<DynamicEntityPropertyValueDto> ToDtoList(List<DynamicEntityPropertyValue> source);
    }

    /// <summary>
    /// <see cref="IObjectMapper"/> adapter that dispatches the pairs this module owns to the
    /// source-generated <see cref="DynamicEntityPropertiesMapperly"/> mapper and falls back to
    /// the registered AutoMapper <see cref="IMapper"/> for every other pair during the migration.
    /// </summary>
    public class EafMapperlyObjectMapper : IObjectMapper
    {
        private static readonly DynamicEntityPropertiesMapperly Generated = new();

        private static readonly Dictionary<(Type Source, Type Destination), Func<object, object>> Dispatch =
            new()
            {
                [(typeof(DynamicProperty), typeof(DynamicPropertyDto))] = s => Generated.ToDto((DynamicProperty)s),
                [(typeof(DynamicPropertyValue), typeof(DynamicPropertyValueDto))] = s => Generated.ToDto((DynamicPropertyValue)s),
                [(typeof(DynamicEntityProperty), typeof(DynamicEntityPropertyDto))] = s => Generated.ToDto((DynamicEntityProperty)s),
                [(typeof(DynamicEntityPropertyValue), typeof(DynamicEntityPropertyValueDto))] = s => Generated.ToDto((DynamicEntityPropertyValue)s),
                [(typeof(List<DynamicProperty>), typeof(List<DynamicPropertyDto>))] = s => Generated.ToDtoList((List<DynamicProperty>)s),
                [(typeof(List<DynamicEntityProperty>), typeof(List<DynamicEntityPropertyDto>))] = s => Generated.ToDtoList((List<DynamicEntityProperty>)s),
                [(typeof(List<DynamicEntityPropertyValue>), typeof(List<DynamicEntityPropertyValueDto>))] = s => Generated.ToDtoList((List<DynamicEntityPropertyValue>)s),
            };

        private readonly IMapper _autoMapper;

        /// <summary>
        /// EafMapperlyObjectMapper.
        /// </summary>
        public EafMapperlyObjectMapper(IMapper autoMapper)
        {
            _autoMapper = autoMapper;
        }

        /// <inheritdoc />
        public TDestination Map<TDestination>(object source)
        {
            if (source != null
                && Dispatch.TryGetValue((source.GetType(), typeof(TDestination)), out var map))
            {
                return (TDestination)map(source);
            }
            return _autoMapper.Map<TDestination>(source);
        }

        /// <inheritdoc />
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return _autoMapper.Map(source, destination);
        }

        /// <inheritdoc />
        public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source)
        {
            return _autoMapper.ProjectTo<TDestination>(source);
        }
    }
}
