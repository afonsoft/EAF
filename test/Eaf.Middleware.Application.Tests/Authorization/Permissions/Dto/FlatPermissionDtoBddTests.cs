using Eaf.Middleware.Authorization.Permissions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Permissions
{
    public class FlatPermissionDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new FlatPermissionDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDisplayName_Entao_DeveArmazenar()
        {
            var sut = new FlatPermissionDto();
            sut.DisplayName = "test_value";
            sut.DisplayName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsGrantedByDefault_Entao_DeveArmazenar()
        {
            var sut = new FlatPermissionDto();
            sut.IsGrantedByDefault = true;
            sut.IsGrantedByDefault.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new FlatPermissionDto();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirParentName_Entao_DeveArmazenar()
        {
            var sut = new FlatPermissionDto();
            sut.ParentName = "test_value";
            sut.ParentName.ShouldBe("test_value");
        }
    }
}
