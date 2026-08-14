using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Abp.BlobStoring;
using Abp.Runtime.Session;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Eaf.BlobStoring.Providers;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Xunit;

namespace Eaf.BlobStoring.Tests
{
    /// <summary>
    /// Testes unitários do cliente AWS S3 para armazenamento de BLOBs.
    /// </summary>
    public class AwsS3BlobClientTests
    {
        /// <summary>
        /// Dado um cliente AWS, quando instanciar, então deve implementar ICloudBlobClient.
        /// </summary>
        [Fact]
        public void Dado_ClienteAws_Quando_Instanciar_Entao_Deve_ImplementarICloudBlobClient()
        {
            var client = new AwsS3BlobClient(NullAbpSession.Instance, Substitute.For<IBlobNormalizeNamingService>());

            client.ShouldBeAssignableTo<ICloudBlobClient>();
        }

        /// <summary>
        /// Dado um cliente AWS configurado, quando salvar, então deve chamar PutObject no S3.
        /// </summary>
        [Fact]
        public async Task Dado_ClienteAwsConfigurado_Quando_Salvar_Entao_Deve_ChamarPutObject()
        {
            var s3Client = Substitute.For<IAmazonS3>();
            s3Client.PutObjectAsync(Arg.Any<PutObjectRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new PutObjectResponse()));

            var client = new TestAwsS3BlobClient(s3Client);
            var args = CreateSaveArgs("blob.txt", new byte[] { 1, 2, 3 });

            await client.SaveAsync(args);

            await s3Client.Received(1).PutObjectAsync(
                Arg.Is<PutObjectRequest>(r => r.BucketName == "my-bucket" && r.Key == "host/blob.txt"),
                Arg.Any<CancellationToken>());
        }

        /// <summary>
        /// Dado um cliente AWS configurado, quando verificar existência de um objeto presente, então deve retornar true.
        /// </summary>
        [Fact]
        public async Task Dado_ClienteAwsConfiguradoComObjetoPresente_Quando_Existir_Entao_Deve_RetornarTrue()
        {
            var s3Client = Substitute.For<IAmazonS3>();
            s3Client.GetObjectMetadataAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new GetObjectMetadataResponse()));

            var client = new TestAwsS3BlobClient(s3Client);
            var args = CreateExistsArgs("blob.txt");

            var result = await client.ExistsAsync(args);

            result.ShouldBeTrue();
        }

        /// <summary>
        /// Dado um cliente AWS configurado, quando verificar existência de um objeto ausente, então deve retornar false.
        /// </summary>
        [Fact]
        public async Task Dado_ClienteAwsConfiguradoComObjetoAusente_Quando_Existir_Entao_Deve_RetornarFalse()
        {
            var s3Client = Substitute.For<IAmazonS3>();
            s3Client.GetObjectMetadataAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Throws(new AmazonS3Exception("NotFound") { StatusCode = HttpStatusCode.NotFound });

            var client = new TestAwsS3BlobClient(s3Client);
            var args = CreateExistsArgs("blob.txt");

            var result = await client.ExistsAsync(args);

            result.ShouldBeFalse();
        }

        /// <summary>
        /// Dado um cliente AWS configurado, quando obter um BLOB existente, então deve retornar o stream.
        /// </summary>
        [Fact]
        public async Task Dado_ClienteAwsConfigurado_Quando_Obter_Entao_Deve_RetornarStream()
        {
            var bytes = new byte[] { 9, 8, 7 };
            var response = new GetObjectResponse
            {
                ResponseStream = new MemoryStream(bytes)
            };

            var s3Client = Substitute.For<IAmazonS3>();
            s3Client.GetObjectAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(response));

            var client = new TestAwsS3BlobClient(s3Client);
            var args = CreateGetArgs("blob.txt");

            var stream = await client.GetOrNullAsync(args);

            stream.ShouldNotBeNull();
            var resultBytes = new byte[bytes.Length];
            await stream.ReadExactlyAsync(resultBytes, CancellationToken.None);
            resultBytes.ShouldBe(bytes);
        }

        /// <summary>
        /// Dado um cliente AWS configurado, quando deletar, então deve chamar DeleteObject no S3.
        /// </summary>
        [Fact]
        public async Task Dado_ClienteAwsConfigurado_Quando_Deletar_Entao_Deve_ChamarDeleteObject()
        {
            var s3Client = Substitute.For<IAmazonS3>();
            s3Client.DeleteObjectAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new DeleteObjectResponse()));

            var client = new TestAwsS3BlobClient(s3Client);
            var args = CreateDeleteArgs("blob.txt");

            await client.DeleteAsync(args);

            await s3Client.Received(1).DeleteObjectAsync(
                Arg.Is("my-bucket"),
                Arg.Is("host/blob.txt"),
                Arg.Any<CancellationToken>());
        }

        private static BlobProviderSaveArgs CreateSaveArgs(string blobName, byte[] bytes)
        {
            var configuration = new BlobContainerConfiguration { ProviderType = typeof(AwsS3BlobClient) };
            configuration.SetConfiguration(AwsS3BlobProviderConfiguration.BucketName, "my-bucket");
            configuration.SetConfiguration(AwsS3BlobProviderConfiguration.Region, "us-east-1");

            return new BlobProviderSaveArgs(
                "default",
                configuration,
                blobName,
                new MemoryStream(bytes),
                false,
                CancellationToken.None);
        }

        private static BlobProviderExistsArgs CreateExistsArgs(string blobName)
        {
            var configuration = new BlobContainerConfiguration { ProviderType = typeof(AwsS3BlobClient) };
            configuration.SetConfiguration(AwsS3BlobProviderConfiguration.BucketName, "my-bucket");
            configuration.SetConfiguration(AwsS3BlobProviderConfiguration.Region, "us-east-1");

            return new BlobProviderExistsArgs(
                "default",
                configuration,
                blobName,
                CancellationToken.None);
        }

        private static BlobProviderGetArgs CreateGetArgs(string blobName)
        {
            var configuration = new BlobContainerConfiguration { ProviderType = typeof(AwsS3BlobClient) };
            configuration.SetConfiguration(AwsS3BlobProviderConfiguration.BucketName, "my-bucket");
            configuration.SetConfiguration(AwsS3BlobProviderConfiguration.Region, "us-east-1");

            return new BlobProviderGetArgs(
                "default",
                configuration,
                blobName,
                CancellationToken.None);
        }

        private static BlobProviderDeleteArgs CreateDeleteArgs(string blobName)
        {
            var configuration = new BlobContainerConfiguration { ProviderType = typeof(AwsS3BlobClient) };
            configuration.SetConfiguration(AwsS3BlobProviderConfiguration.BucketName, "my-bucket");
            configuration.SetConfiguration(AwsS3BlobProviderConfiguration.Region, "us-east-1");

            return new BlobProviderDeleteArgs(
                "default",
                configuration,
                blobName,
                CancellationToken.None);
        }

        /// <summary>
        /// Subclasse de teste que substitui a criação do cliente S3 por um mock.
        /// </summary>
        private class TestAwsS3BlobClient : AwsS3BlobClient
        {
            private readonly IAmazonS3 _s3Client;

            public TestAwsS3BlobClient(IAmazonS3 s3Client)
                : base(NullAbpSession.Instance, Substitute.For<IBlobNormalizeNamingService>())
            {
                _s3Client = s3Client;
            }

            protected override Task<IAmazonS3> GetS3ClientAsync(BlobProviderArgs args)
            {
                return Task.FromResult(_s3Client);
            }
        }
    }
}
