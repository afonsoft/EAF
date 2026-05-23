using Abp.EntityHistory;
using Eaf.Middleware.Auditing;
using Eaf.Middleware.Authorization.Users;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Auditing
{
    public class EntityChangeAndUserTests
    {
        [Fact]
        public void Dado_EntityChangeAndUser_Quando_Criado_Entao_PropriedadesDevemSerAtribuidas()
        {
            var entityChange = new EntityChange();
            var user = new User();

            var obj = new EntityChangeAndUser
            {
                EntityChange = entityChange,
                User = user
            };

            obj.EntityChange.ShouldBe(entityChange);
            obj.User.ShouldBe(user);
        }

        [Fact]
        public void Dado_EntityChangeAndUser_Quando_PadraoInicial_Entao_PropriedadesDevemSerNulas()
        {
            var obj = new EntityChangeAndUser();
            obj.EntityChange.ShouldBeNull();
            obj.User.ShouldBeNull();
        }
    }
}
