using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.UserDelegations;
using Eaf.Middleware.UserDelegations.Dto;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.UserDelegations
{
    /// <summary>
    /// Testes BDD para UserDelegationAppService.
    /// </summary>
    public class UserDelegationAppServiceBddTests
    {
        private readonly UserDelegationAppService _sut;
        private readonly IRepository<UserDelegation, long> _userDelegationRepository;
        private readonly IUserDelegationManager _userDelegationManager;
        private readonly UserManager _userManager;

        public UserDelegationAppServiceBddTests()
        {
            _userDelegationRepository = Substitute.For<IRepository<UserDelegation, long>>();
            _userDelegationManager = Substitute.For<IUserDelegationManager>();
            _userManager = ManagerTestHelper.CreateUserManager();

            _sut = new UserDelegationAppService(_userDelegationRepository, _userDelegationManager, _userManager);
            _sut.ObjectMapper = CreateObjectMapper();
            _sut.UnitOfWorkManager = ManagerTestHelper.CreateUnitOfWorkManager();
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns((long?)1);
            _sut.AbpSession = abpSession;
        }

        private static IObjectMapper CreateObjectMapper()
        {
            var mapper = Substitute.For<IObjectMapper>();
            mapper.Map<UserDelegationDto>(Arg.Any<UserDelegation>()).Returns(ci =>
            {
                var d = (UserDelegation)ci[0];
                return new UserDelegationDto
                {
                    Id = d.Id,
                    SourceUserId = d.SourceUserId,
                    TargetUserId = d.TargetUserId,
                    StartTime = d.StartTime,
                    EndTime = d.EndTime,
                    Description = d.Description,
                    IsActive = d.IsActive(DateTime.Now),
                };
            });
            mapper.Map<List<UserDelegationDto>>(Arg.Any<IEnumerable<UserDelegation>>()).Returns(ci =>
            {
                var delegations = (IEnumerable<UserDelegation>)ci[0];
                return delegations.Select(d => mapper.Map<UserDelegationDto>(d)).ToList();
            });
            return mapper;
        }

        [Fact]
        public async Task Dado_InputValido_Quando_CreateAsync_Entao_DeveInserirDelegacao()
        {
            // Dado
            var start = DateTime.Now.AddDays(1);
            var end = DateTime.Now.AddDays(7);
            var input = new CreateUserDelegationInput
            {
                TargetUserId = 2,
                StartTime = start,
                EndTime = end,
                Description = "Vacation",
            };

            UserDelegation inserted = null;
            await _userDelegationRepository.InsertAsync(Arg.Do<UserDelegation>(d => inserted = d));

            // Quando
            var result = await _sut.CreateAsync(input);

            // Então
            inserted.ShouldNotBeNull();
            inserted.TargetUserId.ShouldBe(2);
            inserted.StartTime.ShouldBe(start);
            inserted.EndTime.ShouldBe(end);
            result.Description.ShouldBe("Vacation");
        }

        [Fact]
        public async Task Dado_DelegacaoExistente_Quando_CancelAsync_Entao_DeveRemoverDelegacao()
        {
            // Dado
            var delegation = new UserDelegation
            {
                Id = 1,
                SourceUserId = 1,
                TargetUserId = 2,
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.Now.AddDays(7),
            };
            _userDelegationRepository.GetAsync(1).Returns(delegation);

            // Quando
            await _sut.CancelAsync(new EntityDto<long>(1));

            // Então
            await _userDelegationRepository.Received(1).DeleteAsync(delegation);
        }
    }
}
