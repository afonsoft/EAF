# Eaf.OpenIddict

Servidor OAuth2/OpenID Connect para EAF basado en [OpenIddict](https://documentation.openiddict.com/).

## Características

- Endpoints estándar: `/connect/token`, `/connect/authorize`, `/connect/userinfo`, `/connect/logout`.
- Grant types: `password`, `client_credentials`, `authorization_code` (con PKCE) y `refresh_token`.
- Access tokens JWT firmados con la misma clave simétrica de `Authentication:JwtBearer`, por lo que los
  endpoints existentes con `[AbpAuthorize]` los aceptan configurando `AdditionalIssuers`.
- Stores EF Core en un `DbContext` dedicado (`EafOpenIddictDbContext`).
- Registro de aplicaciones (clients) desde `appsettings.json`, sembradas al arrancar.

## Configuración

```json
"OpenIddict": {
  "Issuer": "https://localhost:44301/",
  "Applications": [
    {
      "ClientId": "client",
      "ClientSecret": "...",
      "DisplayName": "Console API Client",
      "ConsentType": "implicit",
      "Permissions": [
        "ept:token",
        "gt:password",
        "gt:client_credentials",
        "scp:default-api"
      ]
    }
  ]
}
```

Y en `Authentication:JwtBearer` agregar el issuer del servidor:

```json
"Authentication": {
  "JwtBearer": {
    "AdditionalIssuers": [ "https://localhost:44301/" ]
  }
}
```

## Uso en el host

```csharp
services.AddEafOpenIddict(_appConfiguration);
```

y en el módulo del host:

```csharp
[DependsOn(typeof(EafOpenIddictModule))]
```

## Migraciones

El esquema de OpenIddict se genera sobre `EafOpenIddictDbContext`:

```bash
dotnet ef migrations add AddOpenIddict -p src/Eaf.OpenIddict -c EafOpenIddictDbContext
dotnet ef database update -c EafOpenIddictDbContext -p src/Eaf.OpenIddict
```

La cadena de conexión se toma de `ConnectionStrings__Default`.
