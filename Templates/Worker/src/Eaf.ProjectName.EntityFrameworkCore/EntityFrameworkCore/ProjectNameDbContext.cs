using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Eaf.ProjectName.EntityFrameworkCore
{
    public class ProjectNameDbContext : AbpDbContext
    {
        /* Define an IDbSet for each entity of the application */

        public ProjectNameDbContext(DbContextOptions<ProjectNameDbContext> options)
            : base(options)
        {
        }
    }
}