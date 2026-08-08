# Eaf.Middleware.Ldap

## Descrição Técnica

O **Eaf.Middleware.Ldap** é um módulo de autenticação LDAP/Active Directory. Este módulo fornece integração completa com diretórios LDAP para autenticação externa, permitindo que usuários autentiquem usando suas credenciais existentes.

Este módulo suporta tanto Active Directory quanto outros servidores LDAP compatíveis, facilitando a integração com infraestruturas existentes.

## Relação com o EAF e ASP.NET Boilerplate

### Integração com ABP
- **Abp**: Framework base para injeção de dependência e configuração
- **Abp.Zero.Common**: Funcionalidades comuns do ABP Zero

### Dependências Externas
- **Novell.Directory.Ldap.NET**: Cliente LDAP para .NET
- **System.DirectoryServices**: Integração nativa com Active Directory

### Principais Componentes

#### LdapAuthenticationSource
Implementação de autenticação externa via LDAP:
- Conexão com servidor LDAP
- Validação de credenciais
- Sincronização de usuários
- Mapeamento de atributos LDAP

#### LdapSettings
Configurações de conexão LDAP:
- Endereço do servidor
- Porta (padrão: 389 para LDAP, 636 para LDAPS)
- Base DN
- Atributos de usuário
- Configurações de SSL/TLS

## Guia de Instalação

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0
- Servidor LDAP ou Active Directory configurado

### Instalação via NuGet
```bash
dotnet add package Eaf.Middleware.Ldap --version 9.4.4
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.Middleware.Ldap\Eaf.Middleware.Ldap.csproj" />
```

## Exemplo Básico de Uso

### 1. Registrando o Módulo

No seu módulo principal, herde de `MiddlewareLdapModule`:

```csharp
[DependsOn(
    typeof(MiddlewareLdapModule),
    typeof(AbpZeroCommonModule)
)]
public class MyAuthenticationModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

### 2. Configurando LDAP

No `appsettings.json`:

```json
{
  "Ldap": {
    "Server": "ldap.example.com",
    "Port": 389,
    "UseSsl": false,
    "Domain": "example.com",
    "BaseDn": "DC=example,DC=com",
    "UserDn": "CN=Users,DC=example,DC=com",
    "UsernameAttribute": "sAMAccountName",
    "EmailAttribute": "mail",
    "FirstNameAttribute": "givenName",
    "LastNameAttribute": "sn"
  }
}
```

### 3. Configurando para Active Directory

```json
{
  "Ldap": {
    "Server": "ad.example.com",
    "Port": 636,
    "UseSsl": true,
    "Domain": "example.com",
    "BaseDn": "DC=example,DC=com",
    "UserDn": "CN=Users,DC=example,DC=com",
    "UsernameAttribute": "sAMAccountName",
    "EmailAttribute": "mail",
    "FirstNameAttribute": "givenName",
    "LastNameAttribute": "sn"
  }
}
```

### 4. Usando Autenticação LDAP

```csharp
public class LdapAuthenticationAppService : ApplicationService
{
    private readonly LdapAuthenticationSource _ldapAuthSource;

    public LdapAuthenticationAppService(LdapAuthenticationSource ldapAuthSource)
    {
        _ldapAuthSource = ldapAuthSource;
    }

    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        try
        {
            var result = await _ldapAuthSource.AuthenticateAsync(username, password);
            return result != null;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "LDAP authentication failed");
            return false;
        }
    }
}
```

### 5. Sincronizando Usuários do LDAP

```csharp
public class LdapSyncService : ApplicationService
{
    private readonly LdapAuthenticationSource _ldapAuthSource;

    public LdapSyncService(LdapAuthenticationSource ldapAuthSource)
    {
        _ldapAuthSource = ldapAuthSource;
    }

    public async Task SyncUserAsync(string username)
    {
        var user = await _ldapAuthSource.CreateOrUpdateUserAsync(
            new ExternalAuthUserInfo
            {
                ProviderName = "LDAP",
                ProviderKey = username,
                Name = username
            }
        );
    }
}
```

## Estrutura do Módulo

```
Eaf.Middleware.Ldap/
├── Ldap/                  # Implementações LDAP
│   ├── LdapAuthenticationSource.cs
│   ├── LdapSettings.cs
│   └── LdapUserManager.cs
└── MiddlewareLdapModule.cs  # Módulo ABP
```

## Configurações Opcionais

### Configuração de Timeout
```json
{
  "Ldap": {
    "Server": "ldap.example.com",
    "Port": 389,
    "ConnectionTimeout": 30,
    "SearchTimeout": 60
  }
}
```

### Configuração de Atributos Personalizados
```json
{
  "Ldap": {
    "CustomAttributes": {
      "Department": "department",
      "Title": "title",
      "Phone": "telephoneNumber"
    }
  }
}
```

### Filtro de Usuários
```csharp
public override void PreInitialize()
{
    Configuration.Modules.EafLdap().UserFilter = "(objectClass=user)";
    Configuration.Modules.EafLdap().Enabled = true;
}
```

## Testes

Os testes para este módulo estão localizados em:
```
test/Eaf.Middleware.Ldap.Tests/
```

Para executar os testes:
```bash
dotnet test test/Eaf.Middleware.Ldap.Tests/Eaf.Middleware.Ldap.Tests.csproj
```

**Cobertura Atual:** 5.1% (necessita expansão para atingir meta de 90%)

## Licença

Este projeto faz parte do Enterprise Application Foundation (EAF) e está licenciado sob os mesmos termos do projeto principal.

## Suporte

Para issues e perguntas, consulte o repositório principal do EAF:
https://github.com/afonsoft/EAF