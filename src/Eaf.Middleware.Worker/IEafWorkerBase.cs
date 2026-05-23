using Abp.Dependency;
using Abp.Domain.Services;
using Abp.Events.Bus;
using Abp.Localization;
using Abp.ObjectMapping;
using Castle.Core.Logging;
using Microsoft.Extensions.Hosting;

namespace Eaf
{
    /// <summary>
    /// Representa a interface IEafWorkerBase.
    /// </summary>
    public interface IEafWorkerBase : IHostedService, IDomainService, ISingletonDependency
    {
        /// <summary>
        /// Obtém ou define IocManager.
        /// </summary>
        public IIocManager IocManager { get; set; }

        /// <summary>
        /// Gets the event bus.
        /// </summary>
        public IEventBus EventBus { get; set; }

        /// <summary>
        /// Reference to the localization manager.
        /// </summary>
        public ILocalizationManager LocalizationManager { set; }

        /// <summary>
        /// Reference to the logger to write logs.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Reference to the object to object mapper.
        /// </summary>
        public IObjectMapper ObjectMapper { get; set; }
    }
}