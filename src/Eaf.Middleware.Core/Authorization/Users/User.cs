using Abp.Authorization.Users;
using Abp.Extensions;
using Abp.Timing;
using System;

namespace Eaf.Middleware.Authorization.Users
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User : AbpUser<User>
    {
        /// <summary>
        /// User.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public User()
        {
            IsLockoutEnabled = true;
        }

        public virtual Guid? ProfilePictureId { get; set; }

        /// <summary>
        /// Obtém ou define ShouldChangePasswordOnNextLogin.
        /// </summary>
        public virtual bool ShouldChangePasswordOnNextLogin { get; set; }

        public DateTime? SignInTokenExpireTimeUtc { get; set; }

        /// <summary>
        /// Obtém ou define ExternalAuthProviderformation.
        /// </summary>
        public string ExternalAuthProviderformation { get; set; }

        /// <summary>
        /// Obtém ou define SignInToken.
        /// </summary>
        public virtual string SignInToken { get; set; }


        /// <summary>
        /// CreateRandomPassword.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        /// <summary>
        /// Creates admin <see cref="User"/> for a tenant.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="emailAddress">Email address</param>
        /// <returns>Created <see cref="User"/> object</returns>
        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress
            };

            user.SetNormalizedNames();

            return user;
        }

        /// <summary>
        /// SetNewPasswordResetCode.
        /// </summary>
        public override void SetNewPasswordResetCode()
        {
            PasswordResetCode = Guid.NewGuid().ToString("N").Truncate(10).ToUpperInvariant();
        }

        /// <summary>
        /// SetSignInToken.
        /// </summary>
        /// <param name="seconds">Parâmetro seconds.</param>
        public void SetSignInToken(int? seconds = null)
        {
            SignInToken = Guid.NewGuid().ToString();
            SignInTokenExpireTimeUtc = Clock.Now.AddSeconds(seconds ?? (30 * 60)).ToUniversalTime();
        }

        /// <summary>
        /// Unlock.
        /// </summary>
        public void Unlock()
        {
            AccessFailedCount = 0;
            LockoutEndDateUtc = null;
        }
    }
}