using Eaf.ProjectName.Airplanes;
using System;
using System.Linq;

namespace Eaf.ProjectName.EntityHistory
{
    public static class EntityHistoryHelper
    {
        public static readonly Type[] ProjectNameTrackedTypes =
        {
            typeof(Airplane)
        };

        public static Type[] TrackedTypes { get; } = ProjectNameTrackedTypes
            .GroupBy(type => type.FullName)
            .Select(types => types.First())
            .ToArray();
    }
}