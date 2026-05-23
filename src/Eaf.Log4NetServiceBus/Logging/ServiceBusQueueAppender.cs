using log4net.Appender;
using log4net.Core;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Eaf.Log4NetServiceBus.Logging
{
    /// <summary>
    /// Representa a classe ServiceBusQueueAppender.
    /// </summary>
    public class ServiceBusQueueAppender : BufferingAppenderSkeleton
    {
        private static readonly object _sync = new object();
        private ServiceBusConnection _serviceBusConnection;

        /// <summary>
        /// Obtém ou define ApplicationName.
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// Obtém ou define ConnectionString.
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Obtém ou define QueueName.
        /// </summary>
        public string QueueName { get; set; }
        /// <summary>
        /// Obtém ou define RetentionTime.
        /// </summary>
        public int RetentionTime { get; set; }
        /// <summary>
        /// Obtém ou define StorageType.
        /// </summary>
        public string StorageType { get; set; }

        protected void AppendBuffer(LoggingEvent[] events)
        {
            try
            {
                if (string.IsNullOrEmpty(ConnectionString) || string.IsNullOrEmpty(QueueName) || string.IsNullOrEmpty(ApplicationName) || string.IsNullOrEmpty(StorageType))
                    return;

                QueueClient queueClient = null;

                lock (_sync)
                {
                    _serviceBusConnection ??= new ServiceBusConnection(ConnectionString);

                    queueClient = new QueueClient(_serviceBusConnection, QueueName, ReceiveMode.PeekLock, RetryPolicy.Default);
                }

                var messages = new List<Message>();

                foreach (var loggingEvent in events)
                {
                    var paramsFull = RenderLoggingEvent(loggingEvent);

                    var log = new LogMessage()
                    {
                        StorageType = StorageType,
                        EventDateUTC = DateTime.UtcNow,
                        PurgeDateUTC = DateTime.UtcNow.AddDays(RetentionTime),
                        RetentionTime = RetentionTime,
                        ApplicationName = ApplicationName,
                        Level = getParams(0, paramsFull),
                        ServerName = getParams(1, paramsFull),
                        Event = getParams(2, paramsFull),
                        Message = getParams(3, paramsFull),
                        JsonData = getParams(4, paramsFull)
                    };

                    messages.Add(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(log))));
                }

                queueClient.SendAsync(messages);
            }
            catch (Exception)
            {
                //bypass
            }
        }

        protected override async void OnClose()
        {
            if (!_serviceBusConnection.IsClosedOrClosing)
                await _serviceBusConnection.CloseAsync();

            base.OnClose();
        }

        protected override void SendBuffer(LoggingEvent[] events)
        {
            Task.Run(() => AppendBuffer(events));
        }

        private string getParams(int index, string message)
        {
            try
            {
                var msg = message.Split('|')[index].Trim();
                return msg == "(null)" ? "" : msg;
            }
            catch (Exception)
            {
                return "Params parse error";
            }
        }
    }
}