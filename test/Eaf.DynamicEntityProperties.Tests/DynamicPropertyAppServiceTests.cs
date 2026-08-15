using Abp.DynamicEntityProperties;
using Eaf.DynamicEntityProperties.Application;
using Eaf.DynamicEntityProperties.Application.Dto;
using Eaf.DynamicEntityProperties.Authorization;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.DynamicEntityProperties.Tests
{
    /// <summary>
    /// Testes do serviço de aplicação DynamicPropertyAppService.
    /// </summary>
    public class DynamicPropertyAppServiceTests : EafDynamicEntityPropertiesTestBase
    {
        private readonly DynamicPropertyAppService _dynamicPropertyAppService;
        private readonly IDynamicPropertyManager _dynamicPropertyManager;
        private readonly IDynamicPropertyStore _dynamicPropertyStore;
        private readonly IDynamicPropertyValueManager _dynamicPropertyValueManager;

        public DynamicPropertyAppServiceTests()
        {
            _dynamicPropertyAppService = Resolve<DynamicPropertyAppService>();
            _dynamicPropertyManager = Resolve<IDynamicPropertyManager>();
            _dynamicPropertyStore = Resolve<IDynamicPropertyStore>();
            _dynamicPropertyValueManager = Resolve<IDynamicPropertyValueManager>();
        }

        [Fact]
        public async Task Dado_UmaPropriedade_Quando_Criar_Entao_DeveRetornarDtoComIdPreenchido()
        {
            // Arrange
            _dynamicPropertyManager.AddAsync(Arg.Any<DynamicProperty>())
                .Returns(callInfo =>
                {
                    var property = callInfo.Arg<DynamicProperty>();
                    property.Id = 42;
                    return Task.FromResult(property);
                });

            _dynamicPropertyValueManager.GetAllValuesOfDynamicPropertyAsync(Arg.Any<int>())
                .Returns(Task.FromResult(new List<DynamicPropertyValue>()));

            var input = new CreateOrUpdateDynamicPropertyInput
            {
                PropertyName = "City",
                DisplayName = "Cidade",
                InputType = "SingleLineStringInputType",
                Values = new List<DynamicPropertyValueDto>
                {
                    new DynamicPropertyValueDto { Value = "São Paulo" }
                }
            };

            // Act
            var result = await _dynamicPropertyAppService.CreateAsync(input);

            // Assert
            result.Id.ShouldBe(42);
            result.PropertyName.ShouldBe("City");
            result.DisplayName.ShouldBe("Cidade");
            await _dynamicPropertyValueManager.Received(1).AddAsync(Arg.Any<DynamicPropertyValue>());
        }

        [Fact]
        public async Task Dado_PropriedadesCadastradas_Quando_Listar_Entao_DeveRetornarLista()
        {
            // Arrange
            _dynamicPropertyStore.GetAllAsync()
                .Returns(Task.FromResult(new List<DynamicProperty>
                {
                    new DynamicProperty { Id = 1, PropertyName = "City", InputType = "SingleLineStringInputType" },
                    new DynamicProperty { Id = 2, PropertyName = "Gender", InputType = "ComboboxInputType" }
                }));

            // Act
            var result = await _dynamicPropertyAppService.GetAllAsync();

            // Assert
            result.Items.Count.ShouldBe(2);
        }

        [Fact]
        public async Task Dado_UmaPropriedadeExistente_Quando_Atualizar_Entao_DeveRetornarDtoAtualizado()
        {
            // Arrange
            var existing = new DynamicProperty
            {
                Id = 1,
                PropertyName = "City",
                DisplayName = "Cidade",
                InputType = "SingleLineStringInputType"
            };

            _dynamicPropertyManager.GetAsync(1).Returns(Task.FromResult(existing));
            _dynamicPropertyManager.UpdateAsync(Arg.Any<DynamicProperty>())
                .Returns(callInfo => Task.FromResult(callInfo.Arg<DynamicProperty>()));
            _dynamicPropertyValueManager.GetAllValuesOfDynamicPropertyAsync(Arg.Any<int>())
                .Returns(Task.FromResult(new List<DynamicPropertyValue>()));

            var input = new CreateOrUpdateDynamicPropertyInput
            {
                Id = 1,
                PropertyName = "CityUpdated",
                DisplayName = "Cidade Atualizada",
                InputType = "ComboboxInputType",
                Values = new List<DynamicPropertyValueDto>()
            };

            // Act
            var result = await _dynamicPropertyAppService.UpdateAsync(input);

            // Assert
            result.PropertyName.ShouldBe("CityUpdated");
            result.InputType.ShouldBe("ComboboxInputType");
            await _dynamicPropertyValueManager.Received(1).CleanValuesAsync(1);
        }

        [Fact]
        public async Task Dado_UmaPropriedade_Quando_Excluir_Entao_DeveChamarDelete()
        {
            // Act
            await _dynamicPropertyAppService.DeleteAsync(10);

            // Assert
            await _dynamicPropertyManager.Received(1).DeleteAsync(10);
        }

        [Fact]
        public void Dado_NomesDePermissoes_Entao_DevemEstarPrefixadosPorPagesAdministration()
        {
            // Assert
            EafDynamicEntityPropertiesPermissions.DynamicProperties.ShouldStartWith("Pages.Administration");
            EafDynamicEntityPropertiesPermissions.DynamicProperties_Create.ShouldStartWith("Pages.Administration");
        }
    }
}
