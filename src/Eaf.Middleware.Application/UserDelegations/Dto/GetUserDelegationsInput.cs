using Abp.Application.Services.Dto;

namespace Eaf.Middleware.UserDelegations.Dto
{
    /// <summary>
    /// Entrada para listar delegações de usuário.
    /// </summary>
    public class GetUserDelegationsInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Identificador do usuário que delegou. Null para todos.
        /// </summary>
        public long? SourceUserId { get; set; }

        /// <summary>
        /// Identificador do usuário destino. Null para todos.
        /// </summary>
        public long? TargetUserId { get; set; }
    }
}
