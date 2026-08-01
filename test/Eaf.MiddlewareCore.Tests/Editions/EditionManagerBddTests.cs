using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Eaf.Middleware.Core.Editions;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Editions
{
    /// <summary>
    /// Testes BDD para EditionManager seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class EditionManagerBddTests
    {
        private readonly IRepository<Edition> _editionRepository;
        private readonly IAbpZeroFeatureValueStore _featureValueStore;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly EditionManager _sut;

        public EditionManagerBddTests()
        {
            _editionRepository = Substitute.For<IRepository<Edition>>();
            _featureValueStore = Substitute.For<IAbpZeroFeatureValueStore>();
            _unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            _sut = new EditionManager(_editionRepository, _featureValueStore, _unitOfWorkManager);
        }

        #region Constantes

        [Fact]
        public void Dado_EditionManager_Quando_VerificarDefaultEditionName_Entao_DeveSerFree()
        {
            EditionManager.DefaultEditionName.ShouldBe("Free");
        }

        #endregion

        #region GetAllAsync

        [Fact]
        public async Task Dado_EdicoesExistentes_Quando_GetAllAsync_Entao_DeveRetornarTodasEdicoes()
        {
            // Dado
            var editions = new List<Edition>
            {
                new Edition { DisplayName = "Standard" },
                new Edition { DisplayName = "Premium" }
            };
            _editionRepository.GetAllListAsync().Returns(editions);

            // Quando
            var result = await _sut.GetAllAsync();

            // Entao
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
        }

        [Fact]
        public async Task Dado_NenhumaEdicao_Quando_GetAllAsync_Entao_DeveRetornarListaVazia()
        {
            // Dado
            _editionRepository.GetAllListAsync().Returns(new List<Edition>());

            // Quando
            var result = await _sut.GetAllAsync();

            // Entao
            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
            _sut.ShouldBeAssignableTo<AbpEditionManager>();
        }

        #endregion
    }
}
