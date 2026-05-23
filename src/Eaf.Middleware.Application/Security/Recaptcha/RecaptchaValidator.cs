using Abp;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Json;
using Abp.UI;
using Microsoft.AspNetCore.Http;
using Owl.reCAPTCHA;
using Owl.reCAPTCHA.v3;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Security.Recaptcha
{
    /// <summary>
    /// Representa a classe RecaptchaValidator.
    /// </summary>
    public class RecaptchaValidator : MiddlewareAppServiceBase, IRecaptchaValidator, ITransientDependency
    {
        public const string RecaptchaResponseKey = "g-recaptcha-response";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IreCAPTCHASiteVerifyV3 _reCAPTCHASiteVerifyV3;

        /// <summary>
        /// RecaptchaValidator.
        /// </summary>
        /// <param name="reCAPTCHASiteVerifyV3">Parâmetro reCAPTCHASiteVerifyV3.</param>
        /// <param name="httpContextAccessor">Parâmetro httpContextAccessor.</param>
        /// <returns>Resultado da operação.</returns>
        public RecaptchaValidator(IreCAPTCHASiteVerifyV3 reCAPTCHASiteVerifyV3, IHttpContextAccessor httpContextAccessor)
        {
            _reCAPTCHASiteVerifyV3 = reCAPTCHASiteVerifyV3;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// ValidateAsync.
        /// </summary>
        /// <param name="captchaResponse">Parâmetro captchaResponse.</param>
        public async Task ValidateAsync(string captchaResponse)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new AbpException("RecaptchaValidator should be used in a valid HTTP context!");
            }

            if (captchaResponse.IsNullOrEmpty())
            {
                throw new UserFriendlyException(L("CaptchaCanNotBeEmpty"));
            }

            var response = await _reCAPTCHASiteVerifyV3.Verify(new reCAPTCHASiteVerifyRequest
            {
                Response = captchaResponse,
                RemoteIp = _httpContextAccessor.HttpContext.Connection?.RemoteIpAddress?.ToString()
            });

            if (!response.Success || response.Score < 0.5)
            {
                Logger.Warn(response.ToJsonString());
                throw new UserFriendlyException(L("IncorrectCaptchaAnswer"));
            }
        }
    }
}