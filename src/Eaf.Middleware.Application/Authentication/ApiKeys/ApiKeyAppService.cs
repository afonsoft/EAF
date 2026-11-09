using Abp;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.UI;
using Eaf.Middleware.Application.Authentication.ApiKeys.Dto;
using Eaf.Middleware.Authentication.ApiKeys;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Application.Authentication.ApiKeys
{
    /// <summary>
    /// Gerenciamento das API keys do próprio usuário ("My API Keys").
    /// </summary>
    public interface IApiKeyAppService
    {
        /// <summary>Lista as chaves do usuário autenticado.</summary>
        Task<ListResultDto<ApiKeyDto>> GetMyApiKeys();

        /// <summary>Cria uma nova chave; o valor em claro é retornado uma única vez.</summary>
        Task<CreatedApiKeyDto> CreateApiKey(CreateApiKeyInput input);

        /// <summary>Revoga uma chave do usuário autenticado.</summary>
        Task RevokeApiKey(EntityDto<long> input);

        /// <summary>Rotaciona uma chave: revoga a atual e emite substituta.</summary>
        Task<CreatedApiKeyDto> RotateApiKey(EntityDto<long> input);
    }

    /// <summary>
    /// Implementação de <see cref="IApiKeyAppService"/>.
    /// </summary>
    [AbpAuthorize]
    public class ApiKeyAppService : MiddlewareAppServiceBase, IApiKeyAppService
    {
        private readonly IApiKeyManager _apiKeyManager;
        private readonly IRepository<ApiKey, long> _apiKeyRepository;
        private readonly UserManager _userManager;

        /// <summary>
        /// ApiKeyAppService.
        /// </summary>
        public ApiKeyAppService(
            IApiKeyManager apiKeyManager,
            IRepository<ApiKey, long> apiKeyRepository,
            UserManager userManager)
        {
            _apiKeyManager = apiKeyManager;
            _apiKeyRepository = apiKeyRepository;
            _userManager = userManager;
        }

        /// <inheritdoc />
        public async Task<ListResultDto<ApiKeyDto>> GetMyApiKeys()
        {
            var keys = await _apiKeyRepository.GetAllListAsync(
                k => k.UserId == AbpSession.UserId.Value && k.TenantId == AbpSession.TenantId);

            return new ListResultDto<ApiKeyDto>(keys.Select(MapToDto).ToList());
        }

        /// <inheritdoc />
        public async Task<CreatedApiKeyDto> CreateApiKey(CreateApiKeyInput input)
        {
            await EnsureEnabledAsync();

            var maxKeys = await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.ApiKeys.MaxKeysPerUser);
            var activeCount = (await _apiKeyRepository.GetAllListAsync(
                k => k.UserId == AbpSession.UserId.Value && k.TenantId == AbpSession.TenantId && k.Status == ApiKeyStatus.Active))
                .Count;
            if (activeCount >= maxKeys)
            {
                throw new UserFriendlyException($"Maximum of {maxKeys} active API keys per user reached.");
            }

            var requireScope = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.ApiKeys.RequireScope);
            if (requireScope && (input.PermissionScopes == null || input.PermissionScopes.Length == 0))
            {
                throw new UserFriendlyException("An explicit permission scope is required for API keys.");
            }

            var expiration = NormalizeExpiration(input.ExpirationTime,
                await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.ApiKeys.MaxLifetimeDays));

            var (entity, plainKey) = await _apiKeyManager.CreateAsync(
                new UserIdentifier(AbpSession.TenantId, AbpSession.UserId.Value),
                input.Name,
                input.PermissionScopes,
                input.AllowedIpRanges,
                expiration);

            return new CreatedApiKeyDto { ApiKey = MapToDto(entity), PlainKey = plainKey };
        }

        /// <inheritdoc />
        public async Task RevokeApiKey(EntityDto<long> input)
        {
            var key = await _apiKeyRepository.FirstOrDefaultAsync(input.Id);
            EnsureOwnership(key);
            await _apiKeyManager.RevokeAsync(input.Id);
        }

        /// <inheritdoc />
        public async Task<CreatedApiKeyDto> RotateApiKey(EntityDto<long> input)
        {
            await EnsureEnabledAsync();
            var key = await _apiKeyRepository.FirstOrDefaultAsync(input.Id);
            EnsureOwnership(key);

            var (entity, plainKey) = await _apiKeyManager.RotateAsync(input.Id);
            return new CreatedApiKeyDto { ApiKey = MapToDto(entity), PlainKey = plainKey };
        }

        private void EnsureOwnership(ApiKey key)
        {
            if (key == null || key.UserId != AbpSession.UserId.Value || key.TenantId != AbpSession.TenantId)
            {
                throw new UserFriendlyException("API key not found.");
            }
        }

        private async Task EnsureEnabledAsync()
        {
            if (!await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.ApiKeys.IsEnabled))
            {
                throw new UserFriendlyException("API keys are disabled.");
            }
        }

        private static DateTime? NormalizeExpiration(DateTime? requested, int maxLifetimeDays)
        {
            if (maxLifetimeDays <= 0)
            {
                return requested;
            }
            var max = DateTime.UtcNow.AddDays(maxLifetimeDays);
            return requested.HasValue && requested.Value <= max ? requested : max;
        }

        private ApiKeyDto MapToDto(ApiKey k)
        {
            return new ApiKeyDto
            {
                Id = k.Id,
                Name = k.Name,
                KeyPrefix = k.KeyPrefix,
                PermissionScopes = k.GetPermissionScopes(),
                AllowedIpRanges = string.IsNullOrWhiteSpace(k.AllowedIpRanges)
                    ? null
                    : k.AllowedIpRanges.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                ExpirationTime = k.ExpirationTime,
                Status = k.Status,
                LastUsedTime = k.LastUsedTime,
                CreationTime = k.CreationTime,
                UserId = k.UserId
            };
        }
    }

    /// <summary>
    /// Administração de todas as API keys do tenant ("Administration > API Keys").
    /// </summary>
    public interface IAdminApiKeyAppService
    {
        /// <summary>Lista todas as chaves do tenant atual.</summary>
        Task<ListResultDto<ApiKeyDto>> GetTenantApiKeys();

        /// <summary>Revoga qualquer chave do tenant.</summary>
        Task RevokeApiKey(EntityDto<long> input);
    }

    /// <summary>
    /// Implementação de <see cref="IAdminApiKeyAppService"/>.
    /// </summary>
    [AbpAuthorize(MiddlewarePermissions.Pages_Administration_ApiKeys)]
    public class AdminApiKeyAppService : MiddlewareAppServiceBase, IAdminApiKeyAppService
    {
        private readonly IApiKeyManager _apiKeyManager;
        private readonly IRepository<ApiKey, long> _apiKeyRepository;
        private readonly UserManager _userManager;

        /// <summary>
        /// AdminApiKeyAppService.
        /// </summary>
        public AdminApiKeyAppService(
            IApiKeyManager apiKeyManager,
            IRepository<ApiKey, long> apiKeyRepository,
            UserManager userManager)
        {
            _apiKeyManager = apiKeyManager;
            _apiKeyRepository = apiKeyRepository;
            _userManager = userManager;
        }

        /// <inheritdoc />
        public async Task<ListResultDto<ApiKeyDto>> GetTenantApiKeys()
        {
            var keys = await _apiKeyRepository.GetAllListAsync(k => k.TenantId == AbpSession.TenantId);
            var result = new List<ApiKeyDto>();
            foreach (var k in keys)
            {
                var user = await _userManager.FindByIdAsync(k.UserId.ToString());
                result.Add(new ApiKeyDto
                {
                    Id = k.Id,
                    Name = k.Name,
                    KeyPrefix = k.KeyPrefix,
                    PermissionScopes = k.GetPermissionScopes(),
                    AllowedIpRanges = string.IsNullOrWhiteSpace(k.AllowedIpRanges)
                        ? null
                        : k.AllowedIpRanges.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                    ExpirationTime = k.ExpirationTime,
                    Status = k.Status,
                    LastUsedTime = k.LastUsedTime,
                    CreationTime = k.CreationTime,
                    UserId = k.UserId,
                    UserName = user?.UserName
                });
            }
            return new ListResultDto<ApiKeyDto>(result);
        }

        /// <inheritdoc />
        public async Task RevokeApiKey(EntityDto<long> input)
        {
            var key = await _apiKeyRepository.FirstOrDefaultAsync(input.Id);
            if (key == null || key.TenantId != AbpSession.TenantId)
            {
                throw new UserFriendlyException("API key not found.");
            }
            await _apiKeyManager.RevokeAsync(input.Id);
        }
    }
}
