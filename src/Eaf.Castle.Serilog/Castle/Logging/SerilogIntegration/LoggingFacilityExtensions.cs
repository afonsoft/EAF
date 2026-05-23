using Castle.Facilities.Logging;

namespace Eaf.Castle.Logging.SerilogIntegration
{
    /// <summary>
    /// Extensões para integração do Serilog com Castle Windsor LoggingFacility.
    /// Fornece métodos de extensão para configurar o Serilog como provedor de logging no Castle.
    /// </summary>
    public static class LoggingFacilityExtensions
    {
        /// <summary>
        /// Configura o LoggingFacility do Castle Windsor para usar o Serilog como provedor de logging.
        /// Substitui o logger padrão do Castle pela implementação personalizada do EAF com Serilog.
        /// </summary>
        /// <param name="loggingFacility">A instância do LoggingFacility a ser configurada.</param>
        /// <returns>A mesma instância do LoggingFacility configurada para usar o SerilogLoggerFactory.</returns>
        public static LoggingFacility UseEafSerilog(this LoggingFacility loggingFacility)
        {
            return loggingFacility.LogUsing<SerilogLoggerFactory>();
        }
    }
}