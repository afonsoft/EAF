using Abp.Authorization.Users;
using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

using Abp.Runtime.Session;
using Abp.Threading.Timers;
using Eaf.MiddlewareCore.SampleApp.Core;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Users
{
    public class UserTokenExpirationWorker_Tests : EafMiddlewareTestBase
    {
        private readonly AbpUserManager<Role, User> _AbpUserManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly MyUserTokenExpirationWorker _userTokenExpirationWorker;
        private readonly IRepository<UserToken, long> _userTokenRepository;

        public UserTokenExpirationWorker_Tests()
        {
            _userTokenExpirationWorker = Resolve<MyUserTokenExpirationWorker>();
            _userTokenRepository = Resolve<IRepository<UserToken, long>>();
            _AbpUserManager = Resolve<AbpUserManager<Role, User>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public async Task Should_Remove_Expired_TokenValidityKeys()
        {
            //Arrange
            using (_unitOfWorkManager.Begin())
            {
                var user = await _AbpUserManager.GetUserByIdAsync(AbpSession.GetUserId());

                await _AbpUserManager.AddTokenValidityKeyAsync(
                    user,
                    Guid.NewGuid().ToString(),
                    DateTime.UtcNow
                );

                await _AbpUserManager.AddTokenValidityKeyAsync(
                    user,
                    Guid.NewGuid().ToString(),
                    DateTime.UtcNow.AddDays(1)
                );

                await _AbpUserManager.AddTokenValidityKeyAsync(
                    user,
                    Guid.NewGuid().ToString(),
                    DateTime.UtcNow.AddDays(1)
                );

                await _unitOfWorkManager.Current.SaveChangesAsync();

                var allTokens = await _userTokenRepository.GetAllListAsync(t => t.UserId == user.Id);
                allTokens.Count.ShouldBe(3);
            }

            using (_unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    var user = await _AbpUserManager.FindByNameOrEmailAsync(AbpUserBase.AdminUserName);

                    await _AbpUserManager.AddTokenValidityKeyAsync(
                        user,
                        Guid.NewGuid().ToString(),
                        DateTime.UtcNow
                    );

                    await _AbpUserManager.AddTokenValidityKeyAsync(
                        user,
                        Guid.NewGuid().ToString(),
                        DateTime.UtcNow.AddDays(1)
                    );

                    await _AbpUserManager.AddTokenValidityKeyAsync(
                        user,
                        Guid.NewGuid().ToString(),
                        DateTime.UtcNow.AddDays(1)
                    );

                    await _unitOfWorkManager.Current.SaveChangesAsync();

                    var allTokens = await _userTokenRepository.GetAllListAsync(t => t.UserId == user.Id);
                    allTokens.Count.ShouldBe(3);
                }
            }

            //Act
            _userTokenExpirationWorker.Start();

            //Assert
            using (_unitOfWorkManager.Begin())
            {
                var user = await _AbpUserManager.GetUserByIdAsync(AbpSession.GetUserId());
                var allTokens = await _userTokenRepository.GetAllListAsync(t => t.UserId == user.Id);
                allTokens.Count.ShouldBe(2);
            }

            using (_unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    var user = await _AbpUserManager.FindByNameOrEmailAsync(AbpUserBase.AdminUserName);
                    var allTokens = await _userTokenRepository.GetAllListAsync(t => t.UserId == user.Id);
                    allTokens.Count.ShouldBe(2);
                }
            }
        }
    }

    internal class MyUserTokenExpirationWorker : UserTokenExpirationWorker<Tenant, User>
    {
        public MyUserTokenExpirationWorker(AbpTimer timer, IRepository<UserToken, long> userTokenRepository,
            IBackgroundJobConfiguration backgroundJobConfiguration, IUnitOfWorkManager unitOfWorkManager,
            IRepository<Tenant> tenantRepository) : base(timer, userTokenRepository, backgroundJobConfiguration,
            unitOfWorkManager, tenantRepository)
        {
        }

        public override void Start()
        {
            DoWork();
        }
    }
}