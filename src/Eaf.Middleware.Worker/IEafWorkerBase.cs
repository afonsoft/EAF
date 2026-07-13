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
    /// Interface base para workers EAF com suporte a logging e infraestrutura ABP.
    /// </summary>
    public interface IEafWorkerBase : IHostedService, IDomainService, ISingletonDependency
    {
        /// <summary>
        /// Obtém ou define o logger.
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// Obtém ou define o event bus.
        /// </summary>
        IEventBus EventBus { get; set; }

        /// <summary>
        /// Obtém ou define o localization manager.
        /// </summary>
        ILocalizationManager LocalizationManager { get; set; }

        /// <summary>
        /// Obtém ou define o object mapper.
        /// </summary>
        IObjectMapper ObjectMapper { get; set; }
    }
}