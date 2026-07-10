using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;

namespace Eaf.Middleware.Worker.Tests.Middleware
{
    public class WorkerTestTenant : AbpTenant<WorkerTestUser>
    {
    }

    public class WorkerTestRole : AbpRole<WorkerTestUser>
    {
    }

    public class WorkerTestUser : AbpUser<WorkerTestUser>
    {
    }
}
