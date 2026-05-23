using Abp.AspNetCore.SignalR.Hubs;
using Abp.Localization;
using Abp.RealTime;
using Abp.UI;
using Abp;
using Castle.Core.Logging;
using Castle.Windsor;

using Eaf.Middleware.Chat;

using Abp.Runtime.Session;

using System;

using System.Threading.Tasks;

namespace Eaf.AspNetCore.SignalR.Chat
{
    /// <summary>
    /// Representa a classe ChatHub.
    /// </summary>
    public class ChatHub : OnlineClientHubBase
    {
        private readonly IChatMessageManager _chatMessageManager;
        private readonly ILocalizationManager _localizationManager;
        private readonly IWindsorContainer _windsorContainer;
        private bool _isCallByRelease;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatHub"/> class.
        /// </summary>
        public ChatHub(
            IChatMessageManager chatMessageManager,
            ILocalizationManager localizationManager,
            IWindsorContainer windsorContainer,
            IOnlineClientManager<ChatChannel> onlineClientManager,
            IOnlineClientInfoProvider clientInfoProvider) : base(onlineClientManager, clientInfoProvider)
        {
            _chatMessageManager = chatMessageManager;
            _localizationManager = localizationManager;
            _windsorContainer = windsorContainer;

            Logger = NullLogger.Instance;
            ChatAbpSession = NullAbpSession.Instance;
        }

        private IAbpSession ChatAbpSession { get; }

        /// <summary>
        /// Register.
        /// </summary>
        public void Register()
        {
            Logger.DebugFormat("A client is registered: {0}", Context.ConnectionId);
        }

        /// <summary>
        /// DeleteMessage.
        /// </summary>
        /// <param name="id">Parâmetro id.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<string> DeleteMessage(int id)
        {
            var sender = Context.ToUserIdentifier();
            try
            {
                var message = await _chatMessageManager.FindMessageAsync(id, sender.UserId);

                if (message?.SharedMessageId != null)
                {
                    Logger.DebugFormat("Delete a chat message {0} to user: {1}", id, sender);
                    _chatMessageManager.Delete(message.SharedMessageId.Value);
                    return String.Empty;
                }
                Logger.InfoFormat("Could not delete chat message {0} to user: {1}", id, sender);
                return String.Format("Could not find chat message {0} to user: {1}", id, sender);
            }
            catch (AbpException ex)
            {
                Logger.WarnFormat("Could not delete chat message {0} to user: {1}", id, sender);
                Logger.Error(ex.ToString(), ex);
                return ex.Message;
            }
            catch (Exception ex)
            {
                Logger.WarnFormat("Could not delete chat message {0} to user: {1}", id, sender);
                Logger.Error(ex.ToString(), ex);
                return _localizationManager.GetSource("Eaf").GetString("InternalServerError");
            }

            return _localizationManager.GetSource("Eaf").GetString("InternalServerError");
        }

        /// <summary>
        /// SendMessage.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<string> SendMessage(SendChatMessageInput input)
        {
            var sender = Context.ToUserIdentifier();

            if (input.UserId.HasValue && input.UserId.Value > 0)
            {
                var receiver = new UserIdentifier(input.TenantId, input.UserId.Value);
                try
                {
                    using (ChatAbpSession.Use(Context.GetTenantId(), Context.GetUserId()))
                    {
                        await _chatMessageManager.SendMessageAsync(sender, receiver, input.Message, input.TenancyName, input.UserName, input.ProfilePictureId);
                        return string.Empty;
                    }
                }
                catch (UserFriendlyException ex)
                {
                    Logger.WarnFormat("Could not send chat message to user: {0}", receiver);
                    Logger.Warn(ex.ToString(), ex);
                    return ex.Message;
                }
                catch (Exception ex)
                {
                    Logger.WarnFormat("Could not send chat message to user: {0}", receiver);
                    Logger.Warn(ex.ToString(), ex);
                    return _localizationManager.GetSource("Eaf").GetString("InternalServerError");
                }
            }
            else if (input.GroupId.HasValue && input.GroupId.Value > 0)
            {
                var receiver = new UserIdentifier(input.TenantId, input.GroupId.Value);
                try
                {
                    using (ChatAbpSession.Use(Context.GetTenantId(), Context.GetUserId()))
                    {
                        await _chatMessageManager.SendMessageToGroupAsync(sender, receiver, input.Message);
                        return string.Empty;
                    }
                }
                catch (UserFriendlyException ex)
                {
                    Logger.WarnFormat("Could not send chat message to group: {0}", input.GroupId);
                    Logger.Warn(ex.ToString(), ex);
                    return ex.Message;
                }
                catch (Exception ex)
                {
                    Logger.WarnFormat("Could not send chat message to group: {0}", receiver);
                    Logger.Warn(ex.ToString(), ex);
                    return _localizationManager.GetSource("Eaf").GetString("InternalServerError");
                }
            }

            return _localizationManager.GetSource("Eaf").GetString("InternalServerError");
        }

        protected override void Dispose(bool disposing)
        {
            if (_isCallByRelease)
            {
                return;
            }
            base.Dispose(disposing);
            if (disposing)
            {
                _isCallByRelease = true;
                _windsorContainer.Release(this);
            }
        }
    }
}