using Eaf.Log4NetServiceBus.Logging;
using Shouldly;
using System;
using System.Reflection;
using Xunit;

namespace Eaf.Log4NetServiceBus.Tests.Logging
{
    /// <summary>
    /// Testes BDD para ServiceBusQueueAppender — Spec 86.
    /// Verifica error handling, propriedades e comportamento defensivo.
    /// </summary>
    public class ServiceBusQueueAppenderBddTests
    {
        #region Propriedades

        [Fact]
        public void Dado_AppenderNovo_Quando_VerificarPropriedades_Entao_DeveRetornarNuloOuVazio()
        {
            // Dado & Quando
            var appender = new ServiceBusQueueAppender();

            // Então
            appender.ApplicationName.ShouldBeNull();
            appender.ConnectionString.ShouldBeNull();
            appender.QueueName.ShouldBeNull();
            appender.StorageType.ShouldBeNull();
            appender.RetentionTime.ShouldBe(0);
        }

        [Fact]
        public void Dado_Appender_Quando_DefinirApplicationName_Entao_DeveReter()
        {
            // Dado
            var appender = new ServiceBusQueueAppender();

            // Quando
            appender.ApplicationName = "MeuApp";

            // Então
            appender.ApplicationName.ShouldBe("MeuApp");
        }

        [Fact]
        public void Dado_Appender_Quando_DefinirConnectionString_Entao_DeveReter()
        {
            // Dado
            var appender = new ServiceBusQueueAppender();

            // Quando
            appender.ConnectionString = "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=key";

            // Então
            appender.ConnectionString.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void Dado_Appender_Quando_DefinirQueueName_Entao_DeveReter()
        {
            // Dado
            var appender = new ServiceBusQueueAppender();

            // Quando
            appender.QueueName = "log-queue";

            // Então
            appender.QueueName.ShouldBe("log-queue");
        }

        [Fact]
        public void Dado_Appender_Quando_DefinirRetentionTime_Entao_DeveReter()
        {
            // Dado
            var appender = new ServiceBusQueueAppender();

            // Quando
            appender.RetentionTime = 30;

            // Então
            appender.RetentionTime.ShouldBe(30);
        }

        [Fact]
        public void Dado_Appender_Quando_DefinirStorageType_Entao_DeveReter()
        {
            // Dado
            var appender = new ServiceBusQueueAppender();

            // Quando
            appender.StorageType = "AzureBlob";

            // Então
            appender.StorageType.ShouldBe("AzureBlob");
        }

        #endregion

        #region Comportamento Defensivo (Spec 86 - Error Handling)

        [Fact]
        public void Dado_PropriedadesNulas_Quando_SendBuffer_Entao_NaoDeveLancarExcecao()
        {
            // Dado — AppendBuffer retorna early se props null/empty
            var appender = new ServiceBusQueueAppender();
            var sendBufferMethod = typeof(ServiceBusQueueAppender)
                .GetMethod("SendBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            sendBufferMethod.ShouldNotBeNull("SendBuffer deve existir como método protegido");

            // Quando & Então — não deve lançar exceção (exit early)
            Should.NotThrow(() => sendBufferMethod.Invoke(appender, new object[] { Array.Empty<log4net.Core.LoggingEvent>() }));
        }

        [Fact]
        public void Dado_ConnectionStringVazia_Quando_SendBuffer_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var appender = new ServiceBusQueueAppender
            {
                ApplicationName = "Test",
                ConnectionString = "",
                QueueName = "queue",
                StorageType = "Blob"
            };
            var sendBufferMethod = typeof(ServiceBusQueueAppender)
                .GetMethod("SendBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            sendBufferMethod.ShouldNotBeNull("SendBuffer deve existir como método protegido");

            // Quando & Então
            Should.NotThrow(() => sendBufferMethod.Invoke(appender, new object[] { Array.Empty<log4net.Core.LoggingEvent>() }));
        }

        [Fact]
        public void Dado_QueueNameVazio_Quando_SendBuffer_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var appender = new ServiceBusQueueAppender
            {
                ApplicationName = "Test",
                ConnectionString = "Endpoint=sb://test.servicebus.windows.net/",
                QueueName = "",
                StorageType = "Blob"
            };
            var sendBufferMethod = typeof(ServiceBusQueueAppender)
                .GetMethod("SendBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            sendBufferMethod.ShouldNotBeNull("SendBuffer deve existir como método protegido");

            // Quando & Então
            Should.NotThrow(() => sendBufferMethod.Invoke(appender, new object[] { Array.Empty<log4net.Core.LoggingEvent>() }));
        }

        [Fact]
        public void Dado_ApplicationNameVazio_Quando_SendBuffer_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var appender = new ServiceBusQueueAppender
            {
                ApplicationName = "",
                ConnectionString = "Endpoint=sb://test.servicebus.windows.net/",
                QueueName = "queue",
                StorageType = "Blob"
            };
            var sendBufferMethod = typeof(ServiceBusQueueAppender)
                .GetMethod("SendBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            sendBufferMethod.ShouldNotBeNull("SendBuffer deve existir como método protegido");

            // Quando & Então
            Should.NotThrow(() => sendBufferMethod.Invoke(appender, new object[] { Array.Empty<log4net.Core.LoggingEvent>() }));
        }

        [Fact]
        public void Dado_PropriedadesValidas_Quando_SendBuffer_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var appender = new ServiceBusQueueAppender
            {
                ApplicationName = "TestApp",
                ConnectionString = "Endpoint=sb://localhost:1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa=",
                QueueName = "log-queue",
                StorageType = "Blob"
            };
            var sendBufferMethod = typeof(ServiceBusQueueAppender)
                .GetMethod("SendBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            sendBufferMethod.ShouldNotBeNull("SendBuffer deve existir como método protegido");

            // Quando & Então
            Should.NotThrow(() => sendBufferMethod.Invoke(appender, new object[] { Array.Empty<log4net.Core.LoggingEvent>() }));
        }

        [Fact]
        public void Dado_PropriedadesValidasComEvento_Quando_SendBuffer_Entao_DeveTratarExcecaoSemLancar()
        {
            // Dado
            var appender = new ServiceBusQueueAppender
            {
                ApplicationName = "TestApp",
                ConnectionString = "Endpoint=sb://localhost:1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa=",
                QueueName = "log-queue",
                StorageType = "Blob"
            };
            var sendBufferMethod = typeof(ServiceBusQueueAppender)
                .GetMethod("SendBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            sendBufferMethod.ShouldNotBeNull("SendBuffer deve existir como método protegido");

            var events = new[] { new log4net.Core.LoggingEvent(new log4net.Core.LoggingEventData { Message = "INFO | server | event | message | json" }) };

            // Quando & Então
            Should.NotThrow(() => sendBufferMethod.Invoke(appender, new object[] { events }));
        }

        #endregion

        #region OnClose

        [Fact]
        public void Dado_AppenderSemConexao_Quando_OnClose_Entao_NaoDeveLancarExcecao()
        {
            // Dado — _serviceBusConnection será null, OnClose deve ser seguro
            var appender = new ServiceBusQueueAppender();
            var onCloseMethod = typeof(ServiceBusQueueAppender)
                .GetMethod("OnClose", BindingFlags.NonPublic | BindingFlags.Instance);

            // Quando & Então
            Should.NotThrow(() => onCloseMethod?.Invoke(appender, null));
        }

        #endregion

        #region GetParams

        [Fact]
        public void Dado_AppenderInstanciado_Quando_VerificarHeranca_Entao_DeveHerdarDeBufferingAppenderSkeleton()
        {
            // Dado & Quando
            var type = typeof(ServiceBusQueueAppender);

            // Então
            typeof(log4net.Appender.BufferingAppenderSkeleton).IsAssignableFrom(type).ShouldBeTrue();
        }

        [Fact]
        public void Dado_AppenderInstanciado_Quando_VerificarMetodoGetParams_Entao_DeveSerPrivado()
        {
            // Dado
            var type = typeof(ServiceBusQueueAppender);

            // Quando
            var getParamsMethod = type.GetMethod("GetParams", BindingFlags.NonPublic | BindingFlags.Instance);

            // Então
            getParamsMethod.ShouldNotBeNull();
            getParamsMethod.IsPrivate.ShouldBeTrue();
        }

        [Fact]
        public void Dado_AppenderInstanciado_Quando_VerificarMetodoAppendBuffer_Entao_DeveSerProtected()
        {
            // Dado
            var type = typeof(ServiceBusQueueAppender);

            // Quando
            var appendMethod = type.GetMethod("AppendBuffer", BindingFlags.NonPublic | BindingFlags.Instance);

            // Então
            appendMethod.ShouldNotBeNull();
            appendMethod.IsFamily.ShouldBeTrue(); // protected
        }

        [Fact]
        public void Dado_GetParams_Quando_MensagemComSeparadores_Entao_DeveRetornarValorLimpo()
        {
            // Dado
            var appender = new ServiceBusQueueAppender();
            var getParamsMethod = typeof(ServiceBusQueueAppender)
                .GetMethod("GetParams", BindingFlags.NonPublic | BindingFlags.Instance);

            // Quando
            var result = getParamsMethod!.Invoke(appender, new object[] { 2, "INFO | server | event | message | json" });

            // Então
            result!.ShouldBe("event");
        }

        [Fact]
        public void Dado_GetParams_Quando_ValorNull_Entao_DeveRetornarVazio()
        {
            // Dado
            var appender = new ServiceBusQueueAppender();
            var getParamsMethod = typeof(ServiceBusQueueAppender)
                .GetMethod("GetParams", BindingFlags.NonPublic | BindingFlags.Instance);

            // Quando
            var result = getParamsMethod!.Invoke(appender, new object[] { 0, "(null) | server" });

            // Então
            result!.ShouldBe("");
        }

        [Fact]
        public void Dado_GetParams_Quando_IndiceForaDoRange_Entao_DeveRetornarMensagemDeErro()
        {
            // Dado
            var appender = new ServiceBusQueueAppender();
            var getParamsMethod = typeof(ServiceBusQueueAppender)
                .GetMethod("GetParams", BindingFlags.NonPublic | BindingFlags.Instance);

            // Quando
            var result = getParamsMethod!.Invoke(appender, new object[] { 10, "apenas um valor" });

            // Então
            result!.ShouldBe("Params parse error");
        }

        #endregion
    }
}
