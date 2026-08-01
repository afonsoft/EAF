using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Core.Editions;
using Eaf.Middleware.Editions;
using Eaf.Middleware.Editions.Dto;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Editions
{
    /// <summary>
    /// Testes BDD para EditionAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class EditionAppServiceBddTests
    {
        private readonly EditionAppService _sut;
        private readonly IRepository<SubscribableEdition, int> _editionRepository;

        public EditionAppServiceBddTests()
        {
            _editionRepository = Substitute.For<IRepository<SubscribableEdition, int>>();
            _sut = new EditionAppService(_editionRepository, CreateEditionManager());
            _sut.ObjectMapper = CreateObjectMapper();
        }

        private static EditionManager CreateEditionManager()
        {
            var baseEditionRepository = Substitute.For<IRepository<Edition>>();
            var featureValueStore = Substitute.For<IAbpZeroFeatureValueStore>();
            var unitOfWorkManager = ManagerTestHelper.CreateUnitOfWorkManager();

            return Substitute.For<EditionManager>(new object[]
            {
                baseEditionRepository,
                featureValueStore,
                unitOfWorkManager
            });
        }

        private static IObjectMapper CreateObjectMapper()
        {
            var mapper = Substitute.For<IObjectMapper>();
            mapper.Map<EditionDto>(Arg.Any<SubscribableEdition>()).Returns(ci =>
            {
                var edition = ci.Arg<SubscribableEdition>();
                return new EditionDto
                {
                    Id = edition.Id,
                    DisplayName = edition.DisplayName,
                };
            });
            mapper.Map<List<EditionDto>>(Arg.Any<IEnumerable<SubscribableEdition>>()).Returns(ci =>
            {
                var editions = ci.Arg<IEnumerable<SubscribableEdition>>();
                return editions.Select(e => mapper.Map<EditionDto>(e)).ToList();
            });
            mapper.Map<SubscribableEdition>(Arg.Any<CreateEditionInput>()).Returns(ci =>
            {
                var input = ci.Arg<CreateEditionInput>();
                return new SubscribableEdition
                {
                    DisplayName = input.DisplayName,
                };
            });
            mapper.Map(Arg.Any<UpdateEditionInput>(), Arg.Any<SubscribableEdition>()).Returns(ci =>
            {
                var input = ci.Arg<UpdateEditionInput>();
                var edition = ci.Arg<SubscribableEdition>();
                edition.DisplayName = input.DisplayName;
                return edition;
            });
            mapper.Map<List<FlatFeatureDto>>(Arg.Any<IEnumerable<Feature>>()).Returns(ci =>
            {
                var features = ci.Arg<IEnumerable<Feature>>();
                return features.Select(f => new FlatFeatureDto
                {
                    Name = f.Name,
                }).ToList();
            });
            return mapper;
        }

        #region GetAllFeatures

        [Fact]
        public async Task Dado_FeaturesCadastradas_Quando_GetAllFeatures_Entao_DeveRetornarListaOrdenada()
        {
            // Dado
            var feature = new TestFeature("FeatureB");

            var featureManager = Substitute.For<IFeatureManager>();
            featureManager.GetAll().Returns(new List<Feature> { feature });
            _sut.FeatureManager = featureManager;

            // Quando
            var result = await _sut.GetAllFeatures();

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
            result.Items[0].Name.ShouldBe("FeatureB");
        }

        #endregion

        #region Construtor

        [Fact]
        public void Dado_RepositorioEdition_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region GetEditions

        [Fact]
        public async Task Dado_EditionsCadastradas_Quando_GetEditions_Entao_DeveRetornarListaPaginada()
        {
            // Dado
            var editions = new List<SubscribableEdition>
            {
                new SubscribableEdition { Id = 1, DisplayName = "Free" },
                new SubscribableEdition { Id = 2, DisplayName = "Pro" },
            }.AsAsyncQueryable();

            _editionRepository.GetAllAsync().Returns(editions);

            // Quando
            var result = await _sut.GetEditions(new GetEditionsInput { MaxResultCount = 10, SkipCount = 0 });

            // Então
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items[0].DisplayName.ShouldBe("Free");
            result.Items[1].DisplayName.ShouldBe("Pro");
        }

        [Fact]
        public async Task Dado_FiltroPeloNome_Quando_GetEditions_Entao_DeveFiltrarPorDisplayName()
        {
            // Dado
            var editions = new List<SubscribableEdition>
            {
                new SubscribableEdition { Id = 1, DisplayName = "Free" },
                new SubscribableEdition { Id = 2, DisplayName = "Pro" },
            }.AsAsyncQueryable();

            _editionRepository.GetAllAsync().Returns(editions);

            // Quando
            var result = await _sut.GetEditions(new GetEditionsInput { Filter = "Pro", MaxResultCount = 10, SkipCount = 0 });

            // Então
            result.TotalCount.ShouldBe(1);
            result.Items[0].DisplayName.ShouldBe("Pro");
        }

        #endregion

        #region GetEditionForEdit

        [Fact]
        public async Task Dado_EditionExistente_Quando_GetEditionForEdit_Entao_DeveRetornarEditionDto()
        {
            // Dado
            var edition = new SubscribableEdition { Id = 1, DisplayName = "Free" };
            _editionRepository.GetAsync(1).Returns(edition);

            // Quando
            var result = await _sut.GetEditionForEdit(new EntityDto(1));

            // Então
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.DisplayName.ShouldBe("Free");
        }

        #endregion

        #region CreateEdition

        [Fact]
        public async Task Dado_InputValido_Quando_CreateEdition_Entao_DeveInserirNoRepositorio()
        {
            // Dado
            var input = new CreateEditionInput
            {
                DisplayName = "Enterprise",
            };

            // Quando
            await _sut.CreateEdition(input);

            // Então
            await _editionRepository.Received(1).InsertAsync(Arg.Is<SubscribableEdition>(e => e.DisplayName == "Enterprise"));
        }

        #endregion

        #region UpdateEdition

        [Fact]
        public async Task Dado_InputValido_Quando_UpdateEdition_Entao_DeveAtualizarNoRepositorio()
        {
            // Dado
            var edition = new SubscribableEdition { Id = 1, DisplayName = "Old" };
            _editionRepository.GetAsync(1).Returns(edition);

            var input = new UpdateEditionInput
            {
                Id = 1,
                DisplayName = "Updated",
            };

            // Quando
            await _sut.UpdateEdition(input);

            // Então
            await _editionRepository.Received(1).UpdateAsync(edition);
            edition.DisplayName.ShouldBe("Updated");
        }

        #endregion

        #region DeleteEdition

        [Fact]
        public async Task Dado_EditionExistente_Quando_DeleteEdition_Entao_DeveRemoverDoRepositorio()
        {
            // Dado
            _editionRepository.GetAsync(1).Returns(new SubscribableEdition { Id = 1, DisplayName = "ToDelete" });

            // Quando
            await _sut.DeleteEdition(new EntityDto(1));

            // Então
            await _editionRepository.Received(1).DeleteAsync(1);
        }

        #endregion
    }

    public class TestFeature : Feature
    {
        public TestFeature(string name)
            : base(name, defaultValue: "true")
        {
        }
    }
}
