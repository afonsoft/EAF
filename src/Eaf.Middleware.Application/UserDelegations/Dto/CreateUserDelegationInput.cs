using System;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.UserDelegations.Dto
{
    /// <summary>
    /// Entrada para criação de uma delegação de usuário.
    /// </summary>
    public class CreateUserDelegationInput
    {
        /// <summary>
        /// Identificador do usuário destino.
        /// </summary>
        public long TargetUserId { get; set; }

        /// <summary>
        /// Data/hora inicial.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Data/hora final.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Descrição opcional.
        /// </summary>
        [StringLength(UserDelegation.MaxDescriptionLength)]
        public string Description { get; set; }
    }
}
