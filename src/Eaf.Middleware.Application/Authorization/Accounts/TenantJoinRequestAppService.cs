using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Data;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Eaf.Middleware.Authorization.Accounts.Dto;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authorization.Accounts
{
    /// <summary>
    /// Serviço de aplicação para solicitações de ingresso em tenants.
    /// </summary>
    public class TenantJoinRequestAppService : MiddlewareAppServiceBase, ITenantJoinRequestAppService
    {
        private readonly IRepository<TenantJoinRequest, long> _joinRequestRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly ITenantUserManager _tenantUserManager;

        /// <summary>
        /// TenantJoinRequestAppService.
        /// </summary>
        public TenantJoinRequestAppService(
            IRepository<TenantJoinRequest, long> joinRequestRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<User, long> userRepository,
            ITenantUserManager tenantUserManager)
        {
            _joinRequestRepository = joinRequestRepository;
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _tenantUserManager = tenantUserManager;
        }

        /// <inheritdoc/>
        [AbpAllowAnonymous]
        public virtual async Task<List<AvailableTenantDto>> GetAvailableTenantsAsync()
        {
            var tenants = await _tenantRepository.GetAll()
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();

            return ObjectMapper.Map<List<AvailableTenantDto>>(tenants);
        }

        /// <inheritdoc/>
        [AbpAuthorize]
        public virtual async Task<TenantJoinRequestDto> CreateRequestAsync(CreateTenantJoinRequestInput input)
        {
            if (!await SettingManager.GetSettingValueAsync<bool>(AppSettings.TenantManagement.AllowJoinRequests))
                throw new UserFriendlyException(L("JoinRequestsAreDisabled"));

            var request = await _tenantUserManager.CreatePendingMembershipAsync(
                AbpSession.UserId.Value,
                input.TenantId,
                input.Message);

            return (await MapRequestsAsync(new List<TenantJoinRequest> { request })).First();
        }

        /// <inheritdoc/>
        [AbpAuthorize]
        public virtual async Task<List<TenantJoinRequestDto>> GetMyRequestsAsync()
        {
            using (CurrentUnitOfWork.SetTenantId(null, switchMustHaveTenantEnableDisable: false))
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var requests = await _joinRequestRepository.GetAll()
                    .Where(r => r.UserId == AbpSession.UserId.Value)
                    .OrderByDescending(r => r.CreationTime)
                    .ToListAsync();

                return await MapRequestsAsync(requests);
            }
        }

        /// <inheritdoc/>
        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users)]
        public virtual async Task<List<TenantJoinRequestDto>> GetPendingRequestsForCurrentTenantAsync()
        {
            if (!AbpSession.TenantId.HasValue)
                throw new UserFriendlyException(L("TenantRequired"));

            using (CurrentUnitOfWork.SetTenantId(null, switchMustHaveTenantEnableDisable: false))
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var requests = await _joinRequestRepository.GetAll()
                    .Where(r => r.TenantId == AbpSession.TenantId.Value && r.Status == TenantJoinRequestStatus.Pending)
                    .OrderByDescending(r => r.CreationTime)
                    .ToListAsync();

                return await MapRequestsAsync(requests);
            }
        }

        private async Task<List<TenantJoinRequestDto>> MapRequestsAsync(List<TenantJoinRequest> requests)
        {
            var result = ObjectMapper.Map<List<TenantJoinRequestDto>>(requests);

            var userIds = requests.Select(r => r.UserId).Distinct().ToList();
            var tenantIds = requests.Select(r => r.TenantId).Distinct().ToList();

            var users = await _userRepository.GetAll()
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName);

            var tenants = await _tenantRepository.GetAll()
                .Where(t => tenantIds.Contains(t.Id))
                .ToDictionaryAsync(t => t.Id, t => t.Name);

            foreach (var dto in result)
            {
                dto.UserName = users.ContainsKey(dto.UserId) ? users[dto.UserId] : null;
                dto.TenantName = tenants.ContainsKey(dto.TenantId) ? tenants[dto.TenantId] : null;
            }

            return result;
        }

        /// <inheritdoc/>
        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users)]
        public virtual async Task ApproveAsync(ApproveTenantJoinRequestInput input)
        {
            if (input.IsApproved)
            {
                await _tenantUserManager.ApproveMembershipAsync(input.RequestId, AbpSession.UserId.Value);
                return;
            }

            using (CurrentUnitOfWork.SetTenantId(null, switchMustHaveTenantEnableDisable: false))
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var request = await _joinRequestRepository.GetAsync(input.RequestId);
                if (request.Status != TenantJoinRequestStatus.Pending)
                    throw new UserFriendlyException(L("RequestAlreadyProcessed"));

                request.Status = TenantJoinRequestStatus.Rejected;
                request.ApproverUserId = AbpSession.UserId.Value;
            }
        }
    }
}
