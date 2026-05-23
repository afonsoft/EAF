using Abp.Application.Services;
using Eaf.Middleware.Friendships.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Friendships
{
    /// <summary>
    /// Representa a interface IFriendshipAppService.
    /// </summary>
    public interface IFriendshipAppService : IApplicationService
    {
        Task AcceptFriendshipRequest(AcceptFriendshipRequestInput input);

        Task BlockUser(BlockUserInput input);

        Task<FriendshipDto> CreateFriendshipRequest(CreateFriendshipRequestInput input);

        Task<FriendshipDto> CreateFriendshipRequestByUserName(CreateFriendshipRequestByUserNameInput input);

        Task UnblockUser(UnblockUserInput input);
    }
}