# EAF — Eaf.BlobStoring Module

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Eaf.BlobStoring Module |
| Product / System | EAF Middleware |
| Module / Bounded Context | File / Binary Storage |
| Change type | Feature |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-blob-storing` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF does not have a reusable binary-large-object (BLOB) storage abstraction. File uploads, profile pictures, and documents are handled ad-hoc, making it hard to switch between filesystem, Azure Blob, and future providers.

### Objective

Create `Eaf.BlobStoring` following the ABP `IBlobContainer` / `IBlobContainerFactory` pattern, with built-in FileSystem and Azure Blob providers, and optional database provider.

### Expected outcome

- `src/Eaf.BlobStoring/` project.
- `IBlobContainer`, `IBlobContainerFactory`, `IBlobProvider`, `BlobContainer`.
- FileSystem and Azure Blob providers.
- Unit/integration tests and README.md.

### Out of scope

- Image processing / thumbnails.
- CDN distribution.
- Frontend upload UI (uses existing `p-fileUpload` or new spec).

## 2. Agent Role

Senior .NET/ABP engineer. Follow ABP BLOB storing public documentation and EAF module conventions.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not invent a new BLOB API; reuse ABP `IBlobContainer` semantics; do not push NuGet packages.

## 4. Product Context

### Functional context

Application services and controllers need to save/retrieve files without knowing the storage backend. ABP's BLOB storing abstraction is the reference pattern.

### Technical context

- EAF uses `ISettingManager` for configuration.
- `Eaf.KeyVault` can store provider secrets.
- `Eaf.Middleware.Web.Core` hosts controllers and static files.

### Relevant stack

- C# 14 / .NET 10 / ABP 10.5
- Azure.Storage.Blobs (optional provider)
- System.IO (FileSystem provider)
- xUnit / Shouldly / NSubstitute

### Relevant files or directories

```text
src/Eaf.KeyVault/
src/Eaf.Middleware.Web.Core/
Templates/Api/
Templates/Angular/Eaf.ProjectName.UI/
```

### Context files the agent must read before implementation

- `src/Eaf.SqlServerCache/Runtime/Caching/SqlServer/EafSqlServerCache.cs` for naming/compression patterns.
- `common.props`

## 5. Task Definition

### Main task

Create `Eaf.BlobStoring` middleware module.

### Subtasks

1. Create project and module.
2. Define `IBlobContainer`, `IBlobContainerFactory`, `IBlobProvider`, `IBlobNamingNormalizer`.
3. Implement default `BlobContainer`.
4. Implement `FileSystemBlobProvider`.
5. Implement `AzureBlobProvider`.
6. Add `BlobStoringConfiguration`.
7. Add unit/integration tests.
8. Add README.md.

### Do not do

- Do not add image processing.
- Do not change existing upload endpoints without a migration note.

## 6. Functional Requirements

### FR-001: BLOB save and read

**Description:** Save and read BLOBs by name through `IBlobContainer`.

**Rules:**

- Names are normalized (no path traversal).
- Provider selected by container configuration.
- Support `Stream` and `byte[]`.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `name` | `string` | yes | Normalized, unique within container |
| `stream` / `bytes` | `Stream` / `byte[]` | yes | Non-null |
| `overrideExisting` | `bool` | no | Default `false` |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `SaveAsync` | `Task` | Persists BLOB |
| `GetAsync` | `Task<Stream>` | Returns readable stream |
| `GetAllBytesOrNullAsync` | `Task<byte[]>` | Returns bytes or null |

**Acceptance criteria:**

- [ ] Save/read round-trip works for FileSystem and Azure providers.
- [ ] `overrideExisting=false` throws `BlobAlreadyExistsException` on duplicate name.

### FR-002: Typed and named containers

**Description:** Support typed containers via `[BlobContainerName("profile-pictures")]` and named containers via factory.

**Rules:**

- Typed container is a marker class/attribute.
- `IBlobContainerFactory.Create(name)` returns a named container.
- Default container name is `"default"`.

**Acceptance criteria:**

- [ ] `Resolve<IBlobContainer<ProfilePictureContainer>>()` works.
- [ ] `factory.Create("invoices")` works.

### FR-003: Provider configuration

**Description:** Configure provider per container using `EafBlobStoringConfiguration`.

**Rules:**

- Default provider is FileSystem unless configured otherwise.
- Azure provider uses `ConnectionStrings:AzureBlob` or `Eaf:BlobStoring:Azure:ConnectionString`.

**Acceptance criteria:**

- [ ] Switch provider per container in `PreInitialize`.
- [ ] Azure provider uploads to correct container/bucket.

## 7. Business Rules

### BR-001: No path traversal

BLOB names are normalized and cannot escape the base directory/container.

### BR-002: Tenant isolation

BLOB names can include tenant id prefix when `MultiTenancyConfig.IsEnabled`.

### BR-003: Backward compatibility

Ad-hoc file storage paths are not changed unless a migration script is provided.

## 8. Domain Modeling

### Bounded Context

File / Binary Storage

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `BlobContainer` | container name | Default `IBlobContainer` implementation |

### Value Objects

| Value Object | Fields | Validations |
|---|---|---|
| `BlobProviderArgs` | `BlobName`, `ContainerName`, `Bytes` | Name normalized |

### Domain Events

- `BlobSavedEvent`
- `BlobDeletedEvent`

## 9. Expected Architecture

```text
src/Eaf.BlobStoring/
  EafBlobStoringModule.cs
  BlobContainer.cs
  BlobContainerFactory.cs
  BlobStoringConfiguration.cs
  IBlobContainer.cs
  IBlobContainerFactory.cs
  IBlobProvider.cs
  Providers/
    FileSystemBlobProvider.cs
    AzureBlobProvider.cs
  Naming/
    IBlobNamingNormalizer.cs
    DefaultBlobNamingNormalizer.cs
test/Eaf.BlobStoring.Tests/
```

## 10. API Contracts

No new HTTP endpoints. Programmatic API:

```csharp
public class ProfileAppService : ApplicationService
{
    private readonly IBlobContainer<ProfilePictureContainer> _blobContainer;

    public ProfileAppService(IBlobContainer<ProfilePictureContainer> blobContainer)
    {
        _blobContainer = blobContainer;
    }

    public async Task SaveProfilePictureAsync(byte[] bytes)
    {
        var blobName = AbpSession.ToUserIdentifier().ToString();
        await _blobContainer.SaveAsync(blobName, bytes, overrideExisting: true);
    }
}
```

## 11. Application Contracts

```csharp
public interface IBlobContainer
{
    Task SaveAsync(string name, Stream stream, bool overrideExisting = false);
    Task SaveAsync(string name, byte[] bytes, bool overrideExisting = false);
    Task<Stream> GetAsync(string name);
    Task<byte[]> GetAllBytesAsync(string name);
    Task<byte[]> GetAllBytesOrNullAsync(string name);
    Task<bool> DeleteAsync(string name);
    Task<bool> ExistsAsync(string name);
}
```

## 12. Persistence and Data

### Persisted entities

N/A — BLOBs stored in filesystem or Azure Blob.

### Migration required

No.

### Compatibility

- [ ] No database migration.
- [ ] FileSystem base path configurable and must exist or be created.

## 13. Integrations

### External services

| Service | Data sent | Data received | Security |
|---|---|---|---|
| Azure Blob Storage | binary data | binary data | SAS / connection string via KeyVault |
| Local filesystem | binary data | binary data | OS permissions |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Duplicate BLOB | same name, `overrideExisting=false` | Throw `BlobAlreadyExistsException` |
| Missing BLOB | `GetAsync` on non-existing | Throw `UserFriendlyException` or return null based on method |
| Path traversal in name | `../../../etc/passwd` | Normalize to safe name or throw validation error |
| Azure container missing | invalid connection | Throw `UserFriendlyException` with clear message |

## 15. Few-Shot Examples

### Example 1: Save and read bytes

```csharp
var container = Resolve<IBlobContainer>();
await container.SaveAsync("report.pdf", bytes);
var read = await container.GetAllBytesAsync("report.pdf");
```

### Example 2: Typed container

```csharp
[BlobContainerName("profile-pictures")]
public class ProfilePictureContainer { }

var container = Resolve<IBlobContainer<ProfilePictureContainer>>();
await container.SaveAsync("user-1", pictureBytes);
```

## 16. Non-Functional Requirements

### Performance

- Streams are not fully loaded into memory unless requested.
- Async operations throughout.

### Security

- Normalize names to prevent path traversal.
- Do not log connection strings.

### Observability

- Log save/delete operations with BLOB name and container.
- OpenTelemetry spans for Azure calls.

### Maintainability

- Provider interface allows adding S3, MinIO later.
- README.md with provider setup.

## 17. Mandatory Guardrails

- Do not create path traversal vulnerabilities.
- Do not log secrets.
- Do not change existing upload endpoints without migration.

## 18. Expected Tests

### Unit tests

| Class | Scenarios |
|---|---|
| `DefaultBlobNamingNormalizer` | Path traversal prevention, normalization |
| `BlobContainer` | Save/Get/Delete/Exists with mocked provider |
| `BlobContainerFactory` | Named and typed container creation |

### Integration tests

| Flow | Validation |
|---|---|
| FileSystem provider | Round-trip with temp directory |
| Azure provider | Round-trip against Azurite or real Azure container |

## 19. Acceptance Criteria

- [ ] `Eaf.BlobStoring` compiles and packs.
- [ ] FileSystem and Azure providers pass integration tests.
- [ ] `IBlobContainer<T>` and `IBlobContainerFactory` work in a test host.
- [ ] README.md documents providers and configuration.
- [ ] Existing tests still pass.

## 20. Implementation Plan

1. Create `src/Eaf.BlobStoring/` and `test/Eaf.BlobStoring.Tests/`.
2. Define contracts and default container.
3. Implement FileSystem provider.
4. Implement Azure provider behind feature/package conditional.
5. Add naming normalizer and configuration.
6. Add tests.
7. Add README.md and update index.
8. Build and test.

## 21. Rollback Strategy

- Disable provider and revert to ad-hoc file paths.
- Delete unused Azure container or local directory.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| Path traversal | High | Low | Normalize names and validate base path |
| Azure SDK version conflicts | Medium | Low | Pin in `common.props` |
| Large file memory pressure | Medium | Medium | Stream async, avoid `byte[]` overload for large files |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Module implemented and tested.
- [ ] README.md and index updated.
- [ ] Build and tests pass.

## 24. Key Reminder

> Follow ABP `IBlobContainer` semantics. Security (path traversal, secrets) is the highest priority.
