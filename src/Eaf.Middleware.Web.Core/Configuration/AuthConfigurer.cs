using Abp.Authorization;
using Abp.Dependency;
using Eaf.Middleware.Web.Authentication.JwtBearer;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Startup
{
    /// <summary>
    /// Representa a classe AuthConfigurer.
    /// </summary>
    public static class AuthConfigurer
    {
        private static TokenAuthConfiguration ConfigureTokenAuth(IConfiguration configuration)
        {
            IocManager.Instance.RegisterIfNot<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Instance.Resolve<TokenAuthConfiguration>();

            string securityKey = configuration.GetValue<string>("Authentication:JwtBearer:SecurityKey");

            string validAudience = configuration.GetValue<string>("Authentication:JwtBearer:Audience");
            string validIssuer = configuration.GetValue<string>("Authentication:JwtBearer:Issuer");

            if (string.IsNullOrEmpty(securityKey))
                securityKey = "8CFB2EC534E14D56_EAF_8CFB2EC534E14D56";

            var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(securityKey));
            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(hmac.Key);
            tokenAuthConfig.Issuer = validIssuer;
            tokenAuthConfig.Audience = validAudience;
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256Signature);

            return tokenAuthConfig;
        }

        /// <summary>
        /// Authentication:JwtBearer:IsEnabled, Authority, SecurityKey, Audience, Issuer
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            var authenticationBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

            if (configuration["Authentication:JwtBearer:IsEnabled"] != null && bool.Parse(configuration["Authentication:JwtBearer:IsEnabled"]))
            {
                var tokenAuthConfig = ConfigureTokenAuth(configuration);

                var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true
                    || Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true;

                authenticationBuilder.AddJwtBearer(options =>
                {
                    options.IncludeErrorDetails = isDevelopment;
                    options.SaveToken = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IgnoreTrailingSlashWhenValidatingAudience = true,

                        // The signing key must match!
                        IssuerSigningKey = tokenAuthConfig.SecurityKey,
                        ValidateIssuerSigningKey = true,

                        // Validate the JWT Issuer (iss) claim
                        ValidateIssuer = true,
                        ValidIssuer = tokenAuthConfig.Issuer,

                        // Validate the JWT Audience (aud) claim
                        ValidateAudience = true,
                        ValidAudience = tokenAuthConfig.Audience,

                        // Validate the token expiry
                        ValidateLifetime = true,

                        // If you want to allow a certain amount of clock drift, set that here
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };

                    options.SecurityTokenValidators.Clear();
                    options.SecurityTokenValidators.Add(new MiddlewareJwtSecurityTokenHandler());

                    options.RefreshOnIssuerKeyNotFound = true;

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = QueryStringTokenResolver
                    };
                });
            }
        }

        /// <summary>
        /// This method is needed to authorize SignalR javascript client. SignalR can not send
        /// authorization header. So, we are getting it from query string as an encrypted text
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Task QueryStringTokenResolver(MessageReceivedContext context)
        {
            if (!context.HttpContext.Request.Path.HasValue)
            {
                return Task.CompletedTask;
            }

            List<string> urlsUsingEnchAuthToken = new List<string>()
            {
                "/Chat/GetUploadedObject?",
                "/Profile/GetProfilePictureByUser?"
            };

            List<string> urlsUsingEnchAuthTokenAllowAnonymous = new List<string>()
            {
                "/hangfire",
                "/job",
                "/signalr",
                "/health",
                "/heartbeat",
                "/healthchecks-ui"
            };

            if (urlsUsingEnchAuthTokenAllowAnonymous.Any(url => context.HttpContext.Request.GetDisplayUrl().Contains(url))
                || context.HttpContext.Request.Path.Value.StartsWith("/signalr")
                || context.HttpContext.Request.Path.Value.StartsWith("/health"))
            {
                return SetToken(context, true);
            }

            if (urlsUsingEnchAuthToken.Any(url => context.HttpContext.Request.GetDisplayUrl().Contains(url)))
            {
                if (context.HttpContext.Request.Headers.ContainsKey("authorization"))
                {
                    return Task.CompletedTask;
                }

                return SetToken(context, false);
            }

            return Task.CompletedTask;
        }

        private static Task SetToken(MessageReceivedContext context, bool allowAnonymous)
        {
            var path = context.HttpContext.Request.Path.Value ?? "";

            // SignalR JS client sends the JWT via access_token query string.
            if (path.StartsWith("/signalr"))
            {
                var accessToken = context.HttpContext.Request.Query["access_token"].FirstOrDefault();
                if (!string.IsNullOrEmpty(accessToken) && accessToken != "null")
                {
                    context.Token = accessToken;
                    return Task.CompletedTask;
                }
            }

            var qsAuthToken = context.HttpContext.Request.Query["enc_auth_token"].FirstOrDefault();
            if (string.IsNullOrEmpty(qsAuthToken) || qsAuthToken == "null")
            {
                if (!allowAnonymous)
                {
                    throw new AbpAuthorizationException("SignalR auth token is missing.");
                }

                return Task.CompletedTask;
            }

            //Set auth token from cookie
            context.Token = SimpleStringCipher.Instance.Decrypt(qsAuthToken, MiddlewareCoreConsts.DefaultPassPhrase);
            return Task.CompletedTask;
        }
    }
}