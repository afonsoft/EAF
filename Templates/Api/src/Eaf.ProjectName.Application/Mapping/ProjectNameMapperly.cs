using Eaf.ProjectName.Airplanes;
using Eaf.ProjectName.Airplanes.Dtos;
using Riok.Mapperly.Abstractions;
using System.Collections.Generic;

namespace Eaf.ProjectName.Mapping
{
    /// <summary>
    /// Source-generated Mapperly mapper for template DTOs.
    /// </summary>
    // Lenient strategy matches the AutoMapper semantics this mapper replaces.
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    public partial class ProjectNameMapperly
    {
        /// <summary>Maps an airplane create/edit DTO to the entity.</summary>
        public partial Airplane ToEntity(CreateOrEditAirplaneDto source);

        /// <summary>Maps an airplane entity to its DTO.</summary>
        public partial AirplaneDto ToDto(Airplane source);

        /// <summary>Maps a list of airplanes.</summary>
        public partial List<AirplaneDto> ToDtoList(List<Airplane> source);
    }
}
