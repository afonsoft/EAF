using Eaf.MiddlewareCore.SampleApp.Core;
using System.Linq;

namespace Eaf.MiddlewareCore.SampleApp.EntityFramework.Seed.Tenants
{
    public class DefaultTenantBuilder
    {
        private readonly SampleAppDbContext _context;

        public DefaultTenantBuilder(SampleAppDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateDefaultTenant();
        }

        private void CreateDefaultTenant()
        {
            //Default tenant

            var defaultTenant = _context.Tenants.FirstOrDefault(t => t.TenancyName == Tenant.DefaultTenantName);
            if (defaultTenant != null)
            {
                return;
            }

            defaultTenant = new Tenant(Tenant.DefaultTenantName, Tenant.DefaultTenantName);

            _context.Tenants.Add(defaultTenant);
            _context.SaveChanges();
        }
    }
}