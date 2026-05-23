namespace Eaf.MiddlewareCore.SampleApp.EntityFramework.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly SampleAppDbContext _context;

        public InitialHostDbBuilder(SampleAppDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}