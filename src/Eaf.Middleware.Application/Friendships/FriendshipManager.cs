using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.UI;
using Eaf.Middleware.Localization;
using System.Globalization;
using System.Threading.Tasks;

namespace Eaf.Middleware.Friendships
{
    /// <summary>
    /// Representa a classe FriendshipManager.
    /// </summary>
    public class FriendshipManager : DomainService, IFriendshipManager
    {
        private readonly IRepository<Friendship, long> _friendshipRepository;

        /// <summary>
        /// FriendshipManager.
        /// </summary>
        /// <param name="friendshipRepository">Parâmetro friendshipRepository.</param>
        /// <returns>Resultado da operação.</returns>
        public FriendshipManager(IRepository<Friendship, long> friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;

            LocalizationSourceName = MiddlewareAppConsts.LocalizationSourceName;
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources.
        /// </summary>
        protected override string L(string name)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources com formatação.
        /// </summary>
        protected override string L(string name, params object[] args)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, args);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources para uma cultura específica.
        /// </summary>
        protected override string L(string name, CultureInfo culture)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, culture);
        }

        [UnitOfWork]
        public async Task AcceptFriendshipRequestAsync(UserIdentifier userIdentifier, UserIdentifier probableFriend)
        {
            var friendship = (await GetFriendshipOrNullAsync(userIdentifier, probableFriend));
            if (friendship == null)
            {
                throw new AbpException("Friendship does not exist between " + userIdentifier + " and " + probableFriend);
            }

            friendship.State = FriendshipState.Accepted;
            await UpdateFriendshipAsync(friendship);
        }

        [UnitOfWork]
        public async Task BanFriendAsync(UserIdentifier userIdentifier, UserIdentifier probableFriend)
        {
            var friendship = (await GetFriendshipOrNullAsync(userIdentifier, probableFriend));
            if (friendship == null)
            {
                throw new AbpException("Friendship does not exist between " + userIdentifier + " and " + probableFriend);
            }

            friendship.State = FriendshipState.Blocked;
            await UpdateFriendshipAsync(friendship);
        }

        [UnitOfWork]
        public async Task CreateFriendshipAsync(Friendship friendship)
        {
            if (friendship.TenantId == friendship.FriendTenantId &&
                friendship.UserId == friendship.FriendUserId)
            {
                throw new UserFriendlyException(L("YouCannotBeFriendWithYourself"));
            }

            using (CurrentUnitOfWork.SetTenantId(friendship.TenantId))
            {
                await _friendshipRepository.InsertAsync(friendship);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public async Task<Friendship> GetFriendshipOrNullAsync(UserIdentifier user, UserIdentifier probableFriend)
        {
            using (CurrentUnitOfWork.SetTenantId(user.TenantId))
            {
                return await _friendshipRepository.FirstOrDefaultAsync(friendship =>
                                    friendship.UserId == user.UserId &&
                                    friendship.TenantId == user.TenantId &&
                                    friendship.FriendUserId == probableFriend.UserId &&
                                    friendship.FriendTenantId == probableFriend.TenantId);
            }
        }

        [UnitOfWork]
        public async Task UpdateFriendshipAsync(Friendship friendship)
        {
            using (CurrentUnitOfWork.SetTenantId(friendship.TenantId))
            {
                await _friendshipRepository.UpdateAsync(friendship);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }
    }
}