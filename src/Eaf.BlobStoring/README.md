# Eaf.BlobStoring

Módulo de armazenamento de BLOBs do Enterprise Application Foundation (EAF).

Este módulo implementa provedores de armazenamento de BLOBs compatíveis com a API `IBlobContainer` / `IBlobContainerFactory` do ASP.NET Boilerplate (ABP), oferecendo suporte nativo a FileSystem, Azure Blob Storage e AWS S3 (ou serviços compatíveis com S3).

## Provedores suportados

- `FileSystemBlobProvider`: salva os BLOBs em disco, com isolamento por tenant e proteção contra path traversal.
- `AzureBlobProvider`: salva os BLOBs no Azure Blob Storage, utilizando o SDK `Azure.Storage.Blobs`.
- `AwsS3BlobClient`: salva os BLOBs no AWS S3, utilizando o SDK `AWSSDK.S3`.
- `EafCloudBlobProvider`: provedor genérico de nuvem que seleciona automaticamente entre Azure e AWS de acordo com a configuração `CloudProvider`.

## Configuração

```csharp
[DependsOn(typeof(EafBlobStoringModule))]
public class MeuModulo : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Modules.EafBlobStoring().DefaultProvider = typeof(FileSystemBlobProvider);
        Configuration.Modules.EafBlobStoring().FileSystemBasePath = @"C:\EAF\Blobs";

        // Ou para Azure:
        Configuration.Modules.EafBlobStoring().DefaultProvider = typeof(AzureBlobProvider);
        Configuration.Modules.EafBlobStoring().AzureConnectionString = "UseDevelopmentStorage=true";
        Configuration.Modules.EafBlobStoring().AzureContainerName = "eaf-blobs";
        Configuration.Modules.EafBlobStoring().AzureCreateContainerIfNotExists = true;

        // Ou para AWS S3:
        Configuration.Modules.EafBlobStoring().DefaultProvider = typeof(EafCloudBlobProvider);
        Configuration.Modules.EafBlobStoring().CloudProvider = "Aws";
        Configuration.Modules.EafBlobStoring().AwsAccessKeyId = "AKIA...";
        Configuration.Modules.EafBlobStoring().AwsSecretAccessKey = "...";
        Configuration.Modules.EafBlobStoring().AwsRegion = "us-east-1";
        Configuration.Modules.EafBlobStoring().AwsBucketName = "eaf-blobs";
        Configuration.Modules.EafBlobStoring().AwsCreateBucketIfNotExists = true;

        // Ou para MinIO / LocalStack (S3-compatível):
        Configuration.Modules.EafBlobStoring().DefaultProvider = typeof(EafCloudBlobProvider);
        Configuration.Modules.EafBlobStoring().CloudProvider = "Aws";
        Configuration.Modules.EafBlobStoring().AwsServiceUrl = "http://localhost:9000";
        Configuration.Modules.EafBlobStoring().AwsForcePathStyle = true;
        Configuration.Modules.EafBlobStoring().AwsAccessKeyId = "minioadmin";
        Configuration.Modules.EafBlobStoring().AwsSecretAccessKey = "minioadmin";
        Configuration.Modules.EafBlobStoring().AwsBucketName = "eaf-blobs";
    }
}
```

> Atenção: nunca armazene credenciais no código-fonte. Prefira variáveis de ambiente, AWS IAM roles ou gerenciadores de secrets como o `Eaf.KeyVault`.

## Uso

```csharp
public class MinhaAppService : IApplicationService
{
    private readonly IBlobContainer _blobContainer;

    public MinhaAppService(IBlobContainer blobContainer)
    {
        _blobContainer = blobContainer;
    }

    public async Task SalvarAsync(string nome, byte[] bytes)
    {
        await _blobContainer.SaveAsync(nome, bytes);
    }
}
```

---

# Eaf.BlobStoring

Blob storage module for the Enterprise Application Foundation (EAF).

This module implements BLOB storage providers compatible with the ASP.NET Boilerplate (ABP) `IBlobContainer` / `IBlobContainerFactory` API, with built-in support for FileSystem, Azure Blob Storage and AWS S3 (or S3-compatible services).

## Supported providers

- `FileSystemBlobProvider`: stores BLOBs on disk, with tenant isolation and path traversal protection.
- `AzureBlobProvider`: stores BLOBs in Azure Blob Storage using the `Azure.Storage.Blobs` SDK.
- `AwsS3BlobClient`: stores BLOBs in AWS S3 using the `AWSSDK.S3` SDK.
- `EafCloudBlobProvider`: generic cloud provider that automatically selects Azure or AWS based on the `CloudProvider` configuration.

## Configuration

```csharp
[DependsOn(typeof(EafBlobStoringModule))]
public class MyModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Modules.EafBlobStoring().DefaultProvider = typeof(FileSystemBlobProvider);
        Configuration.Modules.EafBlobStoring().FileSystemBasePath = @"C:\EAF\Blobs";

        // Or for Azure:
        Configuration.Modules.EafBlobStoring().DefaultProvider = typeof(AzureBlobProvider);
        Configuration.Modules.EafBlobStoring().AzureConnectionString = "UseDevelopmentStorage=true";
        Configuration.Modules.EafBlobStoring().AzureContainerName = "eaf-blobs";
        Configuration.Modules.EafBlobStoring().AzureCreateContainerIfNotExists = true;

        // Or for AWS S3:
        Configuration.Modules.EafBlobStoring().DefaultProvider = typeof(EafCloudBlobProvider);
        Configuration.Modules.EafBlobStoring().CloudProvider = "Aws";
        Configuration.Modules.EafBlobStoring().AwsAccessKeyId = "AKIA...";
        Configuration.Modules.EafBlobStoring().AwsSecretAccessKey = "...";
        Configuration.Modules.EafBlobStoring().AwsRegion = "us-east-1";
        Configuration.Modules.EafBlobStoring().AwsBucketName = "eaf-blobs";
        Configuration.Modules.EafBlobStoring().AwsCreateBucketIfNotExists = true;

        // Or for MinIO / LocalStack (S3-compatible):
        Configuration.Modules.EafBlobStoring().DefaultProvider = typeof(EafCloudBlobProvider);
        Configuration.Modules.EafBlobStoring().CloudProvider = "Aws";
        Configuration.Modules.EafBlobStoring().AwsServiceUrl = "http://localhost:9000";
        Configuration.Modules.EafBlobStoring().AwsForcePathStyle = true;
        Configuration.Modules.EafBlobStoring().AwsAccessKeyId = "minioadmin";
        Configuration.Modules.EafBlobStoring().AwsSecretAccessKey = "minioadmin";
        Configuration.Modules.EafBlobStoring().AwsBucketName = "eaf-blobs";
    }
}
```

> Warning: never store credentials in source code. Prefer environment variables, AWS IAM roles or secret managers such as `Eaf.KeyVault`.

## Usage

```csharp
public class MyAppService : IApplicationService
{
    private readonly IBlobContainer _blobContainer;

    public MyAppService(IBlobContainer blobContainer)
    {
        _blobContainer = blobContainer;
    }

    public async Task SaveAsync(string name, byte[] bytes)
    {
        await _blobContainer.SaveAsync(name, bytes);
    }
}
```
