using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Eaf.OpenIddict.EntityFrameworkCore
{
    /// <summary>
    /// Factory de diseño para <c>dotnet ef</c>. Lee la cadena de conexión de
    /// la variable de entorno <c>ConnectionStrings__Default</c> y, en su
    /// defecto, apunta a una instancia local de SQL Server.
    /// </summary>
    public class EafOpenIddictDbContextFactory
        : IDesignTimeDbContextFactory<EafOpenIddictDbContext>
    {
        /// <inheritdoc />
        public EafOpenIddictDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                ?? "Server=(localdb)\\mssqllocaldb;Database=EafOpenIddictDesign;Trusted_Connection=True";

            var options = new DbContextOptionsBuilder<EafOpenIddictDbContext>()
                .UseSqlServer(connectionString)
                .UseOpenIddict()
                .Options;

            return new EafOpenIddictDbContext(options);
        }
    }
}
