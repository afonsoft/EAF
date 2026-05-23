using Castle.Core.Logging;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Localization;
using Abp.Localization.Sources;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Text;
using Abp.ObjectMapping;
using Abp;
using System;

namespace Eaf.Middleware.Worker
{
    /// <summary>
    /// Base class for all IHostedService in Eaf system.
    /// </summary>
    public abstract class EafWorkerBase : BackgroundService, IEafWorkerBase
    {
        private ILocalizationSource _localizationSource;

        private IUnitOfWorkManager _unitOfWorkManager;

        protected EafWorkerBase()
        {
            Logger = NullLogger.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
            EventBus = NullEventBus.Instance;
            ObjectMapper = NullObjectMapper.Instance;
            LocalizationSourceName = DefaultLocalizationSourceName;
            SetDependencies();
        }

        private void SetDependencies()
        {
            if (IocManager.IsRegistered<ILoggerFactory>())
            {
                Logger = IocManager.Resolve<ILoggerFactory>().Create(typeof(EafWorkerBase));
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
        /// Nome do source padrão de localização.
        /// </summary>
        private const string DefaultLocalizationSourceName = "EafCore";

        /// <summary>
        /// Nomes dos sources de localização na ordem de busca (fallback).
        /// </summary>
        private static readonly string[] LocalizationSourceNames =
        [
            "EafCore",
            "Abp",
            "AbpZero",
            "AbpWeb",
            "EafAzureActiveDirectory",
            "EafLdap"
        ];

        /// <summary>
        /// Gets/sets name of the localization source that is used in this application service. It
        /// must be set in order to use <see cref="L(string)"/> and <see
        /// cref="L(string,CultureInfo)"/> methods.
        /// </summary>
        protected string LocalizationSourceName { get; set; }

        /// <summary>
        /// Gets localized string for given key name and current language.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name)
        {
            return LocalizeWithFallback(name, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, params object[] args)
        {
            var result = LocalizeWithFallback(name, CultureInfo.CurrentUICulture);
            return args != null && args.Length > 0 ? string.Format(result, args) : result;
        }

        /// <summary>
        /// Gets localized string for given key name and specified culture information.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, CultureInfo culture)
        {
            return LocalizeWithFallback(name, culture);
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
            var result = LocalizeWithFallback(name, culture);
            return args != null && args.Length > 0 ? string.Format(result, args) : result;
        }

        /// <summary>
        /// Busca a string localizada em múltiplos sources com fallback.
        /// </summary>
        private string LocalizeWithFallback(string key, CultureInfo culture)
        {
            if (LocalizationManager == null || string.IsNullOrEmpty(key))
            {
                return key;
            }

            foreach (var sourceName in LocalizationSourceNames)
            {
                try
                {
                    var source = LocalizationManager.GetSource(sourceName);
                    var result = source.GetStringOrNull(key, culture);
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch (Exception)
                {
                    // Source não registrado, tentar o próximo
                }
            }

            return key;
        }
    }
}