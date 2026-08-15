using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eaf.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Eaf.SignalR.Tests.Notifications
{
    /// <summary>
    /// Fakes para testes de notificação SignalR.
    /// </summary>
    public class FakeHubContext : IHubContext<EafCommonHub>
    {
        public IHubClients Clients { get; }

        public IGroupManager Groups => throw new NotImplementedException();

        /// <summary>
        /// Inicializa uma nova instância.
        /// </summary>
        public FakeHubContext(IHubClients clients)
        {
            Clients = clients;
        }
    }

    /// <summary>
    /// Fake de IHubClients para testes.
    /// </summary>
    public class FakeHubClients : IHubClients
    {
        private readonly ISingleClientProxy _clientProxy;

        /// <summary>
        /// Inicializa uma nova instância.
        /// </summary>
        public FakeHubClients(ISingleClientProxy clientProxy)
        {
            _clientProxy = clientProxy;
        }

        public IClientProxy All => throw new NotImplementedException();

        public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => throw new NotImplementedException();

        IClientProxy IHubClients<IClientProxy>.Client(string connectionId) => _clientProxy;

        public ISingleClientProxy Client(string connectionId) => _clientProxy;

        public IClientProxy Clients(IReadOnlyList<string> connectionIds) => _clientProxy;

        public IClientProxy Group(string groupName) => throw new NotImplementedException();

        public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => throw new NotImplementedException();

        public IClientProxy Groups(IReadOnlyList<string> groupNames) => throw new NotImplementedException();

        public IClientProxy Others => throw new NotImplementedException();

        public IClientProxy User(string userId) => throw new NotImplementedException();

        public IClientProxy Users(IReadOnlyList<string> userIds) => throw new NotImplementedException();
    }

    /// <summary>
    /// Fake de ISingleClientProxy para capturar chamadas SignalR.
    /// </summary>
    public class FakeSingleClientProxy : ISingleClientProxy
    {
        /// <summary>
        /// Chamadas realizadas ao SendCoreAsync.
        /// </summary>
        public List<(string Method, object[] Args)> SendCoreAsyncCalls { get; } = new List<(string, object[])>();

        /// <summary>
        /// Chamadas realizadas ao InvokeCoreAsync.
        /// </summary>
        public List<(string Method, object[] Args)> InvokeCoreAsyncCalls { get; } = new List<(string, object[])>();

        public Task SendCoreAsync(string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            SendCoreAsyncCalls.Add((methodName, args));
            return Task.CompletedTask;
        }

        public Task<T> InvokeCoreAsync<T>(string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            InvokeCoreAsyncCalls.Add((methodName, args));
            return Task.FromResult<T>(default);
        }
    }
}
