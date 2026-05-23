using Castle.Core.Logging;
using Abp.Dependency;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.ObjectMapping;
using Eaf.Middleware.Localization;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Threading.Tasks;
using Abp;

namespace Eaf.WebHooks
{
    /// <summary>
    /// Representa a classe EafWebHookReceiver.
    /// </summary>
    public abstract class EafWebHookReceiver : IDomainService, ISingletonDependency
    {
        private ILocalizationSource _localizationSource;

        private IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// IocManager for dependency
        /// </summary>
        public IIocManager IocManager { get; }

        protected EafWebHookReceiver()
        {
            IocManager = Abp.Dependency.IocManager.Instance;
            Logger = NullLogger.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
            EventBus = NullEventBus.Instance;
            ObjectMapper = NullObjectMapper.Instance;
            LocalizationSourceName = MiddlewareLocalizationHelper.DefaultSourceName;
            SetDependencies();
        }

        private void SetDependencies()
        {
            if (IocManager.IsRegistered<ILoggerFactory>())
            {
                Logger = IocManager.Resolve<ILoggerFactory>().Create(typeof(EafWebHookReceiver));
            }

            if (IocManager.IsRegistered<IEventBus>())
            {
                EventBus = IocManager.Resolve<IEventBus>();
            }
            if (IocManager.IsRegistered<ILocalizationManager>())
            {
                LocalizationManager = IocManager.Resolve<ILocalizationManager>();
            }

            if (IocManager.IsRegistered<IObjectMapper>())
            {
                ObjectMapper = IocManager.Resolve<IObjectMapper>();
            }
        }

        /// <summary>
        /// Gets/sets name of the localization source that is used in this application service. It
        /// must be set in order to use <see cref="L(string)"/> and <see
        /// cref="L(string,CultureInfo)"/> methods.
        /// </summary>
        protected string LocalizationSourceName { get; set; }

        /// <summary>
        /// Name of WebHook Receiver
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// HttpContext of request
        /// </summary>
        public HttpContext context { get; set; }

        /// <summary>
        /// Gets the event bus.
        /// </summary>
        public IEventBus EventBus { get; set; }

        /// <summary>
        /// Reference to the localization manager.
        /// </summary>
        public ILocalizationManager LocalizationManager { protected get; set; }

        /// <summary>
        /// Reference to the logger to write logs.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Reference to the object to object mapper.
        /// </summary>
        public IObjectMapper ObjectMapper { get; set; }

        /// <summary>
        /// Reference to <see cref="IUnitOfWorkManager"/>.
        /// </summary>
        public IUnitOfWorkManager UnitOfWorkManager
        {
            get
            {
                if (_unitOfWorkManager == null)
                {
                    throw new AbpException("Must set UnitOfWorkManager before use it.");
                }

                return _unitOfWorkManager;
            }
            set { _unitOfWorkManager = value; }
        }

        /// <summary>
        /// Gets current unit of work.
        /// </summary>
        protected IActiveUnitOfWork CurrentUnitOfWork
        { get { return UnitOfWorkManager.Current; } }

        /// <summary>
        /// Gets localization source. It's valid if <see cref="LocalizationSourceName"/> is set.
        /// </summary>
        protected ILocalizationSource LocalizationSource
        {
            get
            {
                if (LocalizationSourceName == null)
                {
                    throw new AbpException("Must set LocalizationSourceName before, in order to get LocalizationSource");
                }

                if (_localizationSource == null || _localizationSource.Name != LocalizationSourceName)
                {
                    _localizationSource = LocalizationManager.GetSource(LocalizationSourceName);
                }

                return _localizationSource;
            }
        }

        /// <summary>
        /// Gets localized string for given key name and current language.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name);
        }

        /// <summary>
        /// Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, params object[] args)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, args);
        }

        /// <summary>
        /// Gets localized string for given key name and specified culture information.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, CultureInfo culture)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, culture);
        }

        /// <summary>
        /// Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, CultureInfo culture, params object[] args)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, culture, args);
        }

        [UnitOfWork]
        public abstract Task ProcessRequest(string requestBody);
    }
}