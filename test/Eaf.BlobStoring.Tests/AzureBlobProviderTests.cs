using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Abp.BlobStoring;
using Abp.Runtime.Session;
using Eaf.BlobStoring.Providers;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.BlobStoring.Tests
{
    /// <summary>
    /// Testes unitários do provedor Azure Blob Storage.
    /// </summary>
    public class AzureBlobProviderTests
    {
        /// <summary>
        /// Dado o provedor Azure, quando instanciar, então deve implementar IBlobProvider.
        /// </summary>
        [Fact]
        public void Dado_ProvedorAzure_Quando_Instanciar_Entao_Deve_ImplementarIBlobProvider()
        {
            var client = new AzureBlobClient(NullAbpSession.Instance, Substitute.For<IBlobNormalizeNamingService>());
            var provider = new AzureBlobProvider(client);

            provider.ShouldBeAssignableTo<IBlobProvider>();
        }

        /// <summary>
        /// Dado o provedor Azure sem connection string, quando salvar, então deve lançar exceção.
        /// </summary>
        [Fact]
        public async Task Dado_ProvedorAzureSemConnectionString_Quando_Salvar_Entao_Deve_LancarExcecao()
        {
            var client = new AzureBlobClient(NullAbpSession.Instance, Substitute.For<IBlobNormalizeNamingService>());
            var provider = new AzureBlobProvider(client);
            var configuration = new BlobContainerConfiguration { ProviderType = typeof(AzureBlobProvider) };
            var args = new BlobProviderSaveArgs(
                "default",
                configuration,
                "blob.txt",
                new MemoryStream(new byte[] { 1 }),
                false,
                CancellationToken.None);

            await Should.ThrowAsync<System.ArgumentException>(() => provider.SaveAsync(args));
        }
    }
}
