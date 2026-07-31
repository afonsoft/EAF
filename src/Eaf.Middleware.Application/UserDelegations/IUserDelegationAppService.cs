using Abp.Application.Services.Dto;
using Eaf.Middleware.UserDelegations.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.UserDelegations
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de delegações de usuário.
    /// </summary>
    public interface IUserDelegationAppService
    {
        /// <summary>
        /// Obtém as delegações do usuário atual como origem.
        /// </summary>
        Task<ListResultDto<UserDelegationDto>> GetMyDelegationsAsync(GetUserDelegationsInput input);

        /// <summary>
        /// Obtém as delegações ativas em que o usuário atual é destino.
        /// </summary>
        Task<ListResultDto<UserDelegationDto>> GetDelegatedUsersAsync(GetUserDelegationsInput input);

        /// <summary>
        /// Cria uma nova delegação de usuário.
        /// </summary>
        Task<UserDelegationDto> CreateAsync(CreateUserDelegationInput input);

        /// <summary>
        /// Cancela (remove) uma delegação.
        /// </summary>
        Task CancelAsync(EntityDto<long> input);
    }
}
