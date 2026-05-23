using Eaf.Log4NetServiceBus.Logging;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Log4NetServiceBus.Tests.Logging
{
    /// <summary>
    /// Testes BDD para a classe LogMessage.
    /// </summary>
    public class LogMessageBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_CriarLogMessage_Entao_PropriedadesDevemSerPadrao()
        {
            // Arrange & Act
            var logMessage = new LogMessage();

            // Assert
            logMessage.ApplicationName.ShouldBeNull();
            logMessage.CorrelationId.ShouldBeNull();
            logMessage.Event.ShouldBeNull();
            logMessage.JsonData.ShouldBeNull();
            logMessage.Level.ShouldBeNull();
            logMessage.Message.ShouldBeNull();
            logMessage.ServerName.ShouldBeNull();
            logMessage.StorageType.ShouldBeNull();
            logMessage.RetentionTime.ShouldBe(0);
            logMessage.EventDateUTC.ShouldBe(default(DateTime));
            logMessage.PurgeDateUTC.ShouldBe(default(DateTime));
        }

        [Fact]
        public void Dado_ValoresValidos_Quando_DefinirPropriedades_Entao_DeveArmazenarCorretamente()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var purgeDate = now.AddDays(30);

            // Act
            var logMessage = new LogMessage
            {
                ApplicationName = "EAF.TestApp",
                CorrelationId = "corr-123-456",
                Event = "UserLogin",
                EventDateUTC = now,
                JsonData = "{\"userId\": 42}",
                Level = "Info",
                Message = "User logged in successfully",
                PurgeDateUTC = purgeDate,
                RetentionTime = 30,
                ServerName = "server-01",
                StorageType = "AzureBlob"
            };

            // Assert
            logMessage.ApplicationName.ShouldBe("EAF.TestApp");
            logMessage.CorrelationId.ShouldBe("corr-123-456");
            logMessage.Event.ShouldBe("UserLogin");
            logMessage.EventDateUTC.ShouldBe(now);
            logMessage.JsonData.ShouldBe("{\"userId\": 42}");
            logMessage.Level.ShouldBe("Info");
            logMessage.Message.ShouldBe("User logged in successfully");
            logMessage.PurgeDateUTC.ShouldBe(purgeDate);
            logMessage.RetentionTime.ShouldBe(30);
            logMessage.ServerName.ShouldBe("server-01");
            logMessage.StorageType.ShouldBe("AzureBlob");
        }

        [Theory]
        [InlineData("Debug")]
        [InlineData("Info")]
        [InlineData("Warn")]
        [InlineData("Error")]
        [InlineData("Fatal")]
        public void Dado_NivelDeLog_Quando_DefinirLevel_Entao_DeveArmazenarNivel(string level)
        {
            // Arrange & Act
            var logMessage = new LogMessage { Level = level };

            // Assert
            logMessage.Level.ShouldBe(level);
        }

        [Fact]
        public void Dado_RetentionTimeNegativo_Quando_Definir_Entao_DevePermitir()
        {
            // Arrange & Act
            var logMessage = new LogMessage { RetentionTime = -1 };

            // Assert
            logMessage.RetentionTime.ShouldBe(-1);
        }

        [Fact]
        public void Dado_StringVazia_Quando_DefinirPropriedades_Entao_DeveArmazenarVazio()
        {
            // Arrange & Act
            var logMessage = new LogMessage
            {
                ApplicationName = "",
                Message = "",
                JsonData = ""
            };

            // Assert
            logMessage.ApplicationName.ShouldBe("");
            logMessage.Message.ShouldBe("");
            logMessage.JsonData.ShouldBe("");
        }

        [Fact]
        public void Dado_JsonDataComplexo_Quando_Definir_Entao_DeveArmazenar()
        {
            // Arrange
            var complexJson = "{\"user\":{\"id\":1,\"name\":\"Admin\"},\"permissions\":[\"read\",\"write\"],\"metadata\":{\"ip\":\"192.168.1.1\"}}";

            // Act
            var logMessage = new LogMessage { JsonData = complexJson };

            // Assert
            logMessage.JsonData.ShouldBe(complexJson);
        }
    }
}
