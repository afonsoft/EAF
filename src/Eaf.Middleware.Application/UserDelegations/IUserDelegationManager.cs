using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.UserDelegations
{
    /// <summary>
    /// Interface do gerenciador de delegações de usuário.
    /// </summary>
    public interface IUserDelegationManager
    {
        /// <summary>
        /// Valida as datas de uma delegação.
        /// </summary>
        void ValidateDates(DateTime startTime, DateTime endTime);

        /// <summary>
        /// Obtém uma delegação ativa entre dois usuários.
        /// </summary>
        Task<UserDelegation> GetActiveDelegationAsync(long sourceUserId, long targetUserId);

        /// <summary>
        /// Indica se existe uma delegação ativa do usuário origem para o destino.
        /// </summary>
        Task<bool> IsDelegationValidAsync(long sourceUserId, long targetUserId);
    }
}
