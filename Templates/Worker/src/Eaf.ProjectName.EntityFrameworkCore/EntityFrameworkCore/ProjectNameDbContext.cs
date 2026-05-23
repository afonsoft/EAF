using Eaf.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Eaf.ProjectName.EntityFrameworkCore
{
    public class ProjectNameDbContext : EafDbContext
    {
        /* Define an IDbSet for each entity of the application */

        public ProjectNameDbContext(DbContextOptions<ProjectNameDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}