using Abp.Authorization;
using Eaf.Middleware.Authorization.Users.Profile;
using Eaf.Middleware.Storage;

namespace Eaf.Middleware.Web.Controllers
{
    [AbpAuthorize]
    public class ProfileController : ProfileControllerBase
    {
        /// <summary>
        /// ProfileController.
        /// </summary>
        /// <param name="tempFileCacheManager">Parâmetro tempFileCacheManager.</param>
        /// <param name="profileAppService">Parâmetro profileAppService.</param>
        /// <returns>Resultado da operação.</returns>
        public ProfileController(
             ITempFileCacheManager tempFileCacheManager,
             IProfileAppService profileAppService) :
             base(tempFileCacheManager, profileAppService)
        {
        }
    }
}