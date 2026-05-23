using Eaf.Middleware.Url;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Net.Url
{
    public class NullAppUrlServiceAdditionalTests
    {
        [Fact]
        public void Dado_NullAppUrlService_Quando_AcessarInstance_Entao_DeveRetornarSingleton()
        {
            var instance1 = NullAppUrlService.Instance;
            var instance2 = NullAppUrlService.Instance;

            instance1.ShouldBeSameAs(instance2);
        }

        [Fact]
        public void Dado_NullAppUrlService_Quando_Verificado_Entao_DeveImplementarIAppUrlService()
        {
            NullAppUrlService.Instance.ShouldBeAssignableTo<IAppUrlService>();
        }

        [Fact]
        public void Dado_NullAppUrlService_Quando_CreateEmailActivationUrlFormatComTenantId_Entao_DeveLancarNotImplemented()
        {
            Should.Throw<NotImplementedException>(() =>
                NullAppUrlService.Instance.CreateEmailActivationUrlFormat(1));
        }

        [Fact]
        public void Dado_NullAppUrlService_Quando_CreateEmailActivationUrlFormatComTenancyName_Entao_DeveLancarNotImplemented()
        {
            Should.Throw<NotImplementedException>(() =>
                NullAppUrlService.Instance.CreateEmailActivationUrlFormat("test"));
        }

        [Fact]
        public void Dado_NullAppUrlService_Quando_CreatePasswordResetUrlFormatComTenantId_Entao_DeveLancarNotImplemented()
        {
            Should.Throw<NotImplementedException>(() =>
                NullAppUrlService.Instance.CreatePasswordResetUrlFormat(1));
        }

        [Fact]
        public void Dado_NullAppUrlService_Quando_CreatePasswordResetUrlFormatComTenancyName_Entao_DeveLancarNotImplemented()
        {
            Should.Throw<NotImplementedException>(() =>
                NullAppUrlService.Instance.CreatePasswordResetUrlFormat("test"));
        }
    }
}
