using Abp.Zero.EntityFrameworkCore;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Core.Cache;
using Eaf.Middleware.Core.Editions;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.MassNotifications;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Payments;
using Eaf.Middleware.Storage;
using Eaf.Middleware.UserDelegations;
using Eaf.ProjectName.Airplanes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.SqlServer.Diagnostics.Internal;
using System;
using System.Linq;

namespace Eaf.ProjectName.EntityFrameworkCore
{
    public class ProjectNameDbContext : AbpZeroDbContext<Tenant, Role, User, ProjectNameDbContext>
    {
        private static readonly object _migrateLock = new object();
        private static bool _migrated;

        /// <summary>
        /// Quando <c>true</c>, desabilita o <see cref="EnsureMigrated"/> durante a criação do contexto em design-time.
        /// </summary>
        public static bool IsDesignTime { get; set; }

        /// <summary>
        /// Quando <c>true</c>, desabilita a aplicação automática de migrations em runtime (usado nos testes).
        /// </summary>
        public static bool SkipMigrate { get; set; }

        public ProjectNameDbContext(DbContextOptions<ProjectNameDbContext> options)
            : base(options)
        {
            EnsureMigrated(this);
        }

        /// <summary>
        /// Garante que as migrations sejam aplicadas apenas uma vez durante a execução da aplicação.
        /// </summary>
        /// <param name="context">Contexto do Entity Framework a ser migrado.</param>
        private static void EnsureMigrated(DbContext context)
        {
            if (IsDesignTime || SkipMigrate)
                return;

            if (_migrated)
                return;

            lock (_migrateLock)
            {
                if (_migrated)
                    return;

                context.Database.Migrate();
                _migrated = true;
            }
        }

        /* Define an IDbSet for each entity of the application */

        public virtual DbSet<Airplane> Airplanes { get; set; }
        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }
        public virtual DbSet<Friendship> Friendships { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
        public virtual DbSet<EafCache> EafCaches { get; set; }
        public virtual DbSet<TenantAddress> TenantAddress { get; set; }
        public virtual DbSet<UserTenantMembership> UserTenantMemberships { get; set; }
        public virtual DbSet<TenantJoinRequest> TenantJoinRequests { get; set; }
        public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }
        public virtual DbSet<SubscriptionPaymentProduct> SubscriptionPaymentProducts { get; set; }
        public virtual DbSet<MassNotification> MassNotifications { get; set; }
        public virtual DbSet<UserDelegation> UserDelegations { get; set; }
        public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var isSqlServer = optionsBuilder.Options.Extensions
                .Any(e => e.GetType().FullName?.Contains("SqlServer") == true);

            if (isSqlServer)
            {
                optionsBuilder.ConfigureWarnings(w => w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS));
            }
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

            modelBuilder.Entity<UserTenantMembership>(b =>
            {
                b.HasIndex(e => new { e.UserId, e.TenantId }).IsUnique();
                b.HasIndex(e => e.TenantUserId);
            });

            modelBuilder.Entity<TenantJoinRequest>(b =>
            {
                b.HasIndex(e => new { e.UserId, e.TenantId });
                b.HasIndex(e => e.Status);
                b.Property(e => e.Status).HasConversion<int>();
            });

            modelBuilder.Entity<SubscribableEdition>(b =>
            {
                b.Property(e => e.DailyPrice).HasPrecision(18, 2);
                b.Property(e => e.WeeklyPrice).HasPrecision(18, 2);
                b.Property(e => e.MonthlyPrice).HasPrecision(18, 2);
                b.Property(e => e.AnnualPrice).HasPrecision(18, 2);
                b.Property(e => e.QuarterlyPrice).HasPrecision(18, 2);
                b.Property(e => e.BiannualPrice).HasPrecision(18, 2);
                b.Property(e => e.PermanentPrice).HasPrecision(18, 2);
            });

            modelBuilder.Entity<SubscriptionPayment>(b =>
            {
                b.Property(e => e.Amount).HasPrecision(18, 2);
                b.HasMany(e => e.Products)
                    .WithOne(e => e.SubscriptionPayment)
                    .HasForeignKey(e => e.SubscriptionPaymentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SubscriptionPaymentProduct>(b =>
            {
                b.Property(e => e.Amount).HasPrecision(18, 2);
                b.Property(e => e.TotalAmount).HasPrecision(18, 2);
            });

            if (Database.IsSqlServer())
            {
                modelBuilder.Entity<Abp.Auditing.AuditLog>(b =>
                {
                    b.Property(e => e.Parameters).HasColumnType("nvarchar(max)");
                });
            }
            else if (Database.IsNpgsql())
            {
                modelBuilder.Entity<Abp.Auditing.AuditLog>(b =>
                {
                    b.Property(e => e.Parameters).HasColumnType("text");
                });
            }
        }
    }
}