using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Core.Editions
{
    /// <summary>
    /// Representa a classe EditionManager.
    /// </summary>
    public class EditionManager : AbpEditionManager
    {
        public const string DefaultEditionName = "Standard";

        /// <summary>
        /// EditionManager.
        /// </summary>
        /// <param name="editionRepository">Parâmetro editionRepository.</param>
        /// <param name="featureValueStore">Parâmetro featureValueStore.</param>
        /// <param name="unitOfWorkManager">Parâmetro unitOfWorkManager.</param>
        /// <returns>Resultado da operação.</returns>
        public EditionManager(IRepository<Edition> editionRepository,
            IAbpZeroFeatureValueStore featureValueStore,
            IUnitOfWorkManager unitOfWorkManager)
            : base(editionRepository,
                  featureValueStore,
                  unitOfWorkManager)
        {
        }

        /// <summary>
        /// GetAllAsync.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<List<Edition>> GetAllAsync()

        {
            return await EditionRepository.GetAllListAsync();
        }
    }
}