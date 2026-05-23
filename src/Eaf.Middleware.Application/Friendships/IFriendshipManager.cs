using Abp;
using Abp.Domain.Services;
using System.Threading.Tasks;

namespace Eaf.Middleware.Friendships
{
    /// <summary>
    /// Representa a interface IFriendshipManager.
    /// </summary>
    public interface IFriendshipManager : IDomainService
    {
        Task AcceptFriendshipRequestAsync(UserIdentifier userIdentifier, UserIdentifier probableFriend);

        Task BanFriendAsync(UserIdentifier userIdentifier, UserIdentifier probableFriend);

        Task CreateFriendshipAsync(Friendship friendship);

        Task<Friendship> GetFriendshipOrNullAsync(UserIdentifier user, UserIdentifier probableFriend);

        Task UpdateFriendshipAsync(Friendship friendship);
    }
}