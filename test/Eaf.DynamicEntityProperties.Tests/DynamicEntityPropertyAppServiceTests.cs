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
    /// Testes do serviço de aplicação DynamicEntityPropertyAppService.
    /// </summary>
    public class DynamicEntityPropertyAppServiceTests : EafDynamicEntityPropertiesTestBase
    {
        private readonly DynamicEntityPropertyAppService _dynamicEntityPropertyAppService;
        private readonly IDynamicEntityPropertyManager _dynamicEntityPropertyManager;

        public DynamicEntityPropertyAppServiceTests()
        {
            _dynamicEntityPropertyAppService = Resolve<DynamicEntityPropertyAppService>();
            _dynamicEntityPropertyManager = Resolve<IDynamicEntityPropertyManager>();
        }

        [Fact]
        public async Task Dado_UmaEntidade_Quando_CriarPropriedadeParaEntidade_Entao_DeveRetornarDto()
        {
            // Arrange
            _dynamicEntityPropertyManager.AddAsync(Arg.Any<DynamicEntityProperty>())
                .Returns(callInfo =>
                {
                    var entityProperty = callInfo.Arg<DynamicEntityProperty>();
                    entityProperty.Id = 7;
                    entityProperty.DynamicProperty = new DynamicProperty { Id = 5, PropertyName = "City" };
                    return Task.FromResult(entityProperty);
                });

            var input = new CreateDynamicEntityPropertyInput
            {
                EntityFullName = "Eaf.Authorization.Users.User",
                DynamicPropertyId = 5
            };

            // Act
            var result = await _dynamicEntityPropertyAppService.CreateAsync(input);

            // Assert
            result.Id.ShouldBe(7);
            result.EntityFullName.ShouldBe("Eaf.Authorization.Users.User");
            result.DynamicProperty.PropertyName.ShouldBe("City");
        }

        [Fact]
        public async Task Dado_PropriedadesDeEntidade_Quando_ListarPorNomeDeEntidade_Entao_DeveRetornarListaFiltrada()
        {
            // Arrange
            _dynamicEntityPropertyManager.GetAsync(Arg.Any<int>())
                .Returns(callInfo => Task.FromResult(new DynamicEntityProperty
                {
                    Id = callInfo.Arg<int>(),
                    EntityFullName = "Eaf.Authorization.Users.User",
                    DynamicProperty = new DynamicProperty { Id = 1, PropertyName = "City" }
                }));

            _dynamicEntityPropertyManager.GetAllAsync("Eaf.Authorization.Users.User")
                .Returns(Task.FromResult(new List<DynamicEntityProperty>
                {
                    new DynamicEntityProperty
                    {
                        Id = 1,
                        EntityFullName = "Eaf.Authorization.Users.User",
                        DynamicProperty = new DynamicProperty { Id = 1, PropertyName = "City" }
                    }
                }));

            // Act
            var result = await _dynamicEntityPropertyAppService.GetAllAsync("Eaf.Authorization.Users.User");

            // Assert
            result.Items.Count.ShouldBe(1);
            result.Items[0].EntityFullName.ShouldBe("Eaf.Authorization.Users.User");
        }

        [Fact]
        public async Task Dado_UmaPropriedadeDeEntidade_Quando_Excluir_Entao_DeveChamarDelete()
        {
            // Act
            await _dynamicEntityPropertyAppService.DeleteAsync(3);

            // Assert
            await _dynamicEntityPropertyManager.Received(1).DeleteAsync(3);
        }
    }
}
