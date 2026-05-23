using Abp.Authorization.Users;

namespace Eaf.MiddlewareCore.SampleApp.Core
{
    public class User : AbpUser<User>
    {
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

        public override string ToString()
        {
            return $"[User {Id}] {UserName}";
        }
    }
}