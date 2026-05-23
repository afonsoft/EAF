using Abp.Auditing;
using Eaf.Middleware.Auditing;
using Eaf.Middleware.Authorization.Users;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Auditing
{
    public class AuditLogAndUserTests
    {
        [Fact]
        public void Dado_AuditLogAndUser_Quando_Criado_Entao_PropriedadesDevemSerAtribuidas()
        {
            var auditLog = new AuditLog();
            var user = new User();

            var obj = new AuditLogAndUser
            {
                AuditLog = auditLog,
                User = user
            };

            obj.AuditLog.ShouldBe(auditLog);
            obj.User.ShouldBe(user);
        }

        [Fact]
        public void Dado_AuditLogAndUser_Quando_PadraoInicial_Entao_PropriedadesDevemSerNulas()
        {
            var obj = new AuditLogAndUser();
            obj.AuditLog.ShouldBeNull();
            obj.User.ShouldBeNull();
        }
    }
}
