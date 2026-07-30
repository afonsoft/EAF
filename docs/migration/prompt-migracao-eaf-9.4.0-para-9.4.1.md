# Prompt para agente: migrar projeto EAF 9.4.0 → 9.4.1

## Contexto

Você é um agente de migração EAF. Sua tarefa é atualizar um projeto gerado a partir dos templates **EAF Angular + API .NET** da versão **9.4.0** para **9.4.1**.

## Objetivo

Aplicar os ajustes de template (Angular e API) que foram feitos no repositório `afonsoft/EAF` entre a tag `9.4.0` e a versão `9.4.1` (commit `HEAD` da branch `main` no momento do release).

## Pré-requisitos

- Acesso ao repositório `afonsoft/EAF` (clone local ou remoto).
- O projeto de destino está em uma branch de migração.
- `python3`, `git`, `dotnet` e `npx` estão disponíveis no ambiente.

## Opção 1: executar o script automatizado (recomendado)

1. Localize o script de migração no repositório EAF:
   ```bash
   # Se não tiver o EAF clonado, faça o clone
   git clone https://github.com/afonsoft/EAF.git /tmp/eaf-repo

   # Caminho para o script
   EAF_SCRIPT=/tmp/eaf-repo/scripts/eaf-migrate-9.4.0-to-9.4.1-temp.py
   ```

2. Execute o script apontando para os diretórios do projeto gerado:
   ```bash
   python3 "$EAF_SCRIPT" \
     --eaf-repo /tmp/eaf-repo \
     --angular-dir /caminho/para/MinhaApp.UI \
     --api-dir /caminho/para/MinhaApp.Api
   ```

3. Para simular sem alterar nada:
   ```bash
   python3 "$EAF_SCRIPT" \
     --eaf-repo /tmp/eaf-repo \
     --angular-dir /caminho/para/MinhaApp.UI \
     --api-dir /caminho/para/MinhaApp.Api \
     --dry-run
   ```

4. O script cria backups em `.eaf-migrate-backup-<timestamp>` dentro de cada diretório.

## Opção 2: ajustes manuais

Se o script falhar ou o projeto tiver se afastado muito do template 9.4.0, aplique os ajustes abaixo manualmente.

### Ajustes no backend/API (.NET)

Atualize as referências dos pacotes EAF de `9.4.0` para `9.4.1` nos `.csproj` que as declarem, e a versão central no `common.props`.

**`common.props` (se existir na raiz da solução):**

```xml
<PropertyGroup>
  <Version>9.4.1</Version>
</PropertyGroup>
```

**`Eaf.ProjectName.Application.csproj`:**

```xml
<!--<PackageReference Include="Eaf.Middleware.Application" Version="9.4.1" />-->
```

**`Eaf.ProjectName.Core.csproj`:**

```xml
<!--<PackageReference Include="Eaf.Middleware.Core" Version="9.4.1" />-->
```

**`Eaf.ProjectName.Web.Host.csproj`:**

```xml
<!--<PackageReference Include="Eaf.Castle.Serilog" Version="9.4.1" />
<PackageReference Include="Eaf.Middleware.Web.Core" Version="9.4.1" />
<PackageReference Include="Eaf.OpenTelemetry" Version="9.4.1" />-->
```

> O script de migração faz a substituição automaticamente em qualquer arquivo `.csproj` e em `common.props`.

### Ajustes no frontend/Angular

#### 1. Modais: remover BOM e `aria-hidden="true"`

Nos arquivos de template abaixo, remova o **BOM (byte order mark)** do início do arquivo e a linha `  aria-hidden="true"` dentro do `<div ... bsModal ...>`.

Lista de arquivos:

- `src/app/admin/audit-logs/audit-log-detail-modal.component.html`
- `src/app/admin/languages/create-or-edit-language-modal.component.html`
- `src/app/admin/languages/edit-text-modal.component.html`
- `src/app/admin/roles/create-or-edit-role-modal.component.html`
- `src/app/admin/tenants/create-tenant-modal.component.html`
- `src/app/admin/tenants/edit-tenant-modal.component.html`
- `src/app/admin/tenants/tenant-features-modal.component.html`
- `src/app/admin/users/create-or-edit-user-modal.component.html`
- `src/app/admin/users/edit-user-permissions-modal.component.html`
- `src/app/shared/common/entityHistory/entity-change-detail-modal.component.html`
- `src/app/shared/common/entityHistory/entity-type-history-modal.component.html`
- `src/app/shared/common/lookup/common-lookup-modal.component.html`
- `src/app/shared/layout/login-attempts-modal.component.html`
- `src/app/shared/layout/notifications/notification-settings-modal.component.html`
- `src/app/shared/layout/profile/change-password-modal.component.html`
- `src/app/shared/layout/profile/change-profile-picture-modal.component.html`
- `src/app/shared/layout/profile/my-settings-modal.component.html`

Exemplo de transformação (em cada um):

```html
<!-- ANTES -->
﻿<div
  bsModal
  #createOrEditModal="bs-modal"
  (onShown)="onShown()"
  class="modal fade"
  tabindex="-1"

  aria-labelledby="createOrEditModal"
  aria-hidden="true"
  [config]="{ backdrop: 'static' }"
>

<!-- DEPOIS -->
<div
  bsModal
  #createOrEditModal="bs-modal"
  (onShown)="onShown()"
  class="modal fade"
  tabindex="-1"

  aria-labelledby="createOrEditModal"
  [config]="{ backdrop: 'static' }"
>
```

#### 2. `src/app/shared/layout/chat/chat-bar.component.css`

Substitua os trechos abaixo e acrescente o bloco de CSS no final do arquivo.

**Tabela de substituições:**

| Antes | Depois |
|-------|--------|
| `background: rgba(98, 93, 187, 0.08);` | `background: rgba(255, 112, 32, 0.08);` |
| `border-bottom: 1px solid rgba(98, 93, 187, 0.16);` | `border-bottom: 1px solid rgba(255, 112, 32, 0.16);` |
| `color: #3f3a75;` | `color: var(--primary, #FF7020);` |
| `background-color: #625dbb;` | `background-color: var(--primary, #FF7020);` |
| `background-color: #6bc0f9;` | `background-color: rgba(255, 112, 32, 0.08);` |
| `color: #000;` (no bloco de mensagem recebida) | `color: var(--primary, #FF7020);` |
| `.card {` | `#chatSideRight .card {` |

Acrescente no final do arquivo o bloco de tema:

```css
/* Theme-aligned colors for the EAF Angular template ------------------------------ */
#chatSideRight .bs-canvas-header {
    background-color: var(--primary, #FF7020) !important;
    color: #fff !important;
}

#chatSideRight .bs-canvas-header .close,
#chatSideRight .bs-canvas-header .pinned,
#chatSideRight .bs-canvas-header h4 {
    color: #fff !important;
}

#chatSideRight .bg-light-primary {
    background-color: rgba(255, 112, 32, 0.1) !important;
    color: #212529 !important;
}

#chatSideRight .bg-light-success {
    background-color: rgba(52, 191, 163, 0.1) !important;
    color: #212529 !important;
}

#chatSideRight .messages > .d-flex {
    background-color: transparent !important;
}

#chatSideRight .text-primary,
#chatSideRight .chat-message-sender {
    color: var(--primary, #FF7020) !important;
}

#chatSideRight .label-dot {
    display: inline-block;
    width: 8px;
    height: 8px;
    border-radius: 50%;
}

#chatSideRight .label-success {
    background-color: #34bfa3 !important;
}

#chatSideRight .label-secondary {
    background-color: #9c9c9c !important;
}

#chatSideRight #EmptyFriendListInfo,
#chatSideRight #EmptyBlockedFriendListInfo {
    text-align: center;
    color: #6c757d;
    padding: 1rem 0;
}

#chatSideRight .chat-attachment-actions .btn {
    color: var(--primary, #FF7020);
    border-color: var(--primary, #FF7020);
    background-color: #fff;
}

#chatSideRight .chat-attachment-actions .btn:hover {
    background-color: var(--primary, #FF7020);
    color: #fff;
}
```

#### 3. `src/app/shared/layout/chat/chat-bar.component.html`

Aplique as três alterações a seguir:

1. No botão de pin:

```html
<!-- ANTES -->
<i [ngClass]="{ 'fa-rotate-90': !pinned }" aria-hidden="true" class="fa fa-map-pin"> </i>

<!-- DEPOIS -->
<i [ngClass]="{ 'fa-rotate-90': !pinned }" aria-hidden="true" class="fa fa-map-pin text-light"> </i>
```

2. No nome do usuário logado (`currentUser.name`):

```html
<!-- ANTES -->
<span
  class="text-dark-75 text-hover-primary font-weight-bold font-size-h6"
  data-placement="top"
  data-toggle="tooltip"
  style="color: #ff7020"
  title="{{ getTitle(currentUser) }}"
>
  {{ currentUser.name }}
</span>

<!-- DEPOIS -->
<span
  class="text-dark-75 text-hover-primary font-weight-bold font-size-h6 text-primary"
  data-placement="top"
  data-toggle="tooltip"
  title="{{ getTitle(currentUser) }}"
>
  {{ currentUser.name }}
</span>
```

3. No nome do amigo (`getFriendName`):

```html
<!-- ANTES -->
<span
  class="text-dark-75 text-hover-primary font-weight-bold font-size-h6"
  data-placement="top"
  data-toggle="tooltip"
  style="color: #ff7020"
  title="{{ getTitle(selectedUser, message) }}"
>
  {{ getFriendName(selectedUser, message) }}
</span>

<!-- DEPOIS -->
<span
  class="text-dark-75 text-hover-primary font-weight-bold font-size-h6 text-primary"
  data-placement="top"
  data-toggle="tooltip"
  title="{{ getTitle(selectedUser, message) }}"
>
  {{ getFriendName(selectedUser, message) }}
</span>
```

#### 4. `src/assets/common/styles/styles.css`

No seletor `.m-switch input:empty ~ span`, altere `width: calc(100% - 75px);` para `width: auto;` e acrescente o bloco abaixo logo em seguida:

```css
.m-switch input:empty ~ span {
    /* ... */
    width: auto;
}

.m-switch input:empty ~ span.m-switch-label {
    position: static;
    float: none;
    display: inline-block;
    width: auto;
    height: auto;
    line-height: 34px;
    text-indent: 0;
    cursor: default;
    margin: 0 0 0 12px;
    vertical-align: top;
}

.m-switch input:empty ~ span.m-switch-label:before,
.m-switch input:empty ~ span.m-switch-label:after {
    content: none !important;
    display: none !important;
}
```

#### 5. `src/web.config`

Dentro da regra de rewrite do SPA, adicione a condição para não reescrever assets estáticos:

```xml
<rule name="Angular" stopProcessing="true">
  <match url=".*" />
  <conditions>
    <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true"/>
    <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true"/>
    <add input="{REQUEST_URI}" pattern="^/(api)" negate="true"/>
    <add input="{REQUEST_URI}" pattern="^.*\.(woff2?|eot|ttf|svg|png|jpg|jpeg|webp|gif|ico|js|css|json|map|ani|cur)(\?.*)?$" negate="true"/>
  </conditions>
  <action type="Rewrite" url="/"/>
</rule>
```

## Verificação

Após aplicar os ajustes:

1. Verifique se restou alguma referência a `9.4.0` nos arquivos de versão:
   ```bash
   grep -R '9\.4\.0' --include='*.csproj' --include='common.props' .
   ```

2. Compile a API:
   ```bash
   dotnet build MinhaApp.sln
   ```

3. Compile o Angular:
   ```bash
   cd src
   npx ng build --configuration=production
   ```

4. Compare com os templates 9.4.1 do EAF se necessário:
   ```bash
   git -C /tmp/eaf-repo diff 9.4.0..HEAD -- Templates/Angular/Eaf.ProjectName.UI/src Templates/Api/src
   ```

## Finalização

- Commite as mudanças em uma branch `migration/eaf-9.4.1`.
- Crie um PR no repositório de destino.
- Destrua os backups `.eaf-migrate-backup-*` após validação, se desejar.
