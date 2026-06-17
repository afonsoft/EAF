using Abp.Auditing;
using Eaf.Middleware.Auditing;
using Eaf.Middleware.Authorization.Users;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Auditing
{
    /// <summary>
    /// Testes BDD para AuditLogAndUser e EntityChangeAndUser seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class AuditLogAndUserBddTests
    {
        [Fact]
        public void Dado_AuditLogAndUser_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var auditLog = new AuditLog { ServiceName = "UserAppService" };
            var user = new User { UserName = "admin" };
            var item = new AuditLogAndUser
            {
                AuditLog = auditLog,
                User = user
            };

            // Então
            item.AuditLog.ServiceName.ShouldBe("UserAppService");
            item.User.UserName.ShouldBe("admin");
        }

        [Fact]
        public void Dado_EntityChangeAndUser_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var entityChange = new Abp.EntityHistory.EntityChange { EntityTypeFullName = "Eaf.Middleware.User" };
            var user = new User { UserName = "editor" };
            var item = new EntityChangeAndUser
            {
                EntityChange = entityChange,
                User = user
            };

            // Então
            item.EntityChange.EntityTypeFullName.ShouldBe("Eaf.Middleware.User");
            item.User.UserName.ShouldBe("editor");
        }
    }
}
