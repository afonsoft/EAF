using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Eaf.MiddlewareCore.SampleApp.Core;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Users
{
    public class UserManager_Tokens_Tests : EafMiddlewareTestBase
    {
        private readonly AbpUserManager<Role, User> _AbpUserManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<UserToken, long> _userTokenRepository;

        public UserManager_Tokens_Tests()
        {
            _AbpUserManager = Resolve<AbpUserManager<Role, User>>();
            _userTokenRepository = Resolve<IRepository<UserToken, long>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public async Task Should_Not_Valid_Expired_TokenValidityKey()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var user = await _AbpUserManager.GetUserByIdAsync(AbpSession.GetUserId());
                var tokenValidityKey = Guid.NewGuid().ToString();
                await _AbpUserManager.AddTokenValidityKeyAsync(user, tokenValidityKey, DateTime.UtcNow.AddDays(-1));
                var isTokenValidityKeyValid =
                    await _AbpUserManager.IsTokenValidityKeyValidAsync(user, tokenValidityKey);

                isTokenValidityKeyValid.ShouldBeFalse();
            }
        }

        [Fact]
        public async Task Should_Remove_Given_Name_TokenValidityKey()
        {
            var tokenValidityKey = Guid.NewGuid().ToString();

            using (_unitOfWorkManager.Begin())
            {
                var user = await _AbpUserManager.GetUserByIdAsync(AbpSession.GetUserId());

                await _AbpUserManager.AddTokenValidityKeyAsync(user, tokenValidityKey, DateTime.UtcNow.AddDays(1));
                await _unitOfWorkManager.Current.SaveChangesAsync();

                var allTokens = await _userTokenRepository.GetAllListAsync(t => t.UserId == user.Id);
                allTokens.Count.ShouldBe(1);
            }

            using (_unitOfWorkManager.Begin())
            {
                var user = await _AbpUserManager.GetUserByIdAsync(AbpSession.GetUserId());

                await _AbpUserManager.RemoveTokenValidityKeyAsync(user, tokenValidityKey);
                await _unitOfWorkManager.Current.SaveChangesAsync();

                var allTokens = await _userTokenRepository.GetAllListAsync(t => t.UserId == user.Id);
                allTokens.Count.ShouldBe(0);
            }
        }

        [Fact]
        public async Task Should_Valid_Non_Expired_TokenValidityKey()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var user = await _AbpUserManager.GetUserByIdAsync(AbpSession.GetUserId());
                var tokenValidityKey = Guid.NewGuid().ToString();
                await _AbpUserManager.AddTokenValidityKeyAsync(user, tokenValidityKey, DateTime.UtcNow.AddDays(1));
                var isTokenValidityKeyValid =
                    await _AbpUserManager.IsTokenValidityKeyValidAsync(user, tokenValidityKey);

                isTokenValidityKeyValid.ShouldBeTrue();
            }
        }
    }
}