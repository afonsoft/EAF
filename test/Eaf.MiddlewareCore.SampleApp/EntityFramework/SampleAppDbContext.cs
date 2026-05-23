using Abp.Zero.EntityFrameworkCore;
using Eaf.MiddlewareCore.SampleApp.Core;
using Eaf.MiddlewareCore.SampleApp.Core.BookStore;
using Eaf.MiddlewareCore.SampleApp.Core.EntityHistory;
using Eaf.MiddlewareCore.SampleApp.Core.Shop;
using Microsoft.EntityFrameworkCore;

namespace Eaf.MiddlewareCore.SampleApp.EntityFramework
{
    //TODO: Re-enable when IdentityServer ready
    public class SampleAppDbContext : AbpZeroDbContext<Tenant, Role, User, SampleAppDbContext>
    {
        public SampleAppDbContext(DbContextOptions<SampleAppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Advertisement> Advertisements { get; set; }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Foo> Foo { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderTranslation> OrderTranslations { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<ProductTranslation> ProductTranslations { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<UserTestEntity> UserTestEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Blog>().OwnsOne(x => x.More);

            modelBuilder.Entity<Blog>().OwnsMany(x => x.Promotions, b =>
            {
                b.WithOwner().HasForeignKey(bp => bp.BlogId);
                b.Property<int>("Id");
                b.HasKey("Id");

                b.HasOne<Blog>()
                 .WithOne()
                 .HasForeignKey<BlogPromotion>(bp => bp.AdvertisementId)
                 .IsRequired();
            });

            modelBuilder.Entity<Advertisement>().OwnsMany(a => a.Feedbacks, b =>
            {
                b.WithOwner().HasForeignKey(af => af.AdvertisementId);
                b.Property<int>("Id");
                b.HasKey("Id");

                b.HasOne<Comment>()
                 .WithOne()
                 .HasForeignKey<AdvertisementFeedback>(af => af.CommentId);
            });

            modelBuilder.Entity<Book>().ToTable("Books");
            modelBuilder.Entity<Book>().Property(e => e.Id).ValueGeneratedNever();

            modelBuilder.Entity<Store>().Property(e => e.Id).HasColumnName("StoreId");
        }
    }
}