using Microsoft.EntityFrameworkCore;

namespace Eaf.MiddlewareCore.SampleApp.EntityFramework
{
    public static class EafMiddlewareTemplateDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<SampleAppDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }
    }
}