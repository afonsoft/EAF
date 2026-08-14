using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.MailKit.Tests
{
    /// <summary>
    /// Cliente SMTP fake para testes de unidade.
    /// </summary>
    public class FakeSmtpClient : global::MailKit.Net.Smtp.SmtpClient
    {
        private readonly Queue<Exception> _failures = new();

        public List<global::MimeKit.MimeMessage> SentMessages { get; } = new();
        public List<string> ConnectedHosts { get; } = new();
        public bool IsDisconnected { get; private set; }

        /// <summary>
        /// Adiciona uma exceção a ser lançada no próximo envio.
        /// </summary>
        /// <param name="exception">Exceção a ser lançada.</param>
        public void EnqueueFailure(Exception exception)
        {
            _failures.Enqueue(exception);
        }

        public override void Connect(string host, int port, global::MailKit.Security.SecureSocketOptions options, CancellationToken cancellationToken = default)
        {
            ConnectedHosts.Add(host);
        }

        public override Task<string> SendAsync(global::MimeKit.MimeMessage message, CancellationToken cancellationToken = default, global::MailKit.ITransferProgress progress = null)
        {
            if (_failures.Count > 0)
            {
                throw _failures.Dequeue();
            }

            SentMessages.Add(message);
            return Task.FromResult(Guid.NewGuid().ToString());
        }

        public override Task DisconnectAsync(bool quit, CancellationToken cancellationToken = default)
        {
            IsDisconnected = true;
            return Task.CompletedTask;
        }

        protected override void Dispose(bool disposing)
        {
            // Não dispõe recursos reais durante os testes.
        }
    }
}
