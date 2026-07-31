using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
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
            var sourceUserId = AbpSession.UserId.Value;
            return await GetDelegationsAsync(sourceUserId, null, input);
        }

        /// <summary>
        /// Obtém as delegações ativas em que o usuário atual é destino.
        /// </summary>
        public virtual async Task<ListResultDto<UserDelegationDto>> GetDelegatedUsersAsync(GetUserDelegationsInput input)
        {
            var targetUserId = AbpSession.UserId.Value;
            var query = (await _userDelegationRepository.GetAllAsync())
                .Where(d => d.TargetUserId == targetUserId && !d.IsDeleted)
                .Where(d => d.StartTime <= Clock.Now && d.EndTime >= Clock.Now)
                .WhereIf(input.SourceUserId.HasValue, d => d.SourceUserId == input.SourceUserId.Value);

            var items = await query.OrderByDescending(d => d.CreationTime).ToListAsync();
            return new ListResultDto<UserDelegationDto>(ObjectMapper.Map<List<UserDelegationDto>>(items));
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

        private async Task<ListResultDto<UserDelegationDto>> GetDelegationsAsync(long? sourceUserId, long? targetUserId, GetUserDelegationsInput input)
        {
            var query = (await _userDelegationRepository.GetAllAsync())
                .WhereIf(sourceUserId.HasValue, d => d.SourceUserId == sourceUserId.Value)
                .WhereIf(targetUserId.HasValue, d => d.TargetUserId == targetUserId.Value)
                .WhereIf(input.TargetUserId.HasValue, d => d.TargetUserId == input.TargetUserId.Value)
                .Where(d => !d.IsDeleted);

            var items = await query.OrderByDescending(d => d.CreationTime).ToListAsync();
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
