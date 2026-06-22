using Abp.Dependency;
using Eaf.Middleware.Core.Authentication.External;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Authorization.External
{
    /// <summary>
    /// Testes BDD para ExternalAuthManager seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class ExternalAuthManagerBddTests
    {
        private readonly IIocResolver _iocResolver;
        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        private readonly ExternalAuthManager _sut;

        public ExternalAuthManagerBddTests()
        {
            _iocResolver = Substitute.For<IIocResolver>();
            _externalAuthConfiguration = Substitute.For<IExternalAuthConfiguration>();
            _externalAuthConfiguration.ExternalLoginInfoProviders
                .Returns(new List<IExternalLoginInfoProvider>());
            _sut = new ExternalAuthManager(_iocResolver, _externalAuthConfiguration);
        }

        #region CreateProviderApi

        [Fact]
        public void Dado_ProviderDesconhecido_Quando_CreateProviderApi_Entao_DeveLancarArgumentNullException()
        {
            // Dado
            _externalAuthConfiguration.ExternalLoginInfoProviders
                .Returns(new List<IExternalLoginInfoProvider>());

            // Quando/Entao
            Should.Throw<ArgumentNullException>(() => _sut.CreateProviderApi("UnknownProvider"));
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
            _sut.ShouldBeAssignableTo<IExternalAuthManager>();
        }

        #endregion
    }
}
