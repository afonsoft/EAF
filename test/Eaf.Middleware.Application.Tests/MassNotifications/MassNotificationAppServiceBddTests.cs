using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.MassNotifications;
using Eaf.Middleware.MassNotifications.Dto;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.MassNotifications
{
    /// <summary>
    /// Testes BDD para MassNotificationAppService.
    /// </summary>
    public class MassNotificationAppServiceBddTests
    {
        private readonly MassNotificationAppService _sut;
        private readonly IRepository<MassNotification, long> _massNotificationRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public MassNotificationAppServiceBddTests()
        {
            _massNotificationRepository = Substitute.For<IRepository<MassNotification, long>>();
            _backgroundJobManager = Substitute.For<IBackgroundJobManager>();

            _sut = new MassNotificationAppService(_massNotificationRepository, _backgroundJobManager);
            _sut.ObjectMapper = CreateObjectMapper();
            _sut.UnitOfWorkManager = ManagerTestHelper.CreateUnitOfWorkManager();
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;
        }

        private static IObjectMapper CreateObjectMapper()
        {
            var mapper = Substitute.For<IObjectMapper>();
            mapper.Map<MassNotificationDto>(Arg.Any<MassNotification>()).Returns(ci =>
            {
                var n = (MassNotification)ci[0];
                return new MassNotificationDto
                {
                    Id = n.Id,
                    Subject = n.Subject,
                    Message = n.Message,
                    Status = n.Status.ToString(),
                    TenantId = n.TenantId,
                    SendToAllUsers = n.SendToAllUsers,
                };
            });
            mapper.Map<List<MassNotificationDto>>(Arg.Any<IEnumerable<MassNotification>>()).Returns(ci =>
            {
                var notifications = (IEnumerable<MassNotification>)ci[0];
                return notifications.Select(n => mapper.Map<MassNotificationDto>(n)).ToList();
            });
            mapper.Map<MassNotification>(Arg.Any<CreateMassNotificationInput>()).Returns(ci =>
            {
                var input = (CreateMassNotificationInput)ci[0];
                return new MassNotification
                {
                    Subject = input.Subject,
                    Message = input.Message,
                    Severity = input.Severity,
                    TargetUserIds = input.TargetUserIds,
                    TargetRoleIds = input.TargetRoleIds,
                    TargetOrganizationUnitIds = input.TargetOrganizationUnitIds,
                    SendToAllUsers = input.SendToAllUsers,
                    ScheduledTime = input.ScheduledTime,
                };
            });
            return mapper;
        }

        [Fact]
        public async Task Dado_InputValido_Quando_CreateAsync_Entao_DeveInserirENotificacaoFicaPendente()
        {
            // Dado
            var input = new CreateMassNotificationInput
            {
                Subject = "Maintenance",
                Message = "System will be down.",
                SendToAllUsers = true,
            };

            MassNotification inserted = null;
            await _massNotificationRepository.InsertAsync(Arg.Do<MassNotification>(n => inserted = n));

            // Quando
            var result = await _sut.CreateAsync(input);

            // Então
            inserted.ShouldNotBeNull();
            inserted.Status.ShouldBe(MassNotificationStatus.Pending);
            result.Subject.ShouldBe("Maintenance");
            await _backgroundJobManager.Received(1).EnqueueAsync<MassNotificationJob, MassNotificationJobArgs>(
                Arg.Any<MassNotificationJobArgs>(),
                Arg.Any<Abp.BackgroundJobs.BackgroundJobPriority>(),
                Arg.Any<TimeSpan?>());
        }

        [Fact]
        public async Task Dado_NotificacaoPendente_Quando_CancelAsync_Entao_DeveAtualizarStatusParaCancelado()
        {
            // Dado
            var notification = new MassNotification { Id = 1, Subject = "Alert", Status = MassNotificationStatus.Pending };
            _massNotificationRepository.GetAsync(1).Returns(notification);

            // Quando
            await _sut.CancelAsync(new Abp.Application.Services.Dto.EntityDto<long>(1));

            // Então
            notification.Status.ShouldBe(MassNotificationStatus.Canceled);
            await _massNotificationRepository.Received(1).UpdateAsync(notification);
        }
    }
}
