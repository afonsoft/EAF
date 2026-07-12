using System;
using System.Collections.Generic;
using System.Reflection;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Collections;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.EntityFrameworkCore.Configuration;
using AutoMapper;
using Eaf.MiddlewareCore.SampleApp;
using Eaf.MiddlewareCore.SampleApp.Application;
using Eaf.MiddlewareCore.SampleApp.EntityFramework;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.SampleApp
{
    public class EafMiddlewareCoreSampleAppModuleBddTests
    {
        [Fact]
        public void Dado_ModuloComDbContextRegistration_Quando_PreInitialize_Entao_DeveRegistrarDbContextEProviders()
        {
            var module = new EafMiddlewareCoreSampleAppModule { SkipDbContextRegistration = false };

            var iocManager = Substitute.For<IIocManager>();

            var config = Substitute.For<IAbpStartupConfiguration>();
            var authorization = Substitute.For<IAuthorizationConfiguration>();
            var authorizationProviders = Substitute.For<ITypeList<AuthorizationProvider>>();
            var features = Substitute.For<IFeatureConfiguration>();
            var featureProviders = Substitute.For<ITypeList<FeatureProvider>>();
            var modules = Substitute.For<IModuleConfigurations>();
            var efCore = Substitute.For<IAbpEfCoreConfiguration>();
            var customProviders = new List<ICustomConfigProvider>();

            authorization.Providers.Returns(authorizationProviders);
            features.Providers.Returns(featureProviders);
            modules.AbpEfCore().Returns(efCore);

            config.Authorization.Returns(authorization);
            config.Features.Returns(features);
            config.Modules.Returns(modules);
            config.CustomConfigProviders.Returns(customProviders);

            var moduleType = typeof(Abp.Modules.AbpModule);
            moduleType.GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(module, iocManager);
            moduleType.GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(module, config);

            Should.NotThrow(() => module.PreInitialize());

            customProviders.Count.ShouldBe(2);
            authorizationProviders.Received().Add<AppAuthorizationProvider>();
            featureProviders.Received().Add<AppFeatureProvider>();
            efCore.Received().AddDbContext<SampleAppDbContext>(Arg.Any<Action<AbpDbContextConfiguration<SampleAppDbContext>>>());
        }

        [Fact]
        public void Dado_ModuloSemDbContextRegistration_Quando_PreInitialize_Entao_NaoDeveRegistrarDbContext()
        {
            var module = new EafMiddlewareCoreSampleAppModule { SkipDbContextRegistration = true };

            var iocManager = Substitute.For<IIocManager>();

            var config = Substitute.For<IAbpStartupConfiguration>();
            var authorization = Substitute.For<IAuthorizationConfiguration>();
            var authorizationProviders = Substitute.For<ITypeList<AuthorizationProvider>>();
            var features = Substitute.For<IFeatureConfiguration>();
            var featureProviders = Substitute.For<ITypeList<FeatureProvider>>();
            var modules = Substitute.For<IModuleConfigurations>();
            var efCore = Substitute.For<IAbpEfCoreConfiguration>();
            var customProviders = new List<ICustomConfigProvider>();

            authorization.Providers.Returns(authorizationProviders);
            features.Providers.Returns(featureProviders);
            modules.AbpEfCore().Returns(efCore);

            config.Authorization.Returns(authorization);
            config.Features.Returns(features);
            config.Modules.Returns(modules);
            config.CustomConfigProviders.Returns(customProviders);

            var moduleType = typeof(Abp.Modules.AbpModule);
            moduleType.GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(module, iocManager);
            moduleType.GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(module, config);

            Should.NotThrow(() => module.PreInitialize());

            customProviders.Count.ShouldBe(2);
            authorizationProviders.Received().Add<AppAuthorizationProvider>();
            featureProviders.Received().Add<AppFeatureProvider>();
            efCore.DidNotReceive().AddDbContext<SampleAppDbContext>(Arg.Any<Action<AbpDbContextConfiguration<SampleAppDbContext>>>());
        }

        [Fact]
        public void Dado_Modulo_Quando_Initialize_Entao_DeveRegistrarAssembliesEAutoMapper()
        {
            var module = new EafMiddlewareCoreSampleAppModule();

            var iocManager = new IocManager();
            var config = Substitute.For<IAbpStartupConfiguration>();
            var modules = Substitute.For<IModuleConfigurations>();
            var abpAutoMapper = Substitute.For<IAbpAutoMapperConfiguration>();
            var configurators = new List<Action<IMapperConfigurationExpression>>();

            abpAutoMapper.Configurators.Returns(configurators);
            modules.AbpAutoMapper().Returns(abpAutoMapper);
            config.Modules.Returns(modules);

            var moduleType = typeof(Abp.Modules.AbpModule);
            moduleType.GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(module, iocManager);
            moduleType.GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(module, config);

            Should.NotThrow(() => module.Initialize());

            configurators.Count.ShouldBe(1);
        }
    }
}
