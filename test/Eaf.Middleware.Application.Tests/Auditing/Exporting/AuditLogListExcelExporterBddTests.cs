using Abp;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Eaf.Middleware.Auditing.Dto;
using Eaf.Middleware.Auditing.Exporting;
using Eaf.Middleware.Storage;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Auditing.Exporting
{
    public class AuditLogListExcelExporterBddTests
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly AuditLogListExcelExporter _sut;

        public AuditLogListExcelExporterBddTests()
        {
            _timeZoneConverter = Substitute.For<ITimeZoneConverter>();
            _abpSession = Substitute.For<IAbpSession>();
            _tempFileCacheManager = Substitute.For<ITempFileCacheManager>();
            _sut = new AuditLogListExcelExporter(_timeZoneConverter, _abpSession, _tempFileCacheManager);
        }

        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ListaDeAuditLogs_Quando_ExportarAuditLogs_Entao_DeveRetornarArquivoExcelEPersistirNoCache()
        {
            // Dado
            _abpSession.TenantId.Returns(1);
            _abpSession.UserId.Returns(42);
            _timeZoneConverter
                .Convert(Arg.Any<DateTime?>(), Arg.Any<int?>(), Arg.Any<long>())
                .Returns(DateTime.UtcNow);

            var executionTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var auditLogs = new List<AuditLogListDto>
            {
                new AuditLogListDto
                {
                    Id = 1,
                    UserName = "john",
                    ServiceName = "MyApp.Service",
                    MethodName = "MyMethod",
                    Parameters = "{}",
                    ExecutionDuration = 100,
                    ExecutionTime = executionTime,
                    ClientIpAddress = "127.0.0.1",
                    ClientName = "Client",
                    BrowserInfo = "Chrome",
                    Exception = string.Empty
                }
            };

            // Quando
            var result = _sut.ExportToFile(auditLogs);

            // Então
            result.ShouldNotBeNull();
            result.FileName.ShouldBe("AuditLogs.xlsx");
            _tempFileCacheManager.Received(1).SetFile(result.FileToken, Arg.Is<byte[]>(b => b.Length > 0));
            _timeZoneConverter.Received(1).Convert(
                Arg.Is<DateTime?>(x => x == executionTime),
                Arg.Is<int?>(x => x == 1),
                Arg.Is<long>(x => x == 42)
            );
        }

        [Fact]
        public void Dado_ListaDeEntityChanges_Quando_ExportarEntityChanges_Entao_DeveRetornarArquivoExcelEPersistirNoCache()
        {
            // Dado
            _abpSession.TenantId.Returns(1);
            _abpSession.UserId.Returns(42);
            _timeZoneConverter
                .Convert(Arg.Any<DateTime?>(), Arg.Any<int?>(), Arg.Any<long>())
                .Returns(DateTime.UtcNow);

            var changeTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var entityChanges = new List<EntityChangeListDto>
            {
                new EntityChangeListDto
                {
                    Id = 1,
                    ChangeTime = changeTime,
                    ChangeType = global::Abp.Events.Bus.Entities.EntityChangeType.Created,
                    EntityTypeFullName = "MyNamespace.MyEntity",
                    UserName = "john"
                }
            };

            // Quando
            var result = _sut.ExportToFile(entityChanges);

            // Então
            result.ShouldNotBeNull();
            result.FileName.ShouldBe("DetailedLogs.xlsx");
            _tempFileCacheManager.Received(1).SetFile(result.FileToken, Arg.Is<byte[]>(b => b.Length > 0));
            _timeZoneConverter.Received(1).Convert(
                Arg.Is<DateTime?>(x => x == changeTime),
                Arg.Is<int?>(x => x == 1),
                Arg.Is<long>(x => x == 42)
            );
        }
    }
}
