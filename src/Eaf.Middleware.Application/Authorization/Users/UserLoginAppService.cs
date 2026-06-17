using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Eaf.Middleware.Authorization.Users.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authorization.Users
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de UserLogin.
    /// </summary>
    [AbpAuthorize]
    public class UserLoginAppService : MiddlewareAppServiceBase, IUserLoginAppService
    {
        private readonly IRepository<UserLoginAttempt, long> _userLoginAttemptRepository;

        /// <summary>
        /// UserLoginAppService.
        /// </summary>
        /// <param name="userLoginAttemptRepository">Parâmetro userLoginAttemptRepository.</param>
        /// <returns>Resultado da operação.</returns>
        public UserLoginAppService(
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository
        )
        {
            _userLoginAttemptRepository = userLoginAttemptRepository;
        }

        [DisableAuditing]
        public async Task<ListResultDto<UserLoginAttemptDto>> GetRecentUserLoginAttempts()
        {
            var userId = AbpSession.GetUserId();

            var loginAttempts = await _userLoginAttemptRepository.GetAll()
                .Where(la => la.UserId == userId)
                .OrderByDescending(la => la.CreationTime)
                .Take(10)
                .ToListAsync();

            return new ListResultDto<UserLoginAttemptDto>(ObjectMapper.Map<List<UserLoginAttemptDto>>(loginAttempts));
        }
    }
}