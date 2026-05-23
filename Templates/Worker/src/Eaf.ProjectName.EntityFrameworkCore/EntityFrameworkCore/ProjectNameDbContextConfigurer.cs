using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Eaf.ProjectName.EntityFrameworkCore
{
    public static class ProjectNameDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<ProjectNameDbContext> builder, string connectionString, string databaseProvider)
        {
            // TODO: Add PostgreSQL support when Npgsql.EntityFrameworkCore.PostgreSQL has stable EF Core 10.0 compatible version
            // TODO: Add MySQL support when Pomelo.EntityFrameworkCore.MySql has stable EF Core 10.0 compatible version
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<ProjectNameDbContext> builder, DbConnection connection, string databaseProvider)
        {
            // TODO: Add PostgreSQL support when Npgsql.EntityFrameworkCore.PostgreSQL has stable EF Core 10.0 compatible version
            // TODO: Add MySQL support when Pomelo.EntityFrameworkCore.MySql has stable EF Core 10.0 compatible version
            builder.UseSqlServer(connection);
        }
    }
}