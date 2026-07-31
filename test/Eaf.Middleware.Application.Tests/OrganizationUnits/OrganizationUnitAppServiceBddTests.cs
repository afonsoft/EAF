using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Organizations;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.OrganizationUnits;
using Eaf.Middleware.OrganizationUnits.Dto;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.OrganizationUnits
{
    /// <summary>
    /// Testes BDD para OrganizationUnitAppService.
    /// </summary>
    public class OrganizationUnitAppServiceBddTests
    {
        private readonly OrganizationUnitAppService _sut;
        private readonly IOrganizationUnitManager _organizationUnitManager;
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;

        public OrganizationUnitAppServiceBddTests()
        {
            _organizationUnitManager = Substitute.For<IOrganizationUnitManager>();
            _organizationUnitRepository = Substitute.For<IRepository<OrganizationUnit, long>>();
            _userManager = ManagerTestHelper.CreateUserManager();
            _roleManager = ManagerTestHelper.CreateRoleManager();

            _sut = new OrganizationUnitAppService(
                _organizationUnitManager,
                _organizationUnitRepository,
                _userManager,
                _roleManager);

            _sut.ObjectMapper = CreateObjectMapper();
            _sut.UnitOfWorkManager = ManagerTestHelper.CreateUnitOfWorkManager();
        }

        private static IObjectMapper CreateObjectMapper()
        {
            var mapper = Substitute.For<IObjectMapper>();
            mapper.Map<OrganizationUnitDto>(Arg.Any<OrganizationUnit>()).Returns(ci =>
            {
                var ou = (OrganizationUnit)ci[0];
                return new OrganizationUnitDto
                {
                    Id = ou.Id,
                    DisplayName = ou.DisplayName,
                    Code = ou.Code,
                    ParentId = ou.ParentId,
                };
            });
            mapper.Map<List<OrganizationUnitDto>>(Arg.Any<IEnumerable<OrganizationUnit>>()).Returns(ci =>
            {
                var ous = (IEnumerable<OrganizationUnit>)ci[0];
                return ous.Select(ou => mapper.Map<OrganizationUnitDto>(ou)).ToList();
            });
            mapper.Map<OrganizationUnitUserListDto>(Arg.Any<User>()).Returns(ci =>
            {
                var user = (User)ci[0];
                return new OrganizationUnitUserListDto
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Name = user.Name,
                    Surname = user.Surname,
                    EmailAddress = user.EmailAddress,
                };
            });
            mapper.Map<List<OrganizationUnitUserListDto>>(Arg.Any<IEnumerable<User>>()).Returns(ci =>
            {
                var users = (IEnumerable<User>)ci[0];
                return users.Select(u => mapper.Map<OrganizationUnitUserListDto>(u)).ToList();
            });
            mapper.Map<OrganizationUnitRoleListDto>(Arg.Any<Role>()).Returns(ci =>
            {
                var role = (Role)ci[0];
                return new OrganizationUnitRoleListDto
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    RoleDisplayName = role.DisplayName,
                };
            });
            mapper.Map<List<OrganizationUnitRoleListDto>>(Arg.Any<IEnumerable<Role>>()).Returns(ci =>
            {
                var roles = (IEnumerable<Role>)ci[0];
                return roles.Select(r => mapper.Map<OrganizationUnitRoleListDto>(r)).ToList();
            });
            return mapper;
        }

        [Fact]
        public async Task Dado_UnidadesOrganizacionaisCadastradas_Quando_GetOrganizationUnits_Entao_DeveRetornarEstruturaEmArvore()
        {
            // Dado
            var ous = new List<OrganizationUnit>
            {
                new OrganizationUnit { Id = 1, DisplayName = "Root", Code = "00001", ParentId = null },
                new OrganizationUnit { Id = 2, DisplayName = "Child", Code = "00001.00001", ParentId = 1 },
            };
            _organizationUnitRepository.GetAllListAsync().Returns(ous);

            // Quando
            var result = await _sut.GetOrganizationUnits();

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
            result.Items[0].DisplayName.ShouldBe("Root");
            result.Items[0].Children.Count.ShouldBe(1);
            result.Items[0].Children[0].DisplayName.ShouldBe("Child");
        }

        [Fact]
        public async Task Dado_InputValido_Quando_CreateAsync_Entao_DeveChamarManagerCreate()
        {
            // Dado
            var input = new CreateOrganizationUnitInput { DisplayName = "Sales", ParentId = null };
            OrganizationUnit created = null;
            await _organizationUnitManager.CreateAsync(Arg.Do<OrganizationUnit>(ou => created = ou));

            // Quando
            var result = await _sut.CreateAsync(input);

            // Então
            created.ShouldNotBeNull();
            created.DisplayName.ShouldBe("Sales");
            result.DisplayName.ShouldBe("Sales");
        }

        [Fact]
        public async Task Dado_InputValido_Quando_UpdateAsync_Entao_DeveAtualizarDisplayName()
        {
            // Dado
            var ou = new OrganizationUnit { Id = 1, DisplayName = "Old" };
            _organizationUnitRepository.GetAsync(1).Returns(ou);
            var input = new UpdateOrganizationUnitInput { Id = 1, DisplayName = "New" };

            // Quando
            await _sut.UpdateAsync(input);

            // Então
            ou.DisplayName.ShouldBe("New");
            await _organizationUnitManager.Received(1).UpdateAsync(ou);
        }

        [Fact]
        public async Task Dado_IdValido_Quando_DeleteAsync_Entao_DeveChamarManagerDelete()
        {
            // Quando
            await _sut.DeleteAsync(new EntityDto<long>(1));

            // Então
            await _organizationUnitManager.Received(1).DeleteAsync(1);
        }
    }
}
