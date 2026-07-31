using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Timing;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.UserDelegations.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.UserDelegations
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de delegações de usuário.
    /// </summary>
    [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users_Delegation)]
    public class UserDelegationAppService : MiddlewareAppServiceBase, IUserDelegationAppService
    {
        private readonly IRepository<UserDelegation, long> _userDelegationRepository;
        private readonly IUserDelegationManager _userDelegationManager;
        private readonly UserManager _userManager;

        /// <summary>
        /// UserDelegationAppService.
        /// </summary>
        public UserDelegationAppService(
            IRepository<UserDelegation, long> userDelegationRepository,
            IUserDelegationManager userDelegationManager,
            UserManager userManager)
        {
            _userDelegationRepository = userDelegationRepository;
            _userDelegationManager = userDelegationManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Obtém as delegações do usuário atual como origem.
        /// </summary>
        public virtual async Task<ListResultDto<UserDelegationDto>> GetMyDelegationsAsync(GetUserDelegationsInput input)
        {
            input.SourceUserId = AbpSession.UserId.Value;
            return await GetDelegationsAsync(input, activeOnly: false);
        }

        /// <summary>
        /// Obtém as delegações ativas em que o usuário atual é destino.
        /// </summary>
        public virtual async Task<ListResultDto<UserDelegationDto>> GetDelegatedUsersAsync(GetUserDelegationsInput input)
        {
            input.TargetUserId = AbpSession.UserId.Value;
            return await GetDelegationsAsync(input, activeOnly: true);
        }

        /// <summary>
        /// Cria uma nova delegação de usuário.
        /// </summary>
        public virtual async Task<UserDelegationDto> CreateAsync(CreateUserDelegationInput input)
        {
            _userDelegationManager.ValidateDates(input.StartTime, input.EndTime);

            var sourceUserId = AbpSession.UserId.Value;
            var userDelegation = new UserDelegation
            {
                TenantId = AbpSession.TenantId,
                SourceUserId = sourceUserId,
                TargetUserId = input.TargetUserId,
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                Description = input.Description,
            };

            await _userDelegationRepository.InsertAsync(userDelegation);

            return ObjectMapper.Map<UserDelegationDto>(userDelegation);
        }

        /// <summary>
        /// Cancela (remove) uma delegação.
        /// </summary>
        public virtual async Task CancelAsync(EntityDto<long> input)
        {
            var userDelegation = await _userDelegationRepository.GetAsync(input.Id);
            if (userDelegation.SourceUserId != AbpSession.UserId.Value)
            {
                throw new Abp.AbpException("You can only cancel your own delegations.");
            }

            await _userDelegationRepository.DeleteAsync(userDelegation);
        }

        private async Task<ListResultDto<UserDelegationDto>> GetDelegationsAsync(GetUserDelegationsInput input, bool activeOnly)
        {
            var query = await _userDelegationRepository.GetAllAsync();

            if (input.SourceUserId.HasValue)
            {
                var sourceUserId = input.SourceUserId.Value;
                query = query.Where(d => d.SourceUserId == sourceUserId);
            }

            if (input.TargetUserId.HasValue)
            {
                var targetUserId = input.TargetUserId.Value;
                query = query.Where(d => d.TargetUserId == targetUserId);
            }

            if (activeOnly)
            {
                var now = Clock.Now;
                query = query.Where(d => d.StartTime <= now && d.EndTime >= now);
            }

            var items = await query.Where(d => !d.IsDeleted).OrderByDescending(d => d.CreationTime).ToListAsync();
            var dtos = ObjectMapper.Map<List<UserDelegationDto>>(items);

            foreach (var dto in dtos)
            {
                var sourceUser = await _userManager.GetUserByIdAsync(dto.SourceUserId);
                var targetUser = await _userManager.GetUserByIdAsync(dto.TargetUserId);
                dto.SourceUserName = sourceUser?.UserName;
                dto.TargetUserName = targetUser?.UserName;
                dto.IsActive = dto.StartTime <= Clock.Now && dto.EndTime >= Clock.Now;
            }

            return new ListResultDto<UserDelegationDto>(dtos);
        }
    }
}
