using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.Middleware.Authorization.Roles.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authorization.Roles
{
    /// <summary>
    /// Application service that is used by 'role management' page.
    /// </summary>
    public interface IRoleAppService : IApplicationService
    {
        Task CreateOrUpdateRole(CreateOrUpdateRoleInput input);

        Task DeleteRole(EntityDto input);

        Task<GetRoleForEditOutput> GetRoleForEdit(NullableIdDto input);

        Task<ListResultDto<RoleListDto>> GetRoles(GetRolesInput input);
    }
}