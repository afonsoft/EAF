using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authentication.ApiKeys
{
    /// <summary>
    /// Implementação de <see cref="IApiKeyManager"/>.
    /// Gera material aleatório de 256 bits, persiste apenas SHA-256 e valida IP/CIDR.
    /// </summary>
    public class ApiKeyManager : DomainService, IApiKeyManager
    {
        private readonly IRepository<ApiKey, long> _apiKeyRepository;

        /// <summary>
        /// ApiKeyManager.
        /// </summary>
        public ApiKeyManager(IRepository<ApiKey, long> apiKeyRepository)
        {
            _apiKeyRepository = apiKeyRepository;
        }

        /// <inheritdoc />
        public async Task<(ApiKey Entity, string PlainKey)> CreateAsync(
            UserIdentifier user,
            string name,
            string[] permissionScopes = null,
            string[] allowedIpRanges = null,
            DateTime? expirationTime = null)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new UserFriendlyException("API key name is required.");
            }
            if (allowedIpRanges != null && allowedIpRanges.Length > 0)
            {
                foreach (var range in allowedIpRanges)
                {
                    if (!TryParseIpOrCidr(range, out _, out _))
                    {
                        throw new UserFriendlyException($"Invalid IP or CIDR range: {range}");
                    }
                }
            }

            var plainKey = GeneratePlainKey();

            var entity = new ApiKey
            {
                TenantId = user.TenantId,
                UserId = user.UserId,
                Name = name.Trim(),
                KeyPrefix = plainKey[..ApiKeyConsts.DisplayPrefixLength],
                KeyHash = ComputeHash(plainKey),
                AllowedIpRanges = allowedIpRanges == null || allowedIpRanges.Length == 0
                    ? null
                    : string.Join(",", allowedIpRanges.Select(r => r.Trim())),
                ExpirationTime = expirationTime,
                Status = ApiKeyStatus.Active
            };
            entity.SetPermissionScopes(permissionScopes);

            await _apiKeyRepository.InsertAsync(entity);
            return (entity, plainKey);
        }

        /// <inheritdoc />
        public async Task<ApiKeyValidationResult> ValidateAsync(string plainKey, IPAddress remoteIp)
        {
            if (string.IsNullOrWhiteSpace(plainKey) || !plainKey.StartsWith(ApiKeyConsts.KeyPrefix, StringComparison.Ordinal))
            {
                return ApiKeyValidationResult.Fail(ApiKeyValidationError.KeyNotFound);
            }

            var hash = ComputeHash(plainKey);
            var apiKey = await _apiKeyRepository.FirstOrDefaultAsync(k => k.KeyHash == hash);
            if (apiKey == null)
            {
                return ApiKeyValidationResult.Fail(ApiKeyValidationError.KeyNotFound);
            }

            if (apiKey.Status == ApiKeyStatus.Revoked)
            {
                return ApiKeyValidationResult.Fail(ApiKeyValidationError.Revoked, apiKey);
            }

            var now = DateTime.UtcNow;
            if (apiKey.ExpirationTime.HasValue && apiKey.ExpirationTime.Value <= now)
            {
                return ApiKeyValidationResult.Fail(ApiKeyValidationError.Expired, apiKey);
            }

            if (!IsIpAllowed(apiKey, remoteIp))
            {
                return ApiKeyValidationResult.Fail(ApiKeyValidationError.IpNotAllowed, apiKey);
            }

            apiKey.LastUsedTime = now;
            await _apiKeyRepository.UpdateAsync(apiKey);

            return ApiKeyValidationResult.Success(apiKey);
        }

        /// <inheritdoc />
        public async Task RevokeAsync(long apiKeyId)
        {
            var apiKey = await _apiKeyRepository.FirstOrDefaultAsync(apiKeyId)
                ?? throw new UserFriendlyException("API key not found.");
            apiKey.Status = ApiKeyStatus.Revoked;
            await _apiKeyRepository.UpdateAsync(apiKey);
        }

        /// <inheritdoc />
        public async Task<(ApiKey Entity, string PlainKey)> RotateAsync(long apiKeyId)
        {
            var old = await _apiKeyRepository.FirstOrDefaultAsync(apiKeyId)
                ?? throw new UserFriendlyException("API key not found.");
            old.Status = ApiKeyStatus.Revoked;
            await _apiKeyRepository.UpdateAsync(old);

            var scopes = old.GetPermissionScopes();
            var ranges = string.IsNullOrWhiteSpace(old.AllowedIpRanges)
                ? null
                : old.AllowedIpRanges.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return await CreateAsync(
                new UserIdentifier(old.TenantId, old.UserId),
                old.Name,
                scopes,
                ranges,
                old.ExpirationTime);
        }

        /// <inheritdoc />
        public bool IsIpAllowed(ApiKey apiKey, IPAddress remoteIp)
        {
            if (apiKey == null || string.IsNullOrWhiteSpace(apiKey.AllowedIpRanges))
            {
                return true;
            }
            if (remoteIp == null)
            {
                return false;
            }

            foreach (var range in apiKey.AllowedIpRanges.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (IsIpInRange(remoteIp, range))
                {
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc />
        public bool IsInScope(ApiKey apiKey, string permissionName)
        {
            var scopes = apiKey.GetPermissionScopes();
            return scopes == null || scopes.Contains(permissionName);
        }

        /// <summary>
        /// Gera o material secreto da chave: <c>eak_</c> + base64url de 32 bytes aleatórios.
        /// </summary>
        public static string GeneratePlainKey()
        {
            var bytes = RandomNumberGenerator.GetBytes(ApiKeyConsts.KeyBytesLength);
            return ApiKeyConsts.KeyPrefix + Base64UrlEncode(bytes);
        }

        /// <summary>
        /// Calcula o hash SHA-256 hexadecimal (lowercase) da chave em claro.
        /// </summary>
        public static string ComputeHash(string plainKey)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plainKey));
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Verifica se <paramref name="remoteIp"/> pertence a um IP exato ou bloco CIDR.
        /// </summary>
        public static bool IsIpInRange(IPAddress remoteIp, string range)
        {
            if (!TryParseIpOrCidr(range, out var network, out var prefixLength))
            {
                return false;
            }
            if (remoteIp.AddressFamily != network.AddressFamily)
            {
                return false;
            }
            if (prefixLength < 0)
            {
                return remoteIp.Equals(network);
            }

            var ipBytes = remoteIp.GetAddressBytes();
            var networkBytes = network.GetAddressBytes();
            var fullBytes = prefixLength / 8;
            var remainingBits = prefixLength % 8;

            for (var i = 0; i < fullBytes; i++)
            {
                if (ipBytes[i] != networkBytes[i])
                {
                    return false;
                }
            }
            if (remainingBits == 0)
            {
                return true;
            }
            var mask = (byte)(0xFF << (8 - remainingBits));
            return (ipBytes[fullBytes] & mask) == (networkBytes[fullBytes] & mask);
        }

        private static bool TryParseIpOrCidr(string range, out IPAddress network, out int prefixLength)
        {
            network = null;
            prefixLength = -1;
            if (string.IsNullOrWhiteSpace(range))
            {
                return false;
            }

            var parts = range.Split('/', StringSplitOptions.TrimEntries);
            if (!IPAddress.TryParse(parts[0], out network))
            {
                return false;
            }

            if (parts.Length == 1)
            {
                return true;
            }
            if (parts.Length != 2 || !int.TryParse(parts[1], out prefixLength))
            {
                return false;
            }

            var maxBits = network.AddressFamily == AddressFamily.InterNetwork ? 32 : 128;
            return prefixLength >= 0 && prefixLength <= maxBits;
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
