using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authentication.Passkeys
{
    /// <summary>
    /// Operações de persistência e consulta de passkeys.
    /// </summary>
    public interface IPasskeyManager
    {
        /// <summary>Lista os passkeys de um usuário (mais recentes primeiro).</summary>
        Task<List<UserPasskey>> GetForUserAsync(UserIdentifier user);

        /// <summary>Insere um passkey já verificado pela cerimônia WebAuthn.</summary>
        Task<UserPasskey> AddAsync(UserPasskey passkey);

        /// <summary>Busca por credentialId (base64url).</summary>
        Task<UserPasskey> FindByCredentialIdAsync(string credentialId);

        /// <summary>Remove um passkey do usuário informado; falha se não pertencer a ele.</summary>
        Task DeleteAsync(UserIdentifier user, long passkeyId);

        /// <summary>Persiste a entidade (sign count / last used).</summary>
        Task UpdateAsync(UserPasskey passkey);
    }

    /// <summary>
    /// Implementação de <see cref="IPasskeyManager"/>.
    /// </summary>
    public class PasskeyManager : DomainService, IPasskeyManager
    {
        private readonly IRepository<UserPasskey, long> _passkeyRepository;

        /// <summary>
        /// PasskeyManager.
        /// </summary>
        public PasskeyManager(IRepository<UserPasskey, long> passkeyRepository)
        {
            _passkeyRepository = passkeyRepository;
        }

        /// <inheritdoc />
        public async Task<List<UserPasskey>> GetForUserAsync(UserIdentifier user)
        {
            var keys = await _passkeyRepository.GetAllListAsync(
                p => p.UserId == user.UserId && p.TenantId == user.TenantId);
            return keys.OrderByDescending(p => p.CreationTime).ToList();
        }

        /// <inheritdoc />
        public async Task<UserPasskey> AddAsync(UserPasskey passkey)
        {
            await _passkeyRepository.InsertAsync(passkey);
            return passkey;
        }

        /// <inheritdoc />
        public Task<UserPasskey> FindByCredentialIdAsync(string credentialId)
        {
            return _passkeyRepository.FirstOrDefaultAsync(p => p.CredentialId == credentialId);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(UserIdentifier user, long passkeyId)
        {
            var passkey = await _passkeyRepository.FirstOrDefaultAsync(passkeyId)
                ?? throw new UserFriendlyException("Passkey not found.");
            if (passkey.UserId != user.UserId || passkey.TenantId != user.TenantId)
            {
                throw new UserFriendlyException("Passkey not found.");
            }
            await _passkeyRepository.DeleteAsync(passkey);
        }

        /// <inheritdoc />
        public Task UpdateAsync(UserPasskey passkey)
        {
            return _passkeyRepository.UpdateAsync(passkey);
        }
    }
}
