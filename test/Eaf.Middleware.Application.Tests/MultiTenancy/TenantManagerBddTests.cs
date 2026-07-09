using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.MultiTenancy;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Application.MultiTenancy
{
    public class TenantManagerBddTests
    {
        [Fact]
        public void Dado_TenantManager_Quando_GetById_Entao_DeveLancarNotImplementedException()
        {
            var tenantManager = ManagerTestHelper.CreateTenantManager();

            Should.Throw<NotImplementedException>(() => tenantManager.GetById(1));
        }
    }
}
