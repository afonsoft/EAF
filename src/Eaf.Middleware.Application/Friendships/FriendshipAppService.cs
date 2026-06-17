using Abp;
using Abp.Authorization;
using Abp.MultiTenancy;
using Abp.RealTime;
using Abp.Runtime.Session;
using Abp.UI;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships.Dto;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Friendships
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de Friendship.
    /// </summary>
    [AbpAuthorize]
    public class FriendshipAppService : MiddlewareAppServiceBase, IFriendshipAppService
    {
        private readonly IChatCommunicator _chatCommunicator;
        private readonly IChatFeatureChecker _chatFeatureChecker;
        private readonly IFriendshipManager _friendshipManager;
        private readonly IOnlineClientManager<ChatChannel> _onlineClientManager;
        private readonly ITenantCache _tenantCache;

        /// <summary>
        /// FriendshipAppService.
        /// </summary>
        /// <param name="friendshipManager">Parâmetro friendshipManager.</param>
        /// <param name="onlineClientManager">Parâmetro onlineClientManager.</param>
        /// <param name="chatCommunicator">Parâmetro chatCommunicator.</param>
        /// <param name="tenantCache">Parâmetro tenantCache.</param>
        /// <param name="chatFeatureChecker">Parâmetro chatFeatureChecker.</param>
        /// <returns>Resultado da operação.</returns>
        public FriendshipAppService(
            IFriendshipManager friendshipManager,
            IOnlineClientManager<ChatChannel> onlineClientManager,
            IChatCommunicator chatCommunicator,
            ITenantCache tenantCache,
            IChatFeatureChecker chatFeatureChecker)
        {
            _friendshipManager = friendshipManager;
            _onlineClientManager = onlineClientManager;
            _chatCommunicator = chatCommunicator;
            _tenantCache = tenantCache;
            _chatFeatureChecker = chatFeatureChecker;
        }

        /// <summary>
        /// AcceptFriendshipRequest.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        public async Task AcceptFriendshipRequest(AcceptFriendshipRequestInput input)
        {
            var userIdentifier = AbpSession.ToUserIdentifier();
            var friendIdentifier = new UserIdentifier(input.TenantId, input.UserId);
            await _friendshipManager.AcceptFriendshipRequestAsync(userIdentifier, friendIdentifier);

            var clients = await _onlineClientManager.GetAllByUserIdAsync(userIdentifier);
            if (clients.Any())
            {
                await _chatCommunicator.SendUserStateChangeToClients(clients, friendIdentifier, FriendshipState.Accepted);
            }
        }

        /// <summary>
        /// BlockUser.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        public async Task BlockUser(BlockUserInput input)
        {
            var userIdentifier = AbpSession.ToUserIdentifier();
            var friendIdentifier = new UserIdentifier(input.TenantId, input.UserId);
            await _friendshipManager.BanFriendAsync(userIdentifier, friendIdentifier);

            var clients = await _onlineClientManager.GetAllByUserIdAsync(userIdentifier);
            if (clients.Any())
            {
                await _chatCommunicator.SendUserStateChangeToClients(clients, friendIdentifier, FriendshipState.Blocked);
            }
        }

        /// <summary>
        /// CreateFriendshipRequest.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<FriendshipDto> CreateFriendshipRequest(CreateFriendshipRequestInput input)
        {
            var userIdentifier = AbpSession.ToUserIdentifier();
            var probableFriend = new UserIdentifier(input.TenantId, input.UserId);

            _chatFeatureChecker.CheckChatFeatures(userIdentifier.TenantId, probableFriend.TenantId);

            if (await _friendshipManager.GetFriendshipOrNullAsync(userIdentifier, probableFriend) != null)
            {
                throw new UserFriendlyException(L("YouAlreadySentAFriendshipRequestToThisUser"));
            }

            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());

            User probableFriendUser;
            using (CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                probableFriendUser = await UserManager.FindByIdAsync(input.UserId.ToString());
            }

            var friendTenancyName = probableFriend.TenantId.HasValue ? _tenantCache.Get(probableFriend.TenantId.Value).TenancyName : null;
            var sourceFriendship = new Friendship(userIdentifier, probableFriend, friendTenancyName, probableFriendUser.UserName, probableFriendUser.ProfilePictureId, FriendshipState.Accepted);
            await _friendshipManager.CreateFriendshipAsync(sourceFriendship);

            var userTenancyName = user.TenantId.HasValue ? _tenantCache.Get(user.TenantId.Value).TenancyName : null;
            var targetFriendship = new Friendship(probableFriend, userIdentifier, userTenancyName, user.UserName, user.ProfilePictureId, FriendshipState.Accepted);
            await _friendshipManager.CreateFriendshipAsync(targetFriendship);

            var clients = await _onlineClientManager.GetAllByUserIdAsync(probableFriend);
            if (clients.Any())
            {
                var isFriendOnline = await _onlineClientManager.IsOnlineAsync(sourceFriendship.ToUserIdentifier());
                await _chatCommunicator.SendFriendshipRequestToClient(clients, targetFriendship, false, isFriendOnline);
            }

            var senderClients = await _onlineClientManager.GetAllByUserIdAsync(userIdentifier);
            if (senderClients.Any())
            {
                var isFriendOnline = await _onlineClientManager.IsOnlineAsync(targetFriendship.ToUserIdentifier());
                await _chatCommunicator.SendFriendshipRequestToClient(senderClients, sourceFriendship, true, isFriendOnline);
            }

            var sourceFriendshipRequest = ObjectMapper.Map<FriendshipDto>(sourceFriendship);
            sourceFriendshipRequest.IsOnline = (await _onlineClientManager.GetAllByUserIdAsync(probableFriend)).Any();

            await _friendshipManager.AcceptFriendshipRequestAsync(userIdentifier, probableFriend);
            await _friendshipManager.AcceptFriendshipRequestAsync(probableFriend, userIdentifier);
            await _chatCommunicator.SendUserStateChangeToClients(clients, userIdentifier, FriendshipState.Accepted);
            await _chatCommunicator.SendUserStateChangeToClients(clients, probableFriend, FriendshipState.Accepted);

            return sourceFriendshipRequest;
        }

        /// <summary>
        /// CreateFriendshipRequestByUserName.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<FriendshipDto> CreateFriendshipRequestByUserName(CreateFriendshipRequestByUserNameInput input)
        {
            var probableFriend = await GetUserIdentifier(input.TenancyName, input.UserName);
            return await CreateFriendshipRequest(new CreateFriendshipRequestInput
            {
                TenantId = probableFriend.TenantId,
                UserId = probableFriend.UserId
            });
        }

        /// <summary>
        /// UnblockUser.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        public async Task UnblockUser(UnblockUserInput input)
        {
            var userIdentifier = AbpSession.ToUserIdentifier();
            var friendIdentifier = new UserIdentifier(input.TenantId, input.UserId);
            await _friendshipManager.AcceptFriendshipRequestAsync(userIdentifier, friendIdentifier);

            var clients = await _onlineClientManager.GetAllByUserIdAsync(userIdentifier);
            if (clients.Any())
            {
                await _chatCommunicator.SendUserStateChangeToClients(clients, friendIdentifier, FriendshipState.Accepted);
            }
        }

        private async Task<UserIdentifier> GetUserIdentifier(string tenancyName, string userName)
        {
            int? tenantId = null;
            if (!tenancyName.Equals("."))
            {
                using (CurrentUnitOfWork.SetTenantId(null))
                {
                    var tenant = await TenantManager.FindByTenancyNameAsync(tenancyName);
                    if (tenant == null)
                    {
                        throw new UserFriendlyException(L("ThereIsNoTenantDefinedWithName", tenancyName));
                    }

                    tenantId = tenant.Id;
                }
            }

            using (CurrentUnitOfWork.SetTenantId(tenantId))
            {
                var user = await UserManager.FindByNameOrEmailAsync(userName);
                if (user == null)
                {
                    throw new UserFriendlyException(L("ThereIsNoTenantDefinedWithName", tenancyName));
                }

                return user.ToUserIdentifier();
            }
        }
    }
}