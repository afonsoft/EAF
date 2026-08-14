# Eaf.BlobStoring

Módulo de armazenamento de BLOBs do Enterprise Application Foundation (EAF).

Este módulo implementa provedores de armazenamento de BLOBs compatíveis com a API `IBlobContainer` / `IBlobContainerFactory` do ASP.NET Boilerplate (ABP), oferecendo suporte nativo a FileSystem e Azure Blob Storage.

## Provedores suportados

- `FileSystemBlobProvider`: salva os BLOBs em disco, com isolamento por tenant e proteção contra path traversal.
- `AzureBlobProvider`: salva os BLOBs no Azure Blob Storage, utilizando o SDK `Azure.Storage.Blobs`.

## Configuração

```csharp
[DependsOn(typeof(EafBlobStoringModule))]
public class MeuModulo : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Modules.EafBlobStoring().DefaultProvider = typeof(FileSystemBlobProvider);
        Configuration.Modules.EafBlobStoring().FileSystemBasePath = @"C:\\EAF\\Blobs";

        // Ou para Azure:
        Configuration.Modules.EafBlobStoring().DefaultProvider = typeof(AzureBlobProvider);
        Configuration.Modules.EafBlobStoring().AzureConnectionString = "UseDevelopmentStorage=true";
        Configuration.Modules.EafBlobStoring().AzureContainerName = "eaf-blobs";
        Configuration.Modules.EafBlobStoring().AzureCreateContainerIfNotExists = true;
    }
}
```

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

This module implements BLOB storage providers compatible with the ASP.NET Boilerplate (ABP) `IBlobContainer` / `IBlobContainerFactory` API, with built-in FileSystem and Azure Blob Storage support.

## Supported providers

- `FileSystemBlobProvider`: stores BLOBs on disk, with tenant isolation and path traversal protection.
- `AzureBlobProvider`: stores BLOBs in Azure Blob Storage using the `Azure.Storage.Blobs` SDK.

## Configuration

```csharp
[DependsOn(typeof(EafBlobStoringModule))]
public class MyModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Modules.EafBlobStoring().DefaultProvider = typeof(FileSystemBlobProvider);
        Configuration.Modules.EafBlobStoring().FileSystemBasePath = @"C:\\EAF\\Blobs";

        // Or for Azure:
        Configuration.Modules.EafBlobStoring().DefaultProvider = typeof(AzureBlobProvider);
        Configuration.Modules.EafBlobStoring().AzureConnectionString = "UseDevelopmentStorage=true";
        Configuration.Modules.EafBlobStoring().AzureContainerName = "eaf-blobs";
        Configuration.Modules.EafBlobStoring().AzureCreateContainerIfNotExists = true;
    }
}
```

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
