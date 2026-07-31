using Abp.Application.Services.Dto;

namespace Eaf.Middleware.Editions.Dto
{
    /// <summary>
    /// Entrada para atualização de uma Edition.
    /// </summary>
    public class UpdateEditionInput : CreateEditionInput, IEntityDto<int>
    {
        /// <summary>
        /// Identificador da edição.
        /// </summary>
        public int Id { get; set; }
    }
}
