using Eaf.BlobStoring.Naming;
using Eaf.BlobStoring.Providers;
using Shouldly;
using Xunit;

namespace Eaf.BlobStoring.Tests
{
    /// <summary>
    /// Testes da configuração do módulo Eaf.BlobStoring.
    /// </summary>
    public class EafBlobStoringConfigurationTests
    {
        /// <summary>
        /// Dado uma configuração padrão, quando instanciar, então o provedor padrão deve ser FileSystem.
        /// </summary>
        [Fact]
        public void Dado_ConfiguracaoPadrao_Quando_Instanciar_Entao_ProvedorPadraoEhFileSystem()
        {
            var configuration = new EafBlobStoringConfiguration();

            configuration.DefaultProvider.ShouldBe(typeof(FileSystemBlobProvider));
            configuration.FileSystemAppendContainerNameToBasePath.ShouldBeTrue();
            configuration.AzureCreateContainerIfNotExists.ShouldBeFalse();
            configuration.CloudProvider.ShouldBe("Azure");
            configuration.AwsRegion.ShouldBe("us-east-1");
            configuration.AwsForcePathStyle.ShouldBeFalse();
            configuration.AwsCreateBucketIfNotExists.ShouldBeFalse();
            configuration.NamingNormalizers.ShouldContain(typeof(EafDefaultBlobNamingNormalizer));
        }
    }
}
