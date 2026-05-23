using Abp.Dependency;
using Abp.Domain.Uow;
using Eaf.Middleware.Sessions;
using Eaf.Middleware.Sessions.Dto;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Session
{
    /// <summary>
    /// Representa a classe PerRequestSessionCache.
    /// </summary>
    public class PerRequestSessionCache : IPerRequestSessionCache, ITransientDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionAppService _sessionAppService;

        /// <summary>
        /// PerRequestSessionCache.
        /// </summary>
        /// <param name="httpContextAccessor">Parâmetro httpContextAccessor.</param>
        /// <param name="sessionAppService">Parâmetro sessionAppService.</param>
        /// <returns>Resultado da operação.</returns>
        public PerRequestSessionCache(
            IHttpContextAccessor httpContextAccessor,
            ISessionAppService sessionAppService)
        {
            _httpContextAccessor = httpContextAccessor;
            _sessionAppService = sessionAppService;
        }

        [UnitOfWork]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return await _sessionAppService.GetCurrentLoginInformations();
            }

            var cachedValue = httpContext.Items["__PerRequestSessionCache"] as GetCurrentLoginInformationsOutput;
            if (cachedValue == null)
            {
                cachedValue = await _sessionAppService.GetCurrentLoginInformations();
                if (cachedValue.User != null)
                    httpContext.Items["__PerRequestSessionCache"] = cachedValue;
            }

            return cachedValue;
        }
    }
}