using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Eaf.ProjectName.EntityFrameworkCore
{
    public static class ProjectNameDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<ProjectNameDbContext> builder, string connectionString, string databaseProvider = "SqlServer")
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<ProjectNameDbContext> builder, DbConnection connection, string databaseProvider = "SqlServer")
        {
            builder.UseSqlServer(connection);
        }
    }
}