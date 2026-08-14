using System;
using Abp;
using Abp.BlobStoring;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading;
using Castle.MicroKernel.Registration;
using Eaf.BlobStoring.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Eaf.BlobStoring
{
    /// <summary>
    /// Módulo de armazenamento de BLOBs do EAF.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule), typeof(AbpBlobStoringModule))]
    public class EafBlobStoringModule : AbpModule
    {
        /// <summary>
        /// Pré-inicializa o módulo registrando as configurações e o suporte a escopos.
        /// </summary>
        public override void PreInitialize()
        {
            if (!IocManager.IsRegistered<EafBlobStoringConfiguration>())
            {
                IocManager.IocContainer.Register(
                    Component.For<EafBlobStoringConfiguration, IEafBlobStoringConfiguration>()
                             .ImplementedBy<EafBlobStoringConfiguration>()
                             .LifestyleSingleton()
                );
            }

            if (!IocManager.IsRegistered<IServiceScopeFactory>())
            {
                IocManager.IocContainer.Register(
                    Component.For<IServiceScopeFactory>()
                             .ImplementedBy<EafServiceScopeAdapter>()
                             .LifestyleSingleton()
                );
            }

            if (!IocManager.IsRegistered<IServiceProvider>())
            {
                IocManager.IocContainer.Register(
                    Component.For<IServiceProvider>()
                             .Instance(new EafServiceProvider(IocManager))
                             .LifestyleSingleton()
                );
            }

            if (!IocManager.IsRegistered<ICancellationTokenProvider>())
            {
                IocManager.IocContainer.Register(
                    Component.For<ICancellationTokenProvider>()
                             .Instance(NullCancellationTokenProvider.Instance)
                             .LifestyleSingleton()
                );
            }
        }

        /// <summary>
        /// Inicializa o módulo registrando componentes por convenção.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafBlobStoringModule).GetAssembly());
        }

        /// <summary>
        /// Pós-inicializa o módulo aplicando a configuração padrão do EAF ao ABP Blob Storing.
        /// </summary>
        public override void PostInitialize()
        {
            var options = IocManager.Resolve<AbpBlobStoringOptions>();
            var configuration = IocManager.Resolve<IEafBlobStoringConfiguration>();

            options.Containers.ConfigureDefault(container =>
            {
                container.ProviderType = configuration.DefaultProvider;
                container.NamingNormalizers.Clear();

                foreach (var normalizer in configuration.NamingNormalizers)
                {
                    container.NamingNormalizers.Add(normalizer);
                }

                container.SetConfiguration(FileSystemBlobProviderConfiguration.BasePath, configuration.FileSystemBasePath);
                container.SetConfiguration(FileSystemBlobProviderConfiguration.AppendContainerNameToBasePath, configuration.FileSystemAppendContainerNameToBasePath);
                container.SetConfiguration(FileSystemBlobProviderConfiguration.Isolation, configuration.FileSystemIsolation);
                container.SetConfiguration(AzureBlobProviderConfiguration.ConnectionString, configuration.AzureConnectionString ?? string.Empty);
                container.SetConfiguration(AzureBlobProviderConfiguration.ContainerName, configuration.AzureContainerName);
                container.SetConfiguration(AzureBlobProviderConfiguration.CreateContainerIfNotExists, configuration.AzureCreateContainerIfNotExists);

                container.SetConfiguration(CloudBlobProviderConfiguration.CloudProvider, configuration.CloudProvider ?? string.Empty);
                container.SetConfiguration(AwsS3BlobProviderConfiguration.AccessKeyId, configuration.AwsAccessKeyId ?? string.Empty);
                container.SetConfiguration(AwsS3BlobProviderConfiguration.SecretAccessKey, configuration.AwsSecretAccessKey ?? string.Empty);
                container.SetConfiguration(AwsS3BlobProviderConfiguration.Region, configuration.AwsRegion ?? string.Empty);
                container.SetConfiguration(AwsS3BlobProviderConfiguration.BucketName, configuration.AwsBucketName ?? string.Empty);
                container.SetConfiguration(AwsS3BlobProviderConfiguration.ServiceUrl, configuration.AwsServiceUrl ?? string.Empty);
                container.SetConfiguration(AwsS3BlobProviderConfiguration.ForcePathStyle, configuration.AwsForcePathStyle);
                container.SetConfiguration(AwsS3BlobProviderConfiguration.CreateBucketIfNotExists, configuration.AwsCreateBucketIfNotExists);
            });
        }
    }
}
