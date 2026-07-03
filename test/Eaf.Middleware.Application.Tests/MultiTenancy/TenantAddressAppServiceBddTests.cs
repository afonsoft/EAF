using Abp.Domain.Repositories;
using Eaf.Middleware.MultiTenancy;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.MultiTenancy
{
    public class TenantAddressAppServiceBddTests
    {
        [Fact]
        public void Dado_Repository_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var repository = Substitute.For<IRepository<TenantAddress, int>>();
            var sut = new TenantAddressAppService(repository);
            sut.ShouldNotBeNull();
        }
    }
}
