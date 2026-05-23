using Abp.MultiTenancy;

namespace Eaf.MiddlewareCore.SampleApp.Core
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }

        protected Tenant()
        {
        }
    }
}