using Abp.Domain.Repositories;
using Abp.Timing;
using Eaf.Middleware.UserDelegations;
using NSubstitute;
using Shouldly;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.UserDelegations
{
    /// <summary>
    /// Testes BDD para UserDelegationManager seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class UserDelegationManagerBddTests
    {
        private readonly IRepository<UserDelegation, long> _repository;
        private readonly UserDelegationManager _sut;

        public UserDelegationManagerBddTests()
        {
            _repository = Substitute.For<IRepository<UserDelegation, long>>();
            _sut = new UserDelegationManager(_repository);
        }

        [Fact]
        public void Dado_DatasInvertidas_Quando_Validar_Entao_DeveLancarArgumentException()
        {
            // Dado
            var inicio = Clock.Now.AddDays(5);
            var fim = Clock.Now.AddDays(1);

            // Quando / Então
            var ex = Should.Throw<ArgumentException>(() => _sut.ValidateDates(inicio, fim));
            ex.Message.ShouldContain("End time must be greater than start time");
        }

        [Fact]
        public void Dado_InicioNoPassado_Quando_Validar_Entao_DeveLancarArgumentException()
        {
            // Dado
            var inicio = Clock.Now.Date.AddDays(-2);
            var fim = Clock.Now.AddDays(2);

            // Quando / Então
            var ex = Should.Throw<ArgumentException>(() => _sut.ValidateDates(inicio, fim));
            ex.Message.ShouldContain("Start time cannot be in the past");
        }

        [Fact]
        public void Dado_DatasValidas_Quando_Validar_Entao_NaoDeveLancar()
        {
            // Dado
            var inicio = Clock.Now.Date;
            var fim = Clock.Now.AddDays(2);

            // Quando / Então
            Should.NotThrow(() => _sut.ValidateDates(inicio, fim));
        }

        [Fact]
        public async Task Dado_DelegacaoAtiva_Quando_Consultar_Entao_DeveRetornarDelegacao()
        {
            // Dado
            var delegacao = new UserDelegation { Id = 7, SourceUserId = 1, TargetUserId = 2 };
            _repository.FirstOrDefaultAsync(Arg.Any<Expression<Func<UserDelegation, bool>>>())
                .Returns(delegacao);

            // Quando
            var encontrada = await _sut.GetActiveDelegationAsync(1, 2);
            var valida = await _sut.IsDelegationValidAsync(1, 2);

            // Então
            encontrada.ShouldBeSameAs(delegacao);
            valida.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_SemDelegacaoAtiva_Quando_Consultar_Entao_DeveRetornarInvalida()
        {
            // Dado
            _repository.FirstOrDefaultAsync(Arg.Any<Expression<Func<UserDelegation, bool>>>())
                .Returns((UserDelegation)null);

            // Quando
            var encontrada = await _sut.GetActiveDelegationAsync(1, 2);
            var valida = await _sut.IsDelegationValidAsync(1, 2);

            // Então
            encontrada.ShouldBeNull();
            valida.ShouldBeFalse();
        }
    }
}
