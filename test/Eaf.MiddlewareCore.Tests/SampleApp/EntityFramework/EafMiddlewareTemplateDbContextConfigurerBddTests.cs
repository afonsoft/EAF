using System.Linq;
using Eaf.MiddlewareCore.SampleApp.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.SampleApp.EntityFramework
{
    public class EafMiddlewareTemplateDbContextConfigurerBddTests
    {
        [Fact]
        public void Dado_ConnectionStringSqlServer_Quando_Configure_Entao_DeveConfigurarDbContextOptions()
        {
            var builder = new DbContextOptionsBuilder<SampleAppDbContext>();
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=EafTest;Trusted_Connection=True;";

            EafMiddlewareTemplateDbContextConfigurer.Configure(builder, connectionString);

            builder.IsConfigured.ShouldBeTrue();
            builder.Options.Extensions.ShouldContain(e => e.GetType().FullName.Contains("SqlServer"));
        }
    }
}
