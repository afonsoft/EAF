using Castle.Core.Logging;
using log4net;
using Newtonsoft.Json;

namespace Eaf.Log4NetServiceBus.Logging
{
    /// <summary>
    /// Representa a classe LogExtensions.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Debug.
        /// </summary>
        /// <param name="logger">Parâmetro logger.</param>
        /// <param name="message">Parâmetro message.</param>
        /// <param name="item">Parâmetro item.</param>
        public static void Debug(this ILogger logger, string message, object item)
        {
            load(item);
            logger.Debug(message);
            clear();
        }

        /// <summary>
        /// Debug.
        /// </summary>
        /// <param name="logger">Parâmetro logger.</param>
        /// <param name="message">Parâmetro message.</param>
        /// <param name="item">Parâmetro item.</param>
        public static void Debug(this ILog logger, string message, object item)
        {
            load(item);
            logger.Debug(message);
            clear();
        }

        /// <summary>
        /// Error.
        /// </summary>
        /// <param name="logger">Parâmetro logger.</param>
        /// <param name="message">Parâmetro message.</param>
        /// <param name="item">Parâmetro item.</param>
        public static void Error(this ILogger logger, string message, object item)
        {
            load(item);
            logger.Error(message);
            clear();
        }

        /// <summary>
        /// Error.
        /// </summary>
        /// <param name="logger">Parâmetro logger.</param>
        /// <param name="message">Parâmetro message.</param>
        /// <param name="item">Parâmetro item.</param>
        public static void Error(this ILog logger, string message, object item)
        {
            load(item);
            logger.Error(message);
            clear();
        }

        /// <summary>
        /// Info.
        /// </summary>
        /// <param name="logger">Parâmetro logger.</param>
        /// <param name="message">Parâmetro message.</param>
        /// <param name="item">Parâmetro item.</param>
        public static void Info(this ILogger logger, string message, object item)
        {
            load(item);
            logger.Info(message);
            clear();
        }

        /// <summary>
        /// Info.
        /// </summary>
        /// <param name="logger">Parâmetro logger.</param>
        /// <param name="message">Parâmetro message.</param>
        /// <param name="item">Parâmetro item.</param>
        public static void Info(this ILog logger, string message, object item)
        {
            load(item);
            logger.Info(message);
            clear();
        }

        /// <summary>
        /// Warn.
        /// </summary>
        /// <param name="logger">Parâmetro logger.</param>
        /// <param name="message">Parâmetro message.</param>
        /// <param name="item">Parâmetro item.</param>
        public static void Warn(this ILogger logger, string message, object item)
        {
            load(item);
            logger.Warn(message);
            clear();
        }

        /// <summary>
        /// Warn.
        /// </summary>
        /// <param name="logger">Parâmetro logger.</param>
        /// <param name="message">Parâmetro message.</param>
        /// <param name="item">Parâmetro item.</param>
        public static void Warn(this ILog logger, string message, object item)
        {
            load(item);
            logger.Warn(message);
            clear();
        }

        private static void clear()
        {
            log4net.LogicalThreadContext.Properties["Json"] = "";
        }

        private static void load(object item)
        {
            log4net.LogicalThreadContext.Properties["Json"] = JsonConvert.SerializeObject(item);
        }
    }
}