using Abp;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eaf.Middleware.Friendships
{
    /// <summary>
    /// Representa a enumeração FriendshipState.
    /// </summary>
    public enum FriendshipState
    {
        Accepted = 1,
        Blocked = 2
    }

    /// <summary>
    /// Representa a classe Friendship.
    /// </summary>
    [Table("EafFriendships")]
    public class Friendship : Entity<long>, IHasCreationTime, IMayHaveTenant
    {
        /// <summary>
        /// Friendship.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        /// <param name="probableFriend">Parâmetro probableFriend.</param>
        /// <param name="probableFriendTenancyName">Parâmetro probableFriendTenancyName.</param>
        /// <param name="probableFriendUserName">Parâmetro probableFriendUserName.</param>
        /// <param name="probableFriendProfilePictureId">Parâmetro probableFriendProfilePictureId.</param>
        /// <param name="state">Parâmetro state.</param>
        /// <returns>Resultado da operação.</returns>
        public Friendship(UserIdentifier user, UserIdentifier probableFriend, string probableFriendTenancyName, string probableFriendUserName, Guid? probableFriendProfilePictureId, FriendshipState state)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (probableFriend == null)
            {
                throw new ArgumentNullException(nameof(probableFriend));
            }

            if (!Enum.IsDefined(typeof(FriendshipState), state))
            {
                throw new AbpException("Invalid FriendshipState value: " + state);
            }

            UserId = user.UserId;
            TenantId = user.TenantId;
            FriendUserId = probableFriend.UserId;
            FriendTenantId = probableFriend.TenantId;
            FriendTenancyName = probableFriendTenancyName;
            FriendUserName = probableFriendUserName;
            State = state;
            FriendProfilePictureId = probableFriendProfilePictureId;

            CreationTime = Clock.Now;
        }

        protected Friendship()
        {
        }

        /// <summary>
        /// Obtém ou define CreationTime.
        /// </summary>
        public DateTime CreationTime { get; set; }
        public Guid? FriendProfilePictureId { get; set; }
        /// <summary>
        /// Obtém ou define FriendTenancyName.
        /// </summary>
        public string FriendTenancyName { get; set; }
        public int? FriendTenantId { get; set; }
        /// <summary>
        /// Obtém ou define FriendUserId.
        /// </summary>
        public long FriendUserId { get; set; }

        [Required]
        [MaxLength(AbpUserBase.MaxUserNameLength)]
        public string FriendUserName { get; set; }

        /// <summary>
        /// Obtém ou define State.
        /// </summary>
        public FriendshipState State { get; set; }
        public int? TenantId { get; set; }
        /// <summary>
        /// Obtém ou define UserId.
        /// </summary>
        public long UserId { get; set; }
    }
}