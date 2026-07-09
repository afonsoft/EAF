using AutoMapper;
using Eaf.Middleware.Configuration;
using Shouldly;
using System;
using System.Reflection;
using Xunit;

namespace Eaf.Middleware.Tests.Application
{
    public class MiddlewareCustomDtoMapperBddTests
    {
        [Fact]
        public void Dado_ConfiguracaoAutoMapper_Quando_CriarMapeamentos_Entao_DeveExecutarSemErros()
        {
            var mapperType = typeof(GoogleAppService).Assembly.GetType("Eaf.Middleware.MiddlewareCustomDtoMapper");
            mapperType.ShouldNotBeNull();

            var method = mapperType!.GetMethod("CreateMappings", BindingFlags.Public | BindingFlags.Static, new[] { typeof(IMapperConfigurationExpression) });
            method.ShouldNotBeNull();

            var config = new MapperConfiguration(cfg =>
            {
                method!.Invoke(null, new object[] { cfg });
            });

            config.ShouldNotBeNull();
            var mapper = config.CreateMapper();
            mapper.ShouldNotBeNull();
        }
    }
}
