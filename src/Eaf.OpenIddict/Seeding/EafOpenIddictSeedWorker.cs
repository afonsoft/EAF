using Eaf.OpenIddict.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using global::OpenIddict.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.OpenIddict.Seeding
{
    /// <summary>
    /// Siembra las aplicaciones (clients) configuradas en
    /// <c>OpenIddict:Applications</c> dentro del store EF Core de OpenIddict.
    /// Es idempotente: actualiza descriptores existentes en lugar de duplicar.
    /// </summary>
    public class EafOpenIddictSeedWorker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EafOpenIddictSeedWorker> _logger;

        /// <summary>
        /// EafOpenIddictSeedWorker.
        /// </summary>
        public EafOpenIddictSeedWorker(
            IServiceProvider serviceProvider,
            ILogger<EafOpenIddictSeedWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var options = scope.ServiceProvider
                .GetRequiredService<IOptions<EafOpenIddictOptions>>().Value;
            var applicationManager = scope.ServiceProvider
                .GetRequiredService<IOpenIddictApplicationManager>();

            foreach (var app in options.Applications)
            {
                if (string.IsNullOrWhiteSpace(app.ClientId))
                {
                    continue;
                }

                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = app.ClientId,
                    ClientSecret = app.ClientSecret,
                    DisplayName = app.DisplayName ?? app.ClientId,
                    ConsentType = app.ConsentType switch
                    {
                        "explicit" => OpenIddictConstants.ConsentTypes.Explicit,
                        "systematic" => OpenIddictConstants.ConsentTypes.Systematic,
                        _ => OpenIddictConstants.ConsentTypes.Implicit,
                    },
                };

                foreach (var uri in app.RedirectUris)
                {
                    descriptor.RedirectUris.Add(new Uri(uri));
                }
                foreach (var uri in app.PostLogoutRedirectUris)
                {
                    descriptor.PostLogoutRedirectUris.Add(new Uri(uri));
                }
                foreach (var permission in ResolvePermissions(app))
                {
                    descriptor.Permissions.Add(permission);
                }

                var existing = await applicationManager
                    .FindByClientIdAsync(app.ClientId, cancellationToken);

                if (existing == null)
                {
                    await applicationManager.CreateAsync(descriptor, cancellationToken);
                }
                else
                {
                    await applicationManager.UpdateAsync(existing, descriptor, cancellationToken);
                }
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private static System.Collections.Generic.IEnumerable<string> ResolvePermissions(
            EafOpenIddictApplicationConfig app)
        {
            if (app.Permissions.Count > 0)
            {
                return app.Permissions;
            }

            return new[]
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.Password,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                OpenIddictConstants.Permissions.Prefixes.Scope + "default-api",
            };
        }
    }
}
