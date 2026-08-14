using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Abp.BlobStoring;
using Abp.Dependency;
using Abp.Runtime.Session;
using Eaf.BlobStoring.Providers;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.BlobStoring.Tests
{
    /// <summary>
    /// Testes unitários do provedor de BLOBs em nuvem genérico do EAF.
    /// </summary>
    public class EafCloudBlobProviderTests
    {
        /// <summary>
        /// Dado o provider cloud, quando instanciar, então deve implementar IBlobProvider.
        /// </summary>
        [Fact]
        public void Dado_ProviderCloud_Quando_Instanciar_Entao_Deve_ImplementarIBlobProvider()
        {
            var provider = new EafCloudBlobProvider(Substitute.For<IIocManager>());

            provider.ShouldBeAssignableTo<IBlobProvider>();
        }

        /// <summary>
        /// Dado uma configuração para Azure, quando salvar, então deve delegar para o cliente Azure.
        /// </summary>
        [Fact]
        public async Task Dado_ConfiguracaoParaAzure_Quando_Salvar_Entao_Deve_DelegarParaAzureBlobClient()
        {
            var azureClient = Substitute.For<AzureBlobClient>(NullAbpSession.Instance, Substitute.For<IBlobNormalizeNamingService>());
            azureClient.SaveAsync(Arg.Any<BlobProviderSaveArgs>()).Returns(Task.CompletedTask);

            var iocManager = Substitute.For<IIocManager>();
            iocManager.Resolve<AzureBlobClient>().Returns(azureClient);

            var provider = new EafCloudBlobProvider(iocManager);
            var configuration = new BlobContainerConfiguration { ProviderType = typeof(EafCloudBlobProvider) };
            configuration.SetConfiguration(CloudBlobProviderConfiguration.CloudProvider, "Azure");
            var args = new BlobProviderSaveArgs(
                "default",
                configuration,
                "blob.txt",
                new MemoryStream(new byte[] { 1 }),
                false,
                CancellationToken.None);

            await provider.SaveAsync(args);

            await azureClient.Received(1).SaveAsync(args);
        }

        /// <summary>
        /// Dado uma configuração para AWS, quando salvar, então deve delegar para o cliente AWS.
        /// </summary>
        [Fact]
        public async Task Dado_ConfiguracaoParaAws_Quando_Salvar_Entao_Deve_DelegarParaAwsS3BlobClient()
        {
            var awsClient = Substitute.For<AwsS3BlobClient>(NullAbpSession.Instance, Substitute.For<IBlobNormalizeNamingService>());
            awsClient.SaveAsync(Arg.Any<BlobProviderSaveArgs>()).Returns(Task.CompletedTask);

            var iocManager = Substitute.For<IIocManager>();
            iocManager.Resolve<AwsS3BlobClient>().Returns(awsClient);

            var provider = new EafCloudBlobProvider(iocManager);
            var configuration = new BlobContainerConfiguration { ProviderType = typeof(EafCloudBlobProvider) };
            configuration.SetConfiguration(CloudBlobProviderConfiguration.CloudProvider, "Aws");
            var args = new BlobProviderSaveArgs(
                "default",
                configuration,
                "blob.txt",
                new MemoryStream(new byte[] { 1 }),
                false,
                CancellationToken.None);

            await provider.SaveAsync(args);

            await awsClient.Received(1).SaveAsync(args);
        }

        /// <summary>
        /// Dado uma configuração de provider não suportado, quando obter cliente, então deve lançar exceção.
        /// </summary>
        [Fact]
        public void Dado_ProviderNaoSuportado_Quando_ObterCliente_Entao_Deve_LancarExcecao()
        {
            var provider = new EafCloudBlobProvider(Substitute.For<IIocManager>());
            var configuration = new BlobContainerConfiguration { ProviderType = typeof(EafCloudBlobProvider) };
            configuration.SetConfiguration(CloudBlobProviderConfiguration.CloudProvider, "Gcp");
            var args = new BlobProviderGetArgs(
                "default",
                configuration,
                "blob.txt",
                CancellationToken.None);

            Should.ThrowAsync<System.NotSupportedException>(() => provider.GetOrNullAsync(args));
        }
    }
}
