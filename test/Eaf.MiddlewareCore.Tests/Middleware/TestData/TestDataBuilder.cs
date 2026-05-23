using Eaf.MiddlewareCore.SampleApp.EntityFramework;

namespace Eaf.Middleware.TestData
{
    public class TestDataBuilder
    {
        private readonly SampleAppDbContext _context;
        private readonly int _tenantId;

        public TestDataBuilder(SampleAppDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            new TestRolesBuilder(_context, _tenantId).Create();
            new TestOrganizationUnitsBuilder(_context, _tenantId).Create();

            _context.SaveChanges();
        }
    }
}