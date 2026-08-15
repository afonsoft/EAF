using System;
using Abp.Dependency;
using Castle.MicroKernel.Lifestyle;
using Microsoft.Extensions.DependencyInjection;

namespace Eaf.BlobStoring
{
    /// <summary>
    /// Adaptador do <see cref="IServiceScopeFactory"/> para o Castle Windsor do EAF.
    /// </summary>
    public class EafServiceScopeAdapter : IServiceScopeFactory
    {
        private readonly IIocManager _iocManager;

        /// <summary>
        /// Inicializa o adaptador.
        /// </summary>
        /// <param name="iocManager">Gerenciador de IoC do EAF.</param>
        public EafServiceScopeAdapter(IIocManager iocManager)
        {
            _iocManager = iocManager;
        }

        /// <inheritdoc />
        public IServiceScope CreateScope()
        {
            var scope = _iocManager.IocContainer.BeginScope();
            return new EafServiceScope(scope, _iocManager);
        }
    }

    /// <summary>
    /// Implementação do <see cref="IServiceProvider"/> baseada no Castle Windsor do EAF.
    /// </summary>
    public class EafServiceProvider : IServiceProvider
    {
        private readonly IIocManager _iocManager;

        /// <summary>
        /// Inicializa o provedor de serviços.
        /// </summary>
        /// <param name="iocManager">Gerenciador de IoC do EAF.</param>
        public EafServiceProvider(IIocManager iocManager)
        {
            _iocManager = iocManager;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (!_iocManager.IsRegistered(serviceType))
            {
                return null;
            }

            return _iocManager.Resolve(serviceType);
        }
    }

    /// <summary>
    /// Escopo de serviços do Castle Windsor.
    /// </summary>
    public class EafServiceScope : IServiceScope
    {
        private readonly IDisposable _scope;
        private readonly IIocManager _iocManager;
        private bool _disposed;

        /// <inheritdoc />
        public IServiceProvider ServiceProvider => new EafServiceProvider(_iocManager);

        /// <summary>
        /// Inicializa um novo escopo.
        /// </summary>
        /// <param name="scope">Escopo do Castle Windsor.</param>
        /// <param name="iocManager">Gerenciador de IoC do EAF.</param>
        public EafServiceScope(IDisposable scope, IIocManager iocManager)
        {
            _scope = scope;
            _iocManager = iocManager;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera os recursos gerenciados do escopo.
        /// </summary>
        /// <param name="disposing">Indica se está liberando recursos gerenciados.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _scope.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
