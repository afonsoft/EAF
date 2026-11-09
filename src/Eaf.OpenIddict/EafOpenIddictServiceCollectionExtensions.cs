using Eaf.OpenIddict.Configuration;
using Eaf.OpenIddict.EntityFrameworkCore;
using Eaf.OpenIddict.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using global::OpenIddict.Abstractions;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Eaf.OpenIddict
{
    /// <summary>
    /// Métodos de extensión para registrar el servidor OpenIddict del EAF.
    /// </summary>
    public static class EafOpenIddictServiceCollectionExtensions
    {
        /// <summary>
        /// Registra los stores EF Core de OpenIddict, el servidor OAuth2/OIDC y la
        /// validación local. Los access tokens son JWT firmados con la misma clave
        /// simétrica de <c>Authentication:JwtBearer</c>, por lo que los endpoints
        /// protegidos con <c>[AbpAuthorize]</c> los aceptan cuando el issuer del
        /// servidor se agrega a <c>Authentication:JwtBearer:AdditionalIssuers</c>.
        /// </summary>
        public static IServiceCollection AddEafOpenIddict(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var options = new EafOpenIddictOptions();
            configuration.GetSection(EafOpenIddictOptions.SectionName).Bind(options);
            if (string.IsNullOrEmpty(options.Audience))
            {
                options.Audience = configuration["Authentication:JwtBearer:Audience"];
            }
            services.Configure<EafOpenIddictOptions>(o =>
                configuration.GetSection(EafOpenIddictOptions.SectionName).Bind(o));

            services.AddDbContext<EafOpenIddictDbContext>(dbOptions =>
            {
                dbOptions.UseSqlServer(configuration.GetConnectionString("Default"));
                dbOptions.UseOpenIddict();
            });

            // La clave simétrica se procesa igual que en AuthConfigurer (HMAC-SHA256
            // sobre el secreto de configuración) para compartir credenciales.
            var rawKey = configuration["Authentication:JwtBearer:SecurityKey"]
                ?? "8CFB2EC534E14D56_EAF_8CFB2EC534E14D56";
            var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(rawKey));
            var signingKey = new SymmetricSecurityKey(hmac.Key);

            services.AddOpenIddict()
                .AddCore(core => core
                    .UseEntityFrameworkCore()
                    .UseDbContext<EafOpenIddictDbContext>())
                .AddServer(server =>
                {
                    server.SetTokenEndpointUris("/connect/token")
                          .SetAuthorizationEndpointUris("/connect/authorize")
                          .SetUserInfoEndpointUris("/connect/userinfo")
                          .SetEndSessionEndpointUris("/connect/logout");

                    server.AllowAuthorizationCodeFlow()
                          .AllowPasswordFlow()
                          .AllowClientCredentialsFlow()
                          .AllowRefreshTokenFlow()
                          .RequireProofKeyForCodeExchange();

                    server.RegisterScopes(
                        OpenIddictConstants.Scopes.OpenId,
                        OpenIddictConstants.Scopes.Profile,
                        OpenIddictConstants.Scopes.Email,
                        OpenIddictConstants.Scopes.Roles,
                        "default-api");

                    if (Uri.TryCreate(options.Issuer, UriKind.Absolute, out var issuer))
                    {
                        server.SetIssuer(issuer);
                    }

                    // Firma con la misma clave simétrica del JwtBearer de EAF para que
                    // los access tokens sean aceptados por los controladores [AbpAuthorize].
                    server.AddSigningKey(signingKey);
                    server.DisableAccessTokenEncryption();
                    server.SetAccessTokenLifetime(TimeSpan.FromMinutes(options.AccessTokenLifetimeMinutes));
                    server.SetAuthorizationCodeLifetime(TimeSpan.FromMinutes(options.AuthorizationCodeLifetimeMinutes));

                    server.UseAspNetCore()
                          .EnableTokenEndpointPassthrough()
                          .EnableAuthorizationEndpointPassthrough()
                          .EnableUserInfoEndpointPassthrough()
                          .EnableEndSessionEndpointPassthrough();
                })
                .AddValidation(validation =>
                {
                    validation.UseLocalServer();
                    validation.UseAspNetCore();
                });

            services.AddHostedService<EafOpenIddictSeedWorker>();

            return services;
        }
    }
}
