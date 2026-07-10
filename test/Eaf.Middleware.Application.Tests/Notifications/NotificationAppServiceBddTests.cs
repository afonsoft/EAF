using Abp;
using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Localization;
using Abp.Notifications;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Eaf.Middleware.Notifications;
using Eaf.Middleware.Notifications.Dto;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Notifications
{
    /// <summary>
    /// Testes BDD para NotificationAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class NotificationAppServiceBddTests
    {
        private readonly INotificationDefinitionManager _notificationDefinitionManager;
        private readonly IUserNotificationManager _userNotificationManager;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly NotificationAppService _sut;

        public NotificationAppServiceBddTests()
        {
            _notificationDefinitionManager = Substitute.For<INotificationDefinitionManager>();
            _userNotificationManager = Substitute.For<IUserNotificationManager>();
            _notificationSubscriptionManager = Substitute.For<INotificationSubscriptionManager>();

            _sut = new NotificationAppService(
                _notificationDefinitionManager,
                _userNotificationManager,
                _notificationSubscriptionManager
            );
        }

        #region Construtor

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region SetAllNotificationsAsRead

        [Fact]
        public async Task Dado_UsuarioLogado_Quando_SetAllNotificationsAsRead_Entao_DeveAtualizarTodas()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 42);
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            // Quando
            await _sut.SetAllNotificationsAsRead();

            // Então
            await _userNotificationManager.Received(1)
                .UpdateAllUserNotificationStatesAsync(
                    Arg.Is<UserIdentifier>(u => u.UserId == 42),
                    UserNotificationState.Read
                );
        }

        #endregion

        #region SetNotificationAsRead

        [Fact]
        public async Task Dado_NotificacaoDoUsuario_Quando_SetNotificationAsRead_Entao_DeveMarcarComoLida()
        {
            // Dado
            var notificationId = Guid.NewGuid();
            var userIdentifier = new UserIdentifier(1, 42);
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            var userNotification = new UserNotification
            {
                UserId = 42,
                TenantId = 1
            };
            _userNotificationManager.GetUserNotificationAsync(1, notificationId)
                .Returns(userNotification);

            // Quando
            await _sut.SetNotificationAsRead(new EntityDto<Guid>(notificationId));

            // Então
            await _userNotificationManager.Received(1)
                .UpdateUserNotificationStateAsync(1, notificationId, UserNotificationState.Read);
        }

        [Fact]
        public async Task Dado_NotificacaoDeOutroUsuario_Quando_SetNotificationAsRead_Entao_DeveLancarExcecao()
        {
            // Dado
            var notificationId = Guid.NewGuid();
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            var userNotification = new UserNotification
            {
                UserId = 99,
                TenantId = 1
            };
            _userNotificationManager.GetUserNotificationAsync(1, notificationId)
                .Returns(userNotification);

            // Quando / Então
            await Should.ThrowAsync<AbpException>(() =>
                _sut.SetNotificationAsRead(new EntityDto<Guid>(notificationId)));
        }

        #endregion

        #region DeleteNotification

        [Fact]
        public async Task Dado_NotificacaoDoUsuario_Quando_DeleteNotification_Entao_DeveDeletar()
        {
            // Dado
            var notificationId = Guid.NewGuid();
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            var userNotification = new UserNotification
            {
                UserId = 42,
                TenantId = 1
            };
            _userNotificationManager.GetUserNotificationAsync(1, notificationId)
                .Returns(userNotification);

            // Quando
            await _sut.DeleteNotification(new EntityDto<Guid>(notificationId));

            // Então
            await _userNotificationManager.Received(1)
                .DeleteUserNotificationAsync(1, notificationId);
        }

        #endregion

        #region GetUserNotifications

        [Fact]
        public async Task Dado_UsuarioLogado_Quando_GetUserNotifications_Entao_DeveRetornarNotificacoes()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 42);
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            _userNotificationManager.GetUserNotificationCountAsync(Arg.Any<UserIdentifier>(), null)
                .Returns(5);
            _userNotificationManager.GetUserNotificationCountAsync(Arg.Any<UserIdentifier>(), UserNotificationState.Unread)
                .Returns(2);
            _userNotificationManager.GetUserNotificationsAsync(Arg.Any<UserIdentifier>(), null, 0, 10)
                .Returns(new List<UserNotification>());

            var input = new GetUserNotificationsInput
            {
                MaxResultCount = 10,
                SkipCount = 0
            };

            // Quando
            var result = await _sut.GetUserNotifications(input);

            // Então
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(5);
            result.UnreadCount.ShouldBe(2);
        }

        #endregion

        #region UpdateNotificationSettings

        [Fact]
        public async Task Dado_SettingsComSubscricoes_Quando_UpdateNotificationSettings_Entao_DeveAtualizarSubscricoes()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 42);
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            var settingManager = Substitute.For<Abp.Configuration.ISettingManager>();
            _sut.SettingManager = settingManager;

            var input = new UpdateNotificationSettingsInput
            {
                ReceiveNotifications = true,
                Notifications = new List<NotificationSubscriptionDto>
                {
                    new NotificationSubscriptionDto { Name = "Notification1", IsSubscribed = true },
                    new NotificationSubscriptionDto { Name = "Notification2", IsSubscribed = false }
                }
            };

            // Quando
            await _sut.UpdateNotificationSettings(input);

            // Então
            await _notificationSubscriptionManager.Received(1)
                .SubscribeAsync(Arg.Any<UserIdentifier>(), "Notification1");
            await _notificationSubscriptionManager.Received(1)
                .UnsubscribeAsync(Arg.Any<UserIdentifier>(), "Notification2");
        }

        #endregion

        #region GetNotificationSettings

        [Fact]
        public async Task Dado_NotificacoesDisponiveis_Quando_GetNotificationSettings_Entao_DeveRetornarConfiguracoesComAssinaturas()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueAsync(NotificationSettingNames.ReceiveNotifications).Returns("true");
            _sut.SettingManager = settingManager;

            var definitions = new List<NotificationDefinition>
            {
                new NotificationDefinition("Test", null, new FixedLocalizableString("Test"), null, null, null)
            };
            _notificationDefinitionManager.GetAllAvailableAsync(Arg.Any<UserIdentifier>())
                .Returns(definitions);

            var subscriptions = new List<NotificationSubscription>
            {
                new NotificationSubscription { NotificationName = "Test" }
            };
            _notificationSubscriptionManager.GetSubscribedNotificationsAsync(Arg.Any<UserIdentifier>())
                .Returns(subscriptions);

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<NotificationSubscriptionWithDisplayNameDto>>(Arg.Any<object>())
                .Returns(ci =>
                {
                    var source = (IEnumerable<NotificationDefinition>)ci.Arg<object>();
                    return source.Select(d => new NotificationSubscriptionWithDisplayNameDto { Name = d.Name, DisplayName = d.DisplayName?.ToString() ?? d.Name }).ToList();
                });
            _sut.ObjectMapper = objectMapper;

            // Quando
            var result = await _sut.GetNotificationSettings();

            // Então
            result.ShouldNotBeNull();
            result.ReceiveNotifications.ShouldBeTrue();
            result.Notifications.Count.ShouldBe(1);
            result.Notifications[0].Name.ShouldBe("Test");
            result.Notifications[0].IsSubscribed.ShouldBeTrue();
        }

        #endregion
    }
}
