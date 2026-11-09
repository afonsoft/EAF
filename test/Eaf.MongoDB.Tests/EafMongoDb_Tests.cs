using Abp.Configuration.Startup;
using Abp.Runtime.Session;
using Eaf.MongoDB;
using MongoDB.Driver;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.MongoDB.Tests
{
    public class SampleDoc { }

    public class EafMongoDb_Tests
    {
        private static (IEafMongoDbContext ctx, IMongoDatabase db) Create(int? tenantId, bool multiTenancy = true, bool prefix = true)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("eaf-tests");
            var session = Substitute.For<IAbpSession>();
            session.TenantId.Returns(tenantId);
            var mt = Substitute.For<IMultiTenancyConfig>();
            mt.IsEnabled.Returns(multiTenancy);
            return (new EafMongoDbContext(db, new EafMongoDbOptions { UseTenantCollectionPrefix = prefix }, session, mt), db);
        }

        // Dado tenant 3 e multi-tenancy habilitado, quando GetCollection, então nome tem prefixo t3_.
        [Fact]
        public void Dado_TenantMultiTenancy_Quando_GetCollection_Entao_NomeComPrefixoTenant()
        {
            var (ctx, _) = Create(3);
            ctx.GetCollection<SampleDoc>().CollectionNamespace.CollectionName.ShouldBe("t3_SampleDoc");
        }

        // Dado tenant nulo, quando GetCollection, então usa nome sem prefixo.
        [Fact]
        public void Dado_SemTenant_Quando_GetCollection_Entao_NomeSemPrefixo()
        {
            var (ctx, _) = Create(null);
            ctx.GetCollection<SampleDoc>().CollectionNamespace.CollectionName.ShouldBe("SampleDoc");
        }

        // Dado multi-tenancy desabilitado, quando GetCollection, então ignora o tenant.
        [Fact]
        public void Dado_MultiTenancyDesabilitado_Quando_GetCollection_Entao_NomeSemPrefixo()
        {
            var (ctx, _) = Create(3, multiTenancy: false);
            ctx.GetCollection<SampleDoc>().CollectionNamespace.CollectionName.ShouldBe("SampleDoc");
        }

        // Dado prefixo desabilitado nas opções, quando GetCollection, então nome sem prefixo.
        [Fact]
        public void Dado_PrefixoDesabilitado_Quando_GetCollection_Entao_NomeSemPrefixo()
        {
            var (ctx, _) = Create(3, prefix: false);
            ctx.GetCollection<SampleDoc>().CollectionNamespace.CollectionName.ShouldBe("SampleDoc");
        }

        // Dado as opções padrão, quando inspecionadas, então os defaults estão corretos.
        [Fact]
        public void Dado_OpcoesPadrao_Quando_Inspecionadas_Entao_DefaultsCorretos()
        {
            var o = new EafMongoDbOptions();
            o.ConnectionString.ShouldBe("mongodb://localhost:27017");
            o.DatabaseName.ShouldBe("Eaf");
            o.UseTenantCollectionPrefix.ShouldBeTrue();
            o.Value.ShouldBeSameAs(o);
        }
    }
}
