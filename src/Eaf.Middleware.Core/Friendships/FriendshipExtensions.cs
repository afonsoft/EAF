using Abp;

namespace Eaf.Middleware.Friendships
{
    /// <summary>
    /// Representa a classe FriendshipExtensions.
    /// </summary>
    public static class FriendshipExtensions
    {
        /// <summary>
        /// ToFriendIdentifier.
        /// </summary>
        /// <param name="friendship">Parâmetro friendship.</param>
        /// <returns>Resultado da operação.</returns>
        public static UserIdentifier ToFriendIdentifier(this Friendship friendship)
        {
            return new UserIdentifier(friendship.FriendTenantId, friendship.FriendUserId);
        }

        /// <summary>
        /// ToUserIdentifier.
        /// </summary>
        /// <param name="friendship">Parâmetro friendship.</param>
        /// <returns>Resultado da operação.</returns>
        public static UserIdentifier ToUserIdentifier(this Friendship friendship)
        {
            return new UserIdentifier(friendship.TenantId, friendship.UserId);
        }
    }
}