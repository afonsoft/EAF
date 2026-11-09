using Microsoft.EntityFrameworkCore;
using global::OpenIddict.EntityFrameworkCore;
using global::OpenIddict.EntityFrameworkCore.Models;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.OpenIddict.EntityFrameworkCore
{
    /// <summary>
    /// DbContext dedicado a los stores de OpenIddict (aplicaciones,
    /// autorizaciones, scopes y tokens). Apunta a la misma base de datos que
    /// el <c>ProjectNameDbContext</c>, pero se mantiene separado para no
    /// acoplar el esquema principal a las entidades de OpenIddict.
    /// </summary>
    public class EafOpenIddictDbContext : DbContext, IOpenIddictEntityFrameworkCoreContext
    {
        /// <summary>
        /// EafOpenIddictDbContext.
        /// </summary>
        public EafOpenIddictDbContext(DbContextOptions<EafOpenIddictDbContext> options)
            : base(options)
        {
        }

        /// <summary>Aplicaciones (clients) registradas.</summary>
        public DbSet<OpenIddictEntityFrameworkCoreApplication> Applications
            => Set<OpenIddictEntityFrameworkCoreApplication>();

        /// <summary>Autorizaciones persistentes.</summary>
        public DbSet<OpenIddictEntityFrameworkCoreAuthorization> Authorizations
            => Set<OpenIddictEntityFrameworkCoreAuthorization>();

        /// <summary>Scopes registrados.</summary>
        public DbSet<OpenIddictEntityFrameworkCoreScope> Scopes
            => Set<OpenIddictEntityFrameworkCoreScope>();

        /// <summary>Tokens persistidos (codes, refresh tokens, etc.).</summary>
        public DbSet<OpenIddictEntityFrameworkCoreToken> Tokens
            => Set<OpenIddictEntityFrameworkCoreToken>();

        /// <summary>Devuelve este contexto, requerido por los stores EF Core.</summary>
        public ValueTask<DbContext> GetDbContextAsync(CancellationToken cancellationToken)
            => new ValueTask<DbContext>(this);

        /// <summary>
        /// Representación de diagnóstico requerida por
        /// <see cref="IOpenIddictEntityFrameworkCoreContext"/>.
        /// </summary>
        public string GetChangeTrackerDebugString()
            => JsonSerializer.Serialize(ChangeTracker.Entries().Select(e => e.Entity.GetType().Name));

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseOpenIddict();
        }
    }
}
