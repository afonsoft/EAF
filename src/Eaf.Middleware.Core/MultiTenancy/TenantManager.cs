using Abp;
using Abp.Application.Features;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using Abp.Notifications;
using Abp.Runtime.Session;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Core.Editions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Tenant manager.
    /// </summary>
    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly RoleManager _roleManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUserEmailer _userEmailer;
        private readonly UserManager _userManager;

        /// <summary>
        /// TenantManager.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public TenantManager(
            IRepository<Tenant> tenantRepository,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            IUnitOfWorkManager unitOfWorkManager,
            RoleManager roleManager,
            IUserEmailer userEmailer,
            UserManager userManager,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IAbpZeroFeatureValueStore featureValueStore,
            IPasswordHasher<User> passwordHasher,
            EditionManager editionManager
        ) : base(
            tenantRepository,
            tenantFeatureRepository,
            editionManager,
            featureValueStore
        )
        {
            EafSession = NullAbpSession.Instance;

            _unitOfWorkManager = unitOfWorkManager;
            _roleManager = roleManager;
            _userEmailer = userEmailer;
            _userManager = userManager;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Obtém ou define EafSession.
        /// </summary>
        public IAbpSession EafSession { get; set; }

        /// <summary>
        /// CreateWithAdminUserAsync.
        /// </summary>
        /// <param name="tenancyName">Parâmetro tenancyName.</param>
        /// <param name="name">Parâmetro name.</param>
        /// <param name="adminPassword">Parâmetro adminPassword.</param>
        /// <param name="adminEmailAddress">Parâmetro adminEmailAddress.</param>
        /// <param name="isActive">Parâmetro isActive.</param>
        /// <param name="shouldChangePasswordOnNextLogin">Parâmetro shouldChangePasswordOnNextLogin.</param>
        /// <param name="sendActivationEmail">Parâmetro sendActivationEmail.</param>
        /// <param name="emailActivationLink">Parâmetro emailActivationLink.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<int> CreateWithAdminUserAsync(
            string tenancyName,
            string name,
            string adminPassword,
            string adminEmailAddress,
            bool isActive,
            bool shouldChangePasswordOnNextLogin,
            bool sendActivationEmail,
            string emailActivationLink
        )
        {
            int newTenantId;
            long newAdminId;

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                //Create tenant
                var tenant = new Tenant(tenancyName, name)
                {
                    IsActive = isActive
                };

                await CreateAsync(tenant);
                await _unitOfWorkManager.Current.SaveChangesAsync(); //To get new tenant's id.

                //We are working entities of new tenant, so changing tenant filter
                using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                {
                    //Create static roles for new tenant
                    CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get static role ids

                    //grant all permissions to admin role
                    var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                    await _roleManager.GrantAllPermissionsAsync(adminRole);

                    //Create admin user for the tenant
                    var adminUser = User.CreateTenantAdminUser(tenant.Id, adminEmailAddress);
                    adminUser.ShouldChangePasswordOnNextLogin = shouldChangePasswordOnNextLogin;
                    adminUser.IsActive = true;

                    if (adminPassword.IsNullOrEmpty())
                    {
                        adminPassword = User.CreateRandomPassword();
                    }
                    else
                    {
                        await _userManager.InitializeOptionsAsync(tenant.Id);
                        foreach (var validator in _userManager.PasswordValidators)
                        {
                            CheckErrors(await validator.ValidateAsync(_userManager, adminUser, adminPassword));
                        }
                    }

                    adminUser.Password = _passwordHasher.HashPassword(adminUser, adminPassword);

                    CheckErrors(await _userManager.CreateAsync(adminUser));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get admin user's id

                    //Assign admin user to admin role!
                    CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));

                    //Send activation email
                    if (sendActivationEmail)
                    {
                        adminUser.SetNewEmailConfirmationCode();
                        await _userEmailer.SendEmailActivationLinkAsync(adminUser, emailActivationLink, adminPassword);
                    }

                    await _unitOfWorkManager.Current.SaveChangesAsync();

                    newTenantId = tenant.Id;
                    newAdminId = adminUser.Id;
                }

                await uow.CompleteAsync();
            }

            //Used a second UOW since UOW above sets some permissions and _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync needs these permissions to be saved.
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(newTenantId))
                {
                    await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(new UserIdentifier(newTenantId, newAdminId));
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    await uow.CompleteAsync();
                }
            }

            return newTenantId;
        }

        /// <summary>
        /// GetById.
        /// </summary>
        /// <param name="value">Parâmetro value.</param>
        /// <returns>Resultado da operação.</returns>
        public static Tenant GetById(object value)
        {
            throw new NotImplementedException();
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}