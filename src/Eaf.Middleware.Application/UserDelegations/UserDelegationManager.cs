using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Timing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.UserDelegations
{
    /// <summary>
    /// Gerenciador de domínio para delegações de usuário.
    /// </summary>
    public class UserDelegationManager : DomainService, IUserDelegationManager
    {
        private readonly IRepository<UserDelegation, long> _userDelegationRepository;

        /// <summary>
        /// UserDelegationManager.
        /// </summary>
        public UserDelegationManager(IRepository<UserDelegation, long> userDelegationRepository)
        {
            _userDelegationRepository = userDelegationRepository;
        }

        /// <summary>
        /// Valida as datas de uma delegação.
        /// </summary>
        public virtual void ValidateDates(DateTime startTime, DateTime endTime)
        {
            if (startTime >= endTime)
            {
                throw new ArgumentException("End time must be greater than start time.");
            }

            if (startTime < Clock.Now.Date)
            {
                throw new ArgumentException("Start time cannot be in the past.");
            }
        }

        /// <summary>
        /// Obtém uma delegação ativa entre dois usuários.
        /// </summary>
        [UnitOfWork]
        public virtual async Task<UserDelegation> GetActiveDelegationAsync(long sourceUserId, long targetUserId)
        {
            var now = Clock.Now;
            return await _userDelegationRepository.FirstOrDefaultAsync(d =>
                d.SourceUserId == sourceUserId &&
                d.TargetUserId == targetUserId &&
                !d.IsDeleted &&
                d.StartTime <= now &&
                d.EndTime >= now);
        }

        /// <summary>
        /// Indica se existe uma delegação ativa do usuário origem para o destino.
        /// </summary>
        public virtual async Task<bool> IsDelegationValidAsync(long sourceUserId, long targetUserId)
        {
            var delegation = await GetActiveDelegationAsync(sourceUserId, targetUserId);
            return delegation != null;
        }
    }
}
