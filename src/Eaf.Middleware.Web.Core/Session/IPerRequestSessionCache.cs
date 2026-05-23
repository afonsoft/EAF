using Eaf.Middleware.Sessions.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Session
{
    /// <summary>
    /// Representa a interface IPerRequestSessionCache.
    /// </summary>
    public interface IPerRequestSessionCache
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync();
    }
}