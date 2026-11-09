using System;
using Abp;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.Modules;
using Eaf.Dapper;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Dapper.Tests
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    }

    public class EafDapper_Tests
    {
        // Dado nenhum UoW ativo, quando GetConnection é chamado, então lança UserFriendlyException.
        [Fact]
        public void Dado_SemUnitOfWork_Quando_GetConnection_Entao_LancaUserFriendlyException()
        {
            var uowProvider = Substitute.For<ICurrentUnitOfWorkProvider>();
            uowProvider.Current.Returns((IUnitOfWork)null);
            var provider = new DapperConnectionProvider(Substitute.For<Abp.Dependency.IIocResolver>(), uowProvider);

            Should.Throw<Abp.UI.UserFriendlyException>(() => provider.GetConnection(typeof(TestDbContext)));
        }

        // Dado um UoW ativo, quando GetConnection é chamado, então retorna a conexão do DbContext.
        [Fact]
        public void Dado_ComUnitOfWork_Quando_GetConnection_Entao_RetornaConexao()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>().UseSqlite("Data Source=:memory:").Options;
            var context = new TestDbContext(options);
            context.Database.OpenConnection();

            var ctxProvider = Substitute.For<IDbContextProvider<TestDbContext>>();
            ctxProvider.GetDbContext().Returns(context);

            var uowProvider = Substitute.For<ICurrentUnitOfWorkProvider>();
            uowProvider.Current.Returns(Substitute.For<IUnitOfWork>());

            var ioc = Substitute.For<Abp.Dependency.IIocResolver>();
            ioc.Resolve(Arg.Any<Type>()).Returns(ctxProvider);

            var provider = new DapperConnectionProvider(ioc, uowProvider);
            provider.GetConnection(typeof(TestDbContext)).ShouldBeSameAs(context.Database.GetDbConnection());
        }

        // Dado o módulo, quando inspecionado, então depende de Abp.Dapper e Abp.EntityFrameworkCore.
        [Fact]
        public void Dado_Modulo_Quando_Inspecionado_Entao_DependenciasCorretas()
        {
            var depends = typeof(EafDapperModule).GetCustomAttributes(typeof(DependsOnAttribute), true);
            depends.Length.ShouldBe(1);
        }
    }
}
