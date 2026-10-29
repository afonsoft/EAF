using Abp;
using Abp.BackgroundJobs;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Notifications;
using Abp.Organizations;
using Abp.Timing;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.MassNotifications;
using Hangfire;
using Hangfire.Server;
using Hangfire.Storage;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.MassNotifications
{
    /// <summary>
    /// Testes BDD para MassNotificationJob seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class MassNotificationJobBddTests
    {
        private readonly IRepository<MassNotification, long> _massNotificationRepository;
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly MassNotificationJob _sut;

        public MassNotificationJobBddTests()
        {
            _massNotificationRepository = Substitute.For<IRepository<MassNotification, long>>();
            _notificationPublisher = Substitute.For<INotificationPublisher>();
            _backgroundJobManager = Substitute.For<IBackgroundJobManager>();
            _userManager = ManagerTestHelper.CreateUserManager();
            _roleManager = ManagerTestHelper.CreateRoleManager();

            _sut = new MassNotificationJob(
                _massNotificationRepository,
                _notificationPublisher,
                _backgroundJobManager,
                _userManager,
                _roleManager);
        }

        private static PerformContext CriarPerformContext()
        {
            return new PerformContext(
                null,
                Substitute.For<IStorageConnection>(),
                new BackgroundJob("job-id", null, DateTime.UtcNow),
                Substitute.For<IJobCancellationToken>());
        }

        private static MassNotification CriarNotificacao(MassNotificationStatus status = MassNotificationStatus.Pending)
        {
            return new MassNotification
            {
                Id = 10,
                TenantId = 1,
                Subject = "Assunto",
                Message = "Mensagem",
                Severity = NotificationSeverity.Info,
                Status = status,
            };
        }

        private async Task VerificarNaoPublicou()
        {
            await _notificationPublisher.DidNotReceive().PublishAsync(
                Arg.Any<string>(),
                Arg.Any<NotificationData>(),
                Arg.Any<EntityIdentifier>(),
                Arg.Any<NotificationSeverity>(),
                Arg.Any<UserIdentifier[]>(),
                Arg.Any<UserIdentifier[]>(),
                Arg.Any<int?[]>(),
                Arg.Any<Type[]>());
        }

        [Fact]
        public async Task Dado_NotificacaoInexistente_Quando_Executar_Entao_NaoDevePublicar()
        {
            // Dado
            _massNotificationRepository.GetAsync(999).Returns((MassNotification)null);
            var args = new MassNotificationJobArgs { MassNotificationId = 999 };

            // Quando
            await _sut.ExecuteAsync(args, CriarPerformContext(), CancellationToken.None);

            // Então
            await VerificarNaoPublicou();
        }

        [Fact]
        public async Task Dado_NotificacaoCancelada_Quando_Executar_Entao_NaoDevePublicar()
        {
            // Dado
            var notificacao = CriarNotificacao(MassNotificationStatus.Canceled);
            _massNotificationRepository.GetAsync(notificacao.Id).Returns(notificacao);
            var args = new MassNotificationJobArgs { MassNotificationId = notificacao.Id };

            // Quando
            await _sut.ExecuteAsync(args, CriarPerformContext(), CancellationToken.None);

            // Então
            await VerificarNaoPublicou();
        }

        [Fact]
        public async Task Dado_NotificacaoAgendadaParaFuturo_Quando_Executar_Entao_DeveReagendar()
        {
            // Dado
            var notificacao = CriarNotificacao();
            notificacao.ScheduledTime = Clock.Now.AddHours(2);
            _massNotificationRepository.GetAsync(notificacao.Id).Returns(notificacao);
            var args = new MassNotificationJobArgs { MassNotificationId = notificacao.Id };

            // Quando
            await _sut.ExecuteAsync(args, CriarPerformContext(), CancellationToken.None);

            // Então
            await _backgroundJobManager.Received(1).EnqueueAsync<MassNotificationJob, MassNotificationJobArgs>(
                args, Arg.Any<BackgroundJobPriority>(), Arg.Any<TimeSpan?>());
            await VerificarNaoPublicou();
        }

        [Fact]
        public async Task Dado_NotificacaoAgendadaNoPassado_Quando_Executar_Entao_DeveProcessarNormalmente()
        {
            // Dado
            var notificacao = CriarNotificacao();
            notificacao.ScheduledTime = Clock.Now.AddHours(-2);
            notificacao.TargetUserIds = "5";
            _massNotificationRepository.GetAsync(notificacao.Id).Returns(notificacao);
            var args = new MassNotificationJobArgs { MassNotificationId = notificacao.Id };

            // Quando
            await _sut.ExecuteAsync(args, CriarPerformContext(), CancellationToken.None);

            // Então
            await _notificationPublisher.Received(1).PublishAsync(
                "MassNotification",
                Arg.Is<MassNotificationData>(d => d.Subject == "Assunto" && d.Message == "Mensagem"),
                Arg.Any<EntityIdentifier>(),
                NotificationSeverity.Info,
                Arg.Is<UserIdentifier[]>(u => u.Length == 1 && u[0].UserId == 5 && u[0].TenantId == 1),
                Arg.Any<UserIdentifier[]>(),
                Arg.Any<int?[]>(),
                Arg.Any<Type[]>());
        }

        [Fact]
        public async Task Dado_UsuariosDiretos_Quando_Executar_Entao_DevePublicarParaIdsValidosEIgnorarInvalidos()
        {
            // Dado
            var notificacao = CriarNotificacao();
            notificacao.TargetUserIds = "5; 7, abc, ,9";
            _massNotificationRepository.GetAsync(notificacao.Id).Returns(notificacao);
            var args = new MassNotificationJobArgs { MassNotificationId = notificacao.Id };

            // Quando
            await _sut.ExecuteAsync(args, CriarPerformContext(), CancellationToken.None);

            // Então
            await _notificationPublisher.Received(1).PublishAsync(
                "MassNotification",
                Arg.Any<MassNotificationData>(),
                Arg.Any<EntityIdentifier>(),
                NotificationSeverity.Info,
                Arg.Is<UserIdentifier[]>(u => u.Length == 3),
                Arg.Any<UserIdentifier[]>(),
                Arg.Any<int?[]>(),
                Arg.Any<Type[]>());

            notificacao.Status.ShouldBe(MassNotificationStatus.Sent);
            await _massNotificationRepository.Received(1).UpdateAsync(notificacao);
        }

        [Fact]
        public async Task Dado_RolesComoAlvo_Quando_Executar_Entao_DevePublicarParaUsuariosDaRole()
        {
            // Dado
            var role = new Role(1, "Admin", "Admin") { Id = 3 };
            _roleManager.GetRoleByIdAsync(3).Returns(role);
            _userManager.GetUsersInRoleAsync("Admin").Returns(new List<User>
            {
                new User { Id = 21, TenantId = 1 },
                new User { Id = 22, TenantId = 1 },
            });

            var notificacao = CriarNotificacao();
            notificacao.TargetRoleIds = "3";
            _massNotificationRepository.GetAsync(notificacao.Id).Returns(notificacao);
            var args = new MassNotificationJobArgs { MassNotificationId = notificacao.Id };

            // Quando
            await _sut.ExecuteAsync(args, CriarPerformContext(), CancellationToken.None);

            // Então
            await _notificationPublisher.Received(1).PublishAsync(
                "MassNotification",
                Arg.Any<MassNotificationData>(),
                Arg.Any<EntityIdentifier>(),
                NotificationSeverity.Info,
                Arg.Is<UserIdentifier[]>(u => u.Length == 2),
                Arg.Any<UserIdentifier[]>(),
                Arg.Any<int?[]>(),
                Arg.Any<Type[]>());
        }

        [Fact]
        public async Task Dado_UnidadesOrganizacionaisComoAlvo_Quando_Executar_Entao_DevePublicarParaUsuariosDaOu()
        {
            // Dado
            _userManager.GetUsersInOrganizationUnitAsync(Arg.Any<OrganizationUnit>(), Arg.Any<bool>())
                .Returns(new List<User> { new User { Id = 31, TenantId = 1 } });

            var notificacao = CriarNotificacao();
            notificacao.TargetOrganizationUnitIds = "8";
            _massNotificationRepository.GetAsync(notificacao.Id).Returns(notificacao);
            var args = new MassNotificationJobArgs { MassNotificationId = notificacao.Id };

            // Quando
            await _sut.ExecuteAsync(args, CriarPerformContext(), CancellationToken.None);

            // Então
            await _notificationPublisher.Received(1).PublishAsync(
                "MassNotification",
                Arg.Any<MassNotificationData>(),
                Arg.Any<EntityIdentifier>(),
                NotificationSeverity.Info,
                Arg.Is<UserIdentifier[]>(u => u.Length == 1 && u[0].UserId == 31),
                Arg.Any<UserIdentifier[]>(),
                Arg.Any<int?[]>(),
                Arg.Any<Type[]>());
        }

        [Fact]
        public async Task Dado_EnviarParaTodosComTenant_Quando_Executar_Entao_DevePublicarComTenantIds()
        {
            // Dado
            var notificacao = CriarNotificacao();
            notificacao.SendToAllUsers = true;
            _massNotificationRepository.GetAsync(notificacao.Id).Returns(notificacao);
            var args = new MassNotificationJobArgs { MassNotificationId = notificacao.Id };

            // Quando
            await _sut.ExecuteAsync(args, CriarPerformContext(), CancellationToken.None);

            // Então
            await _notificationPublisher.Received(1).PublishAsync(
                "MassNotification",
                Arg.Any<MassNotificationData>(),
                Arg.Any<EntityIdentifier>(),
                NotificationSeverity.Info,
                Arg.Any<UserIdentifier[]>(),
                Arg.Any<UserIdentifier[]>(),
                Arg.Is<int?[]>(t => t.Length == 1 && t[0] == 1),
                Arg.Any<Type[]>());
        }

        [Fact]
        public async Task Dado_SemAlvos_Quando_Executar_Entao_NaoDevePublicarMasDeveMarcarEnviada()
        {
            // Dado
            var notificacao = CriarNotificacao();
            _massNotificationRepository.GetAsync(notificacao.Id).Returns(notificacao);
            var args = new MassNotificationJobArgs { MassNotificationId = notificacao.Id };

            // Quando
            await _sut.ExecuteAsync(args, CriarPerformContext(), CancellationToken.None);

            // Então
            await VerificarNaoPublicou();
            notificacao.Status.ShouldBe(MassNotificationStatus.Sent);
        }
    }
}
