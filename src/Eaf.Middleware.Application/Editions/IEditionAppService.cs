using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.Middleware.Editions.Dto;
using Eaf.Middleware.MultiTenancy.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Editions
{
    /// <summary>
    /// Contrato do serviço de aplicação para gerenciamento de Editions.
    /// </summary>
    public interface IEditionAppService : IApplicationService
    {
        /// <summary>
        /// Obtém as edições paginadas.
        /// </summary>
        /// <param name="input">Filtros e paginação.</param>
        /// <returns>Lista paginada de edições.</returns>
        Task<PagedResultDto<EditionDto>> GetEditions(GetEditionsInput input);

        /// <summary>
        /// Obtém uma edição para edição.
        /// </summary>
        /// <param name="input">Identificador da edição.</param>
        /// <returns>Edição encontrada.</returns>
        Task<EditionDto> GetEditionForEdit(EntityDto input);

        /// <summary>
        /// Cria uma nova edição.
        /// </summary>
        /// <param name="input">Dados da edição.</param>
        /// <returns>Task.</returns>
        Task CreateEdition(CreateEditionInput input);

        /// <summary>
        /// Atualiza uma edição existente.
        /// </summary>
        /// <param name="input">Dados da edição.</param>
        /// <returns>Task.</returns>
        Task UpdateEdition(UpdateEditionInput input);

        /// <summary>
        /// Remove uma edição.
        /// </summary>
        /// <param name="input">Identificador da edição.</param>
        /// <returns>Task.</returns>
        Task DeleteEdition(EntityDto input);

        /// <summary>
        /// Obtém as features da edição para edição.
        /// </summary>
        /// <param name="input">Identificador da edição.</param>
        /// <returns>Features e valores da edição.</returns>
        Task<GetEditionFeaturesEditOutput> GetEditionFeaturesForEdit(EntityDto input);

        /// <summary>
        /// Obtém todas as features cadastradas.
        /// </summary>
        /// <returns>Lista de features.</returns>
        Task<ListResultDto<FlatFeatureDto>> GetAllFeatures();

        /// <summary>
        /// Atualiza as features de uma edição.
        /// </summary>
        /// <param name="input">Identificador da edição e valores de features.</param>
        /// <returns>Task.</returns>
        Task UpdateEditionFeatures(UpdateEditionFeaturesInput input);
    }
}
