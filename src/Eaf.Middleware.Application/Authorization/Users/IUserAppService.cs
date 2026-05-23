using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.Middleware.Authorization.Users.Dto;
using Eaf.Middleware.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authorization.Users
{
    /// <summary>
    /// Representa a interface IUserAppService.
    /// </summary>
    public interface IUserAppService : IApplicationService
    {
        Task CloseSessionUser(int userId);

        Task CreateOrUpdateUser(CreateOrUpdateUserInput input);

        Task CreateUsersByActiveDirectory(CreateActiveDirectoryUserInput input);

        Task CreateUsersByLdap(CreateLdapUserInput input);

        Task DeleteUser(EntityDto<long> input);

        Task<List<UserListDto>> GetActiveDirectoryUsers(string name);

        Task<List<UserListDto>> GetLdapUsers(string name);

        Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input);

        Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input);

        Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input);

        Task<FileDto> GetUsersToExcel();

        Task ResetUserSpecificPermissions(EntityDto<long> input);

        Task UnlockUser(EntityDto<long> input);

        Task UpdateUserPermissions(UpdateUserPermissionsInput input);
    }
}