using Abp.Zero.EntityFrameworkCore;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Core.Cache;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Storage;
using Eaf.ProjectName.Airplanes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using System;

namespace Eaf.ProjectName.EntityFrameworkCore
{
    public class ProjectNameDbContext : AbpZeroDbContext<Tenant, Role, User, ProjectNameDbContext>
    {
        private static bool _created = false;
        public static bool SkipMigrate { get; set; } = false;

        public ProjectNameDbContext(DbContextOptions<ProjectNameDbContext> options) : base(options)
        {
            if (!_created)
            {
                try
                {
                    _created = true;
                    if (!SkipMigrate)
                    {
                        Logger.Trace("Database Migrate started...");
                        Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    _created = false;
                    Logger.Warn("Database Migrate started Error ...", ex);
                }
            }
        }

        /* Define an IDbSet for each entity of the application */

        public virtual DbSet<Airplane> Airplanes { get; set; }
        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }
        public virtual DbSet<Friendship> Friendships { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
        public virtual DbSet<EafCache> EafCaches { get; set; }
        public virtual DbSet<TenantAddress> TenantAddress { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w => w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS));
            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tenant>(b =>
            {
                b.HasIndex(e => new { e.Name });
                b.HasIndex(e => new { e.CreationTime });
            });

            modelBuilder.Entity<EafCache>(b =>
            {
                b.HasIndex(e => new { e.Id });
                b.HasIndex(e => new { e.ExpiresAtTime });
            });

            modelBuilder.Entity<BinaryObject>(b =>
            {
                b.HasIndex(e => new { e.TenantId });
            });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId, e.ReadState });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.UserId, e.ReadState });
            });

            modelBuilder.Entity<Friendship>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.FriendUserId });
                b.HasIndex(e => new { e.FriendTenantId, e.UserId });
                b.HasIndex(e => new { e.FriendTenantId, e.FriendUserId });
            });

            modelBuilder.Entity<Abp.Auditing.AuditLog>(b =>
            {
                b.Property(e => e.Parameters).HasColumnType("nvarchar(max)");
            });
        }
    }
}