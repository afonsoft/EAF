using Abp.Modules;
using Abp.Runtime.System;
using Eaf.Controllers;
using Eaf.Models.About;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Controllers
{
    /// <summary>
    /// Testes BDD para AboutController seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class AboutControllerBddTests
    {
        private readonly IAbpModuleManager _moduleManager;
        private readonly IOSPlatformProvider _osPlatformProvider;
        private readonly AboutController _sut;

        public AboutControllerBddTests()
        {
            _moduleManager = Substitute.For<IAbpModuleManager>();
            _osPlatformProvider = Substitute.For<IOSPlatformProvider>();
            _moduleManager.Modules.Returns(new List<AbpModuleInfo>());
            _osPlatformProvider.GetCurrentOSPlatform().Returns(System.Runtime.InteropServices.OSPlatform.Linux);
            _sut = new AboutController(_moduleManager, _osPlatformProvider);
        }

        #region Instanciacao

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region GetAbout

        [Fact]
        public void Dado_Controller_Quando_GetAbout_Entao_DeveRetornarAboutModel()
        {
            // Quando
            var result = _sut.GetAbout();

            // Entao
            result.ShouldNotBeNull();
            result.ShouldBeOfType<AboutModel>();
        }

        [Fact]
        public void Dado_Controller_Quando_GetAbout_Entao_DevePreencherVersion()
        {
            var result = _sut.GetAbout();
            result.Version.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_Controller_Quando_GetAbout_Entao_DevePreencherArchitecture()
        {
            var result = _sut.GetAbout();
            result.Architecture.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_Controller_Quando_GetAbout_Entao_DevePreencherFrameworkDescription()
        {
            var result = _sut.GetAbout();
            result.FrameworkDescription.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_Controller_Quando_GetAbout_Entao_DevePreencherMachineName()
        {
            var result = _sut.GetAbout();
            result.MachineName.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_Controller_Quando_GetAbout_Entao_DevePreencherOSVersion()
        {
            var result = _sut.GetAbout();
            result.OSVersion.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_Controller_Quando_GetAbout_Entao_DevePreencherOS()
        {
            var result = _sut.GetAbout();
            result.OS.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_Controller_Quando_GetAbout_Entao_DevePreencherProcessName()
        {
            var result = _sut.GetAbout();
            result.ProcessName.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_ModulesVazio_Quando_GetAbout_Entao_ModulesDeveSerArrayVazio()
        {
            _moduleManager.Modules.Returns(new List<AbpModuleInfo>());
            var result = _sut.GetAbout();
            result.Modules.ShouldNotBeNull();
            result.Modules.ShouldBeEmpty();
        }

        [Fact]
        public void Dado_Controller_Quando_GetAbout_Entao_EnvironmentsDeveSerDicionario()
        {
            var result = _sut.GetAbout();
            result.Environments.ShouldNotBeNull();
            result.Environments.ShouldBeOfType<Dictionary<string, string>>();
        }

        [Fact]
        public void Dado_Controller_Quando_GetAbout_Entao_NumberOfProcessorsDeveSerPreenchido()
        {
            var result = _sut.GetAbout();
            result.NumberOfProcessors.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_Controller_Quando_GetAbout_Entao_CurrentCultureDeveSerPreenchido()
        {
            var result = _sut.GetAbout();
            result.CurrentCulture.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_Controller_Quando_GetAbout_Entao_CurrentDirectoryDeveSerPreenchido()
        {
            var result = _sut.GetAbout();
            result.CurrentDirectory.ShouldNotBeNullOrEmpty();
        }

        #endregion
    }
}
