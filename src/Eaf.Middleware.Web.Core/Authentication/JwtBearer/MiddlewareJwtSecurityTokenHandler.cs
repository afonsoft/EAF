using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Logging;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Configuration;
using Abp.Runtime.Caching;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Abp;

namespace Eaf.Middleware.Web.Authentication.JwtBearer
{
    /// <summary>
    /// Representa a classe MiddlewareJwtSecurityTokenHandler.
    /// </summary>
    public class MiddlewareJwtSecurityTokenHandler : ISecurityTokenValidator
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;

        /// <summary>
        /// MiddlewareJwtSecurityTokenHandler.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public MiddlewareJwtSecurityTokenHandler()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public bool CanValidateToken => true;

        /// <summary>
        /// Obtém ou define MaximumTokenSizeInBytes.
        /// </summary>
        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        /// <summary>
        /// CanReadToken.
        /// </summary>
        /// <param name="securityToken">Parâmetro securityToken.</param>
        /// <returns>Resultado da operação.</returns>
        public bool CanReadToken(string securityToken)
        {
            return _tokenHandler.CanReadToken(securityToken);
        }

        /// <summary>
        /// ValidateToken.
        /// </summary>
        /// <param name="securityToken">Parâmetro securityToken.</param>
        /// <param name="validationParameters">Parâmetro validationParameters.</param>
        /// <param name="validatedToken">Parâmetro validatedToken.</param>
        /// <returns>Resultado da operação.</returns>
        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            try
            {
                var cacheManager = IocManager.Instance.Resolve<ICacheManager>();

                var principal = _tokenHandler.ValidateToken(securityToken, validationParameters, out validatedToken);

                var userIdentifierString = principal.Claims.FirstOrDefault(c => c.Type == MiddlewareCoreConsts.UserIdentifier)?.Value ?? "";
                var tokenValidityKeyInClaims = principal.Claims.FirstOrDefault(c => c.Type == MiddlewareCoreConsts.TokenValidityKey)?.Value ?? "";
                var tokenValidityValueInClaims = principal.Claims.FirstOrDefault(c => c.Type == MiddlewareCoreConsts.TokenValidityValue)?.Value ?? "";

                var tokenValidityKeyInCache = cacheManager
                    .GetCache(MiddlewareCoreConsts.TokenValidityKey)
                    .GetOrDefault(tokenValidityKeyInClaims);

                if (tokenValidityKeyInCache != null)
                {
                    return principal;
                }

                bool securityStampValid = false;
                bool isValidityKetValid = false;
                string userSecurityStamp = "";

                using (var unitOfWorkManager = IocManager.Instance.ResolveAsDisposable<IUnitOfWorkManager>())
                {
                    using (var uow = unitOfWorkManager.Object.Begin())
                    {
                        var userIdentifier = UserIdentifier.Parse(userIdentifierString);

                        using (unitOfWorkManager.Object.Current.SetTenantId(userIdentifier.TenantId))
                        {
                            using (var userManager = IocManager.Instance.ResolveAsDisposable<UserManager>())
                            {
                                var userManagerObject = userManager.Object;
                                var user = userManagerObject.GetUser(userIdentifier);

                                userSecurityStamp = user.SecurityStamp ?? "";
                                securityStampValid = tokenValidityValueInClaims == userSecurityStamp;
                                isValidityKetValid = userManagerObject.IsTokenValidityKeyValid(user, tokenValidityKeyInClaims);
                                uow.Complete();

                                if (string.IsNullOrEmpty(user.SecurityStamp))
                                    user.SecurityStamp = Guid.NewGuid().ToString("N");

                                if (securityStampValid || isValidityKetValid)
                                {
                                    using (var settingManager = IocManager.Instance.ResolveAsDisposable<ISettingManager>())
                                    {
                                        var allowOneConcurrentLoginPerUser = settingManager.Object.GetSettingValue<bool>(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser);

                                        if (allowOneConcurrentLoginPerUser && !securityStampValid)
                                            throw new SecurityTokenException("Invalid Token allow One Concurrent Login Per User");

                                        var expirationSettings = settingManager.Object.GetSettingValue<int>(AppSettings.UserManagement.TokenExpiration);
                                        var expiration = TimeSpan.FromSeconds(expirationSettings);

                                        cacheManager
                                            .GetCache(MiddlewareCoreConsts.TokenValidityKey)
                                            .Set(tokenValidityKeyInClaims, tokenValidityValueInClaims,
                                            slidingExpireTime: expiration,
                                            absoluteExpireTime: DateTimeOffset.UtcNow.Add(expiration).AddHours(1));
                                    }

                                    return principal;
                                }
                            }
                        }
                    }

                    throw new SecurityTokenException("Invalid Token");
                }
            }
            catch (SecurityTokenException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}