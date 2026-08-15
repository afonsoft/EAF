using Abp.DynamicEntityProperties;
using Eaf.DynamicEntityProperties.Application;
using Eaf.DynamicEntityProperties.Application.Dto;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.DynamicEntityProperties.Tests
{
    /// <summary>
    /// Testes do serviço de aplicação DynamicEntityPropertyValueAppService.
    /// </summary>
    public class DynamicEntityPropertyValueAppServiceTests : EafDynamicEntityPropertiesTestBase
    {
        private readonly DynamicEntityPropertyValueAppService _dynamicEntityPropertyValueAppService;
        private readonly IDynamicEntityPropertyValueManager _dynamicEntityPropertyValueManager;

        public DynamicEntityPropertyValueAppServiceTests()
        {
            _dynamicEntityPropertyValueAppService = Resolve<DynamicEntityPropertyValueAppService>();
            _dynamicEntityPropertyValueManager = Resolve<IDynamicEntityPropertyValueManager>();
        }

        [Fact]
        public async Task Dado_UmValorDePropriedade_Quando_Criar_Entao_DeveRetornarDto()
        {
            // Arrange
            _dynamicEntityPropertyValueManager.AddAsync(Arg.Any<DynamicEntityPropertyValue>())
                .Returns(callInfo =>
                {
                    var value = callInfo.Arg<DynamicEntityPropertyValue>();
                    value.Id = 99;
                    return Task.FromResult(value);
                });

            var input = new CreateOrUpdateDynamicEntityPropertyValueInput
            {
                EntityId = "42",
                DynamicEntityPropertyId = 7,
                Value = "São Paulo"
            };

            // Act
            var result = await _dynamicEntityPropertyValueAppService.CreateAsync(input);

            // Assert
            result.Id.ShouldBe(99);
            result.Value.ShouldBe("São Paulo");
        }

        [Fact]
        public async Task Dado_ValoresExistentes_Quando_ListarPorEntidade_Entao_DeveRetornarLista()
        {
            // Arrange
            _dynamicEntityPropertyValueManager.GetValuesAsync(
                    Arg.Any<int>(),
                    Arg.Any<string>())
                .Returns(Task.FromResult(new List<DynamicEntityPropertyValue>
                {
                    new DynamicEntityPropertyValue
                    {
                        Id = 1,
                        EntityId = "42",
                        Value = "São Paulo",
                        DynamicEntityPropertyId = 7
                    }
                }));

            var input = new GetDynamicEntityPropertyValuesInput
            {
                EntityFullName = "Eaf.Authorization.Users.User",
                EntityId = "42",
                DynamicEntityPropertyId = 7,
                DynamicPropertyId = 5,
                PropertyName = "City"
            };

            // Act
            var result = await _dynamicEntityPropertyValueAppService.GetAllAsync(input);

            // Assert
            result.Items.Count.ShouldBe(1);
            result.Items[0].Value.ShouldBe("São Paulo");
        }

        [Fact]
        public async Task Dado_UmValor_Quando_Atualizar_Entao_DeveRetornarDtoAtualizado()
        {
            // Arrange
            var existing = new DynamicEntityPropertyValue
            {
                Id = 5,
                EntityId = "42",
                DynamicEntityPropertyId = 7,
                Value = "Rio de Janeiro"
            };

            _dynamicEntityPropertyValueManager.GetAsync(5).Returns(Task.FromResult(existing));
            _dynamicEntityPropertyValueManager.UpdateAsync(Arg.Any<DynamicEntityPropertyValue>())
                .Returns(callInfo => Task.FromResult(callInfo.Arg<DynamicEntityPropertyValue>()));

            var input = new CreateOrUpdateDynamicEntityPropertyValueInput
            {
                Id = 5,
                EntityId = "42",
                DynamicEntityPropertyId = 7,
                Value = "Campinas"
            };

            // Act
            var result = await _dynamicEntityPropertyValueAppService.UpdateAsync(input);

            // Assert
            result.Value.ShouldBe("Campinas");
        }

        [Fact]
        public async Task Dado_UmValor_Quando_Excluir_Entao_DeveChamarDelete()
        {
            // Act
            await _dynamicEntityPropertyValueAppService.DeleteAsync(8);

            // Assert
            await _dynamicEntityPropertyValueManager.Received(1).DeleteAsync(8);
        }
    }
}
