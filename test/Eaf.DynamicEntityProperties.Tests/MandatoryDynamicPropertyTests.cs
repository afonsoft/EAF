using Abp.DynamicEntityProperties;
using Abp.UI;
using Eaf.DynamicEntityProperties.Application;
using Eaf.DynamicEntityProperties.Application.Dto;
using Eaf.DynamicEntityProperties.Domain;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.DynamicEntityProperties.Tests
{
    /// <summary>
    /// Testes da obrigatoriedade (IsMandatory) de dynamic properties.
    /// </summary>
    public class MandatoryDynamicPropertyTests : EafDynamicEntityPropertiesTestBase
    {
        private readonly DynamicEntityPropertyValueAppService _valueAppService;
        private readonly IDynamicEntityPropertyValueManager _valueManager;
        private readonly IDynamicEntityPropertyManager _entityPropertyManager;
        private readonly IDynamicPropertyManager _propertyManager;
        private readonly IDynamicPropertyMetadataManager _metadataManager;

        public MandatoryDynamicPropertyTests()
        {
            _valueAppService = Resolve<DynamicEntityPropertyValueAppService>();
            _valueManager = Resolve<IDynamicEntityPropertyValueManager>();
            _entityPropertyManager = Resolve<IDynamicEntityPropertyManager>();
            _propertyManager = Resolve<IDynamicPropertyManager>();
            _metadataManager = Resolve<IDynamicPropertyMetadataManager>();
        }

        private void SetupMandatory(int entityPropertyId = 7, int propertyId = 3, string propertyName = "CNPJ", bool mandatory = true)
        {
            _entityPropertyManager.GetAsync(entityPropertyId)
                .Returns(Task.FromResult(new DynamicEntityProperty { Id = entityPropertyId, DynamicPropertyId = propertyId }));
            _propertyManager.GetAsync(propertyId)
                .Returns(Task.FromResult(new DynamicProperty { Id = propertyId, PropertyName = propertyName }));
            _metadataManager.IsMandatoryAsync(Arg.Any<int?>(), propertyName)
                .Returns(Task.FromResult(mandatory));
        }

        [Fact]
        public async Task Dado_PropriedadeObrigatoria_Quando_SalvarSemValor_Entao_DeveFalhar()
        {
            // Arrange
            SetupMandatory();
            var input = new CreateOrUpdateDynamicEntityPropertyValueInput
            {
                EntityId = "42",
                DynamicEntityPropertyId = 7,
                Value = ""
            };

            // Act / Assert
            var ex = await Should.ThrowAsync<UserFriendlyException>(
                () => _valueAppService.CreateAsync(input));
            ex.Message.ShouldContain("CNPJ");
        }

        [Fact]
        public async Task Dado_PropriedadeObrigatoria_Quando_SalvarComValor_Entao_DevePassar()
        {
            // Arrange
            SetupMandatory();
            _valueManager.AddAsync(Arg.Any<DynamicEntityPropertyValue>())
                .Returns(callInfo => Task.FromResult(callInfo.Arg<DynamicEntityPropertyValue>()));

            var input = new CreateOrUpdateDynamicEntityPropertyValueInput
            {
                EntityId = "42",
                DynamicEntityPropertyId = 7,
                Value = "12.345.678/0001-90"
            };

            // Act
            var result = await _valueAppService.CreateAsync(input);

            // Assert
            result.Value.ShouldBe(input.Value);
        }

        [Fact]
        public async Task Dado_PropriedadeNaoObrigatoria_Quando_SalvarSemValor_Entao_DevePassar()
        {
            // Arrange
            SetupMandatory(mandatory: false);
            _valueManager.AddAsync(Arg.Any<DynamicEntityPropertyValue>())
                .Returns(callInfo => Task.FromResult(callInfo.Arg<DynamicEntityPropertyValue>()));

            var input = new CreateOrUpdateDynamicEntityPropertyValueInput
            {
                EntityId = "42",
                DynamicEntityPropertyId = 7,
                Value = ""
            };

            // Act
            await _valueAppService.CreateAsync(input);
        }

        [Fact]
        public async Task Dado_PropriedadeObrigatoria_Quando_AtualizarParaVazio_Entao_DeveFalhar()
        {
            // Arrange
            SetupMandatory();
            _valueManager.GetAsync(5)
                .Returns(Task.FromResult(new DynamicEntityPropertyValue
                {
                    Id = 5,
                    DynamicEntityPropertyId = 7,
                    Value = "antigo"
                }));

            // Act / Assert
            await Should.ThrowAsync<UserFriendlyException>(
                () => _valueAppService.UpdateAsync(new CreateOrUpdateDynamicEntityPropertyValueInput
                {
                    Id = 5,
                    Value = ""
                }));
        }

        [Fact]
        public async Task Dado_PropriedadeObrigatoria_Quando_Excluir_Entao_DeveFalhar()
        {
            // Arrange
            SetupMandatory();
            _valueManager.GetAsync(5)
                .Returns(Task.FromResult(new DynamicEntityPropertyValue
                {
                    Id = 5,
                    DynamicEntityPropertyId = 7,
                    Value = "antigo"
                }));

            // Act / Assert
            var ex = await Should.ThrowAsync<UserFriendlyException>(
                () => _valueAppService.DeleteAsync(5));
            ex.Message.ShouldContain("CNPJ");
            await _valueManager.DidNotReceive().DeleteAsync(Arg.Any<long>());
        }

        [Fact]
        public async Task Dado_PropriedadeNaoObrigatoria_Quando_Excluir_Entao_DevePassar()
        {
            // Arrange
            SetupMandatory(mandatory: false);
            _valueManager.GetAsync(5)
                .Returns(Task.FromResult(new DynamicEntityPropertyValue
                {
                    Id = 5,
                    DynamicEntityPropertyId = 7,
                    Value = "antigo"
                }));

            // Act
            await _valueAppService.DeleteAsync(5);

            // Assert
            await _valueManager.Received(1).DeleteAsync(5);
        }
    }
}
