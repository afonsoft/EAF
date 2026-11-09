using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Web.Models.TokenAuth;
using Riok.Mapperly.Abstractions;
using System.Collections.Generic;

namespace Eaf.Middleware.Web.Mapping
{
    /// <summary>
    /// Source-generated Mapperly mapper for Middleware.Web.Core models.
    /// </summary>
    // Lenient strategy matches the AutoMapper semantics this mapper replaces.
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    public partial class MiddlewareWebCoreMapperly
    {
        /// <summary>Maps an external login provider to its model.</summary>
        public partial ExternalLoginProviderInfoModel ToModel(ExternalLoginProviderInfo source);

        /// <summary>Maps a list of external login providers.</summary>
        public partial List<ExternalLoginProviderInfoModel> ToModelList(List<ExternalLoginProviderInfo> source);
    }
}
