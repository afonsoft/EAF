using Eaf.Log4NetServiceBus.Logging;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Log4NetServiceBus.Tests
{
    public class LogMessageTests
    {
        [Fact]
        public void Dado_LogMessage_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var now = DateTime.UtcNow;
            var purgeDate = now.AddDays(30);

            var logMessage = new LogMessage
            {
                ApplicationName = "EAF.Middleware",
                CorrelationId = "corr-12345",
                Event = "UserLogin",
                EventDateUTC = now,
                JsonData = "{\"userId\":42}",
                Level = "Info",
                Message = "User logged in successfully",
                PurgeDateUTC = purgeDate,
                RetentionTime = 30,
                ServerName = "server-01",
                StorageType = "AzureServiceBus"
            };

            logMessage.ApplicationName.ShouldBe("EAF.Middleware");
            logMessage.CorrelationId.ShouldBe("corr-12345");
            logMessage.Event.ShouldBe("UserLogin");
            logMessage.EventDateUTC.ShouldBe(now);
            logMessage.JsonData.ShouldBe("{\"userId\":42}");
            logMessage.Level.ShouldBe("Info");
            logMessage.Message.ShouldBe("User logged in successfully");
            logMessage.PurgeDateUTC.ShouldBe(purgeDate);
            logMessage.RetentionTime.ShouldBe(30);
            logMessage.ServerName.ShouldBe("server-01");
            logMessage.StorageType.ShouldBe("AzureServiceBus");
        }

        [Fact]
        public void Dado_LogMessage_Quando_Instanciar_Entao_PropriedadesDevemSerPadrao()
        {
            var logMessage = new LogMessage();

            logMessage.ApplicationName.ShouldBeNull();
            logMessage.CorrelationId.ShouldBeNull();
            logMessage.Event.ShouldBeNull();
            logMessage.JsonData.ShouldBeNull();
            logMessage.Level.ShouldBeNull();
            logMessage.Message.ShouldBeNull();
            logMessage.ServerName.ShouldBeNull();
            logMessage.StorageType.ShouldBeNull();
            logMessage.RetentionTime.ShouldBe(0);
        }

        [Fact]
        public void Dado_LogMessage_Quando_DefinirNiveis_Entao_DeveAceitarTodosNiveis()
        {
            var levels = new[] { "Debug", "Info", "Warn", "Error", "Fatal" };

            foreach (var level in levels)
            {
                var logMessage = new LogMessage { Level = level };
                logMessage.Level.ShouldBe(level);
            }
        }
    }
}
