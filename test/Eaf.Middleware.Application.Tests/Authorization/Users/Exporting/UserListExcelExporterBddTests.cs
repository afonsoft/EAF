using Abp;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Eaf.Middleware.Authorization.Users.Dto;
using Eaf.Middleware.Authorization.Users.Exporting;
using Eaf.Middleware.Storage;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users.Exporting
{
    public class UserListExcelExporterBddTests
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly UserListExcelExporter _sut;

        public UserListExcelExporterBddTests()
        {
            _timeZoneConverter = Substitute.For<ITimeZoneConverter>();
            _abpSession = Substitute.For<IAbpSession>();
            _tempFileCacheManager = Substitute.For<ITempFileCacheManager>();
            _sut = new UserListExcelExporter(_timeZoneConverter, _abpSession, _tempFileCacheManager);
        }

        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ListaDeUsuarios_Quando_Exportar_Entao_DeveRetornarArquivoExcelEPersistirNoCache()
        {
            // Dado
            _abpSession.TenantId.Returns(1);
            _abpSession.UserId.Returns(42);
            _timeZoneConverter
                .Convert(Arg.Any<DateTime?>(), Arg.Any<int?>(), Arg.Any<long>())
                .Returns(DateTime.UtcNow);

            var lastLoginTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var creationTime = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var users = new List<UserListDto>
            {
                new UserListDto
                {
                    Id = 1,
                    Name = "John",
                    Surname = "Doe",
                    UserName = "jdoe",
                    EmailAddress = "jdoe@example.com",
                    IsEmailConfirmed = true,
                    Roles = new List<UserListRoleDto>
                    {
                        new UserListRoleDto { RoleId = 1, RoleName = "Admin" }
                    },
                    LastLoginTime = lastLoginTime,
                    IsActive = true,
                    CreationTime = creationTime
                }
            };

            // Quando
            var result = _sut.ExportToFile(users);

            // Então
            result.ShouldNotBeNull();
            result.FileName.ShouldBe("UserList.xlsx");
            _tempFileCacheManager.Received(1).SetFile(result.FileToken, Arg.Is<byte[]>(b => b.Length > 0));
            _timeZoneConverter.Received(2).Convert(
                Arg.Any<DateTime?>(),
                Arg.Is<int?>(x => x == 1),
                Arg.Is<long>(x => x == 42)
            );
        }
    }
}
