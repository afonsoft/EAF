using Abp.Application.Editions;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Core.Editions;
using Eaf.Middleware.Editions.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Eaf.Middleware.Editions
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de Editions.
    /// </summary>
    [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Editions)]
    public class EditionAppService : MiddlewareAppServiceBase, IEditionAppService
    {
        private readonly IRepository<SubscribableEdition, int> _editionRepository;
        private readonly EditionManager _editionManager;

        /// <summary>
        /// EditionAppService.
        /// </summary>
        /// <param name="editionRepository">Repositório de edições.</param>
        /// <param name="editionManager">Gerenciador de edições.</param>
        public EditionAppService(IRepository<SubscribableEdition, int> editionRepository, EditionManager editionManager)
        {
            _editionRepository = editionRepository;
            _editionManager = editionManager;
        }

        /// <summary>
        /// Obtém as edições paginadas.
        /// </summary>
        /// <param name="input">Filtros e paginação.</param>
        /// <returns>Lista paginada de edições.</returns>
        public async Task<PagedResultDto<EditionDto>> GetEditions(GetEditionsInput input)
        {
            var query = (await _editionRepository.GetAllAsync())
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(), e => e.DisplayName.Contains(input.Filter));

            var total = await query.CountAsync();
            var ordered = DynamicQueryableExtensions.OrderBy(query, input.Sorting ?? "DisplayName");
            var editions = await ordered.PageBy(input).ToListAsync();

            return new PagedResultDto<EditionDto>(total, ObjectMapper.Map<List<EditionDto>>(editions));
        }

        /// <summary>
        /// Obtém uma edição para edição.
        /// </summary>
        /// <param name="input">Identificador da edição.</param>
        /// <returns>Edição encontrada.</returns>
        public async Task<EditionDto> GetEditionForEdit(EntityDto input)
        {
            var edition = await _editionRepository.GetAsync(input.Id);
            return ObjectMapper.Map<EditionDto>(edition);
        }

        /// <summary>
        /// Cria uma nova edição.
        /// </summary>
        /// <param name="input">Dados da edição.</param>
        /// <returns>Task.</returns>
        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Editions_Create)]
        public async Task CreateEdition(CreateEditionInput input)
        {
            var edition = ObjectMapper.Map<SubscribableEdition>(input);
            await _editionRepository.InsertAsync(edition);
        }

        /// <summary>
        /// Atualiza uma edição existente.
        /// </summary>
        /// <param name="input">Dados da edição.</param>
        /// <returns>Task.</returns>
        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Editions_Edit)]
        public async Task UpdateEdition(UpdateEditionInput input)
        {
            var edition = await _editionRepository.GetAsync(input.Id);
            ObjectMapper.Map(input, edition);
            await _editionRepository.UpdateAsync(edition);
        }

        /// <summary>
        /// Remove uma edição.
        /// </summary>
        /// <param name="input">Identificador da edição.</param>
        /// <returns>Task.</returns>
        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Editions_Delete)]
        public async Task DeleteEdition(EntityDto input)
        {
            await _editionRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// Obtém as features da edição para edição.
        /// </summary>
        /// <param name="input">Identificador da edição.</param>
        /// <returns>Features e valores da edição.</returns>
        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Editions_Features)]
        public async Task<GetEditionFeaturesEditOutput> GetEditionFeaturesForEdit(EntityDto input)
        {
            var features = FeatureManager.GetAll()
                .Where(f => f.Scope.HasFlag(Abp.Application.Features.FeatureScopes.Edition));
            var featureValues = await _editionManager.GetFeatureValuesAsync(input.Id);

            return new GetEditionFeaturesEditOutput
            {
                Features = ObjectMapper.Map<List<FlatFeatureDto>>(features).OrderBy(f => f.DisplayName).ToList(),
                FeatureValues = featureValues.Select(fv => new NameValueDto(fv)).ToList()
            };
        }

        /// <summary>
        /// Atualiza as features de uma edição.
        /// </summary>
        /// <param name="input">Identificador da edição e valores de features.</param>
        /// <returns>Task.</returns>
        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Editions_Features)]
        public async Task UpdateEditionFeatures(UpdateEditionFeaturesInput input)
        {
            await _editionManager.SetFeatureValuesAsync(
                input.Id,
                input.FeatureValues.Select(fv => new Abp.NameValue(fv.Name, fv.Value)).ToArray());
        }
    }
}
