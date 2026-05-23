using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.Middleware.Authorization.Permissions.Dto;

namespace Eaf.Middleware.Authorization.Permissions
{
    /// <summary>
    /// Representa a interface IPermissionAppService.
    /// </summary>
    public interface IPermissionAppService : IApplicationService
    {
        ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions();
    }
}