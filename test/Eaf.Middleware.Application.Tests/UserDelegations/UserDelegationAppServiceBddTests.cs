using Abp;
using Abp.Application.Services.Dto;
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
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Xunit;

namespace Eaf.Middleware.Application.Tests.UserDelegations
{
    /// <summary>
    /// Testes BDD para UserDelegationAppService seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class UserDelegationAppServiceBddTests
    {
        private readonly IRepository<UserDelegation, long> _repository;
        private readonly IUserDelegationManager _manager;
        private readonly UserManager _userManager;
        private readonly UserDelegationAppService _sut;

        public UserDelegationAppServiceBddTests()
        {
            _repository = Substitute.For<IRepository<UserDelegation, long>>();
            _manager = Substitute.For<IUserDelegationManager>();
            _userManager = ManagerTestHelper.CreateUserManager();

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns((long?)10);
            abpSession.TenantId.Returns((int?)1);

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<UserDelegationDto>(Arg.Any<UserDelegation>())
                .Returns(ci =>
                {
                    var d = (UserDelegation)ci[0];
                    return new UserDelegationDto
                    {
                        Id = d.Id,
                        SourceUserId = d.SourceUserId,
                        TargetUserId = d.TargetUserId,
                        StartTime = d.StartTime,
                        EndTime = d.EndTime,
                    };
                });
            objectMapper.Map<List<UserDelegationDto>>(Arg.Any<List<UserDelegation>>())
                .Returns(ci =>
                {
                    var list = (List<UserDelegation>)ci[0];
                    var dtos = new List<UserDelegationDto>();
                    foreach (var d in list)
                    {
                        dtos.Add(new UserDelegationDto
                        {
                            Id = d.Id,
                            SourceUserId = d.SourceUserId,
                            TargetUserId = d.TargetUserId,
                            StartTime = d.StartTime,
                            EndTime = d.EndTime,
                        });
                    }
                    return dtos;
                });

            _sut = new UserDelegationAppService(_repository, _manager, _userManager)
            {
                AbpSession = abpSession,
                ObjectMapper = objectMapper,
            };
        }

        [Fact]
        public async Task Dado_InputValido_Quando_Criar_Entao_DeveInserirDelegacaoDoUsuarioLogado()
        {
            // Dado
            var input = new CreateUserDelegationInput
            {
                TargetUserId = 20,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(1),
                Description = "cobertura",
            };

            // Quando
            var resultado = await _sut.CreateAsync(input);

            // Então
            _manager.Received(1).ValidateDates(input.StartTime, input.EndTime);
            await _repository.Received(1).InsertAsync(Arg.Is<UserDelegation>(d =>
                d.SourceUserId == 10 && d.TargetUserId == 20 && d.TenantId == 1));
            resultado.TargetUserId.ShouldBe(20);
        }

        [Fact]
        public async Task Dado_DelegacaoPropria_Quando_Cancelar_Entao_DeveRemover()
        {
            // Dado
            var delegacao = new UserDelegation { Id = 5, SourceUserId = 10 };
            _repository.GetAsync(5).Returns(delegacao);

            // Quando
            await _sut.CancelAsync(new EntityDto<long>(5));

            // Então
            await _repository.Received(1).DeleteAsync(delegacao);
        }

        [Fact]
        public async Task Dado_DelegacaoDeOutroUsuario_Quando_Cancelar_Entao_DeveLancarExcecao()
        {
            // Dado
            var delegacao = new UserDelegation { Id = 6, SourceUserId = 99 };
            _repository.GetAsync(6).Returns(delegacao);

            // Quando / Então
            await Should.ThrowAsync<AbpException>(() => _sut.CancelAsync(new EntityDto<long>(6)));
            await _repository.DidNotReceive().DeleteAsync(Arg.Any<UserDelegation>());
        }

        [Fact]
        public async Task Dado_DelegacoesDoUsuario_Quando_ConsultarMinhas_Entao_DeveRetornarLista()
        {
            // Dado
            var delegacoes = new List<UserDelegation>
            {
                new UserDelegation { Id = 1, SourceUserId = 10, TargetUserId = 20 },
                new UserDelegation { Id = 2, SourceUserId = 30, TargetUserId = 40 },
            };
            _repository.GetAllAsync().Returns(delegacoes.AsAsyncQueryable());
            _userManager.GetUserByIdAsync(Arg.Any<long>()).Returns(ci =>
            {
                var u = new User { Id = (long)ci[0], UserName = "user" + ci[0] };
                return u;
            });

            // Quando
            var resultado = await _sut.GetMyDelegationsAsync(new GetUserDelegationsInput());

            // Então
            resultado.Items.ShouldHaveSingleItem();
        }
    }
}
