using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Authorization.Users.Dto;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            _userLoginAttemptRepository.GetAllAsync().Returns(_ => Task.FromResult(_userLoginAttemptRepository.GetAll()));
            _sut = new UserLoginAppService(_userLoginAttemptRepository);
        }

        #region Construtor

        [Fact]
        public void Dado_Repository_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region GetRecentUserLoginAttempts

        [Fact]
        public async Task Dado_TentativasDeLoginRecentes_Quando_GetRecentUserLoginAttempts_Entao_DeveRetornarAs10MaisRecentes()
        {
            // Dado
            var userId = 1L;
            var attempts = new List<UserLoginAttempt>
            {
                new UserLoginAttempt { UserId = userId, CreationTime = DateTime.UtcNow.AddMinutes(-1) },
                new UserLoginAttempt { UserId = userId, CreationTime = DateTime.UtcNow.AddMinutes(-2) },
                new UserLoginAttempt { UserId = 2L, CreationTime = DateTime.UtcNow.AddMinutes(-1) }
            };

            _userLoginAttemptRepository.GetAll().Returns(attempts.AsAsyncQueryable());

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(userId);
            _sut.AbpSession = abpSession;

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<UserLoginAttemptDto>>(Arg.Any<object>()).Returns(new List<UserLoginAttemptDto> { new UserLoginAttemptDto { UserNameOrEmail = "admin" } });
            _sut.ObjectMapper = objectMapper;

            // Quando
            var result = await _sut.GetRecentUserLoginAttempts();

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
            result.Items[0].UserNameOrEmail.ShouldBe("admin");
        }

        #endregion
    }
}
