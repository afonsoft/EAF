using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Eaf.Middleware.Authorization.Users;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Login
{
    /// <summary>
    /// Testes BDD para UserLoginAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class UserLoginAppServiceBddTests
    {
        private readonly IRepository<UserLoginAttempt, long> _userLoginAttemptRepository;
        private readonly UserLoginAppService _sut;

        public UserLoginAppServiceBddTests()
        {
            _userLoginAttemptRepository = Substitute.For<IRepository<UserLoginAttempt, long>>();
            _sut = new UserLoginAppService(_userLoginAttemptRepository);
        }

        #region Construtor

        [Fact]
        public void Dado_Repository_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion
    }
}
