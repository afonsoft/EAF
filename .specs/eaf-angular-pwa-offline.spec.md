# EAF Angular — PWA, Cache e Experiência Offline

## Resumo
Transformar o template Angular EAF em uma **Progressive Web App (PWA)** com Service Worker (`@angular/service-worker`), cache de assets e APIs, suporte a instalação e notificações push, aproveitando os pacotes `@angular/pwa` e `@angular/service-worker` já presentes no `package.json`.

## Motivação
- `@angular/pwa` e `@angular/service-worker` já estão em `dependencies` do `package.json`, mas não há evidências de configuração ativa de PWA no template.
- Usuários mobile esperam instalar o app e acessar dados offline.
- ASP.NET Zero oferece mobile app .NET MAUI; uma PWA pode cobrir casos mais leves sem publicar nas stores.

## Estado Atual
- `package.json` possui `@angular/pwa` e `@angular/service-worker`.
- `ngsw-config.json` pode existir? Não verificado.
- Não há manifesto `manifest.webmanifest` visível nos assets principais.
- Service worker não é registrado em `main.ts` ou `app.module.ts` sem verificação.

## Proposta de Mudanças

### 1. Configurar Angular PWA
- Rodar `ng add @angular/pwa` (ou manualmente criar `ngsw-config.json`, `manifest.webmanifest`).
- Registrar `ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production })`.
- Adicionar `manifest.webmanifest` em `src/assets` com ícones EAF, cores, atalhos.

### 2. Cache de dados
- Definir `assetGroups` para cachear CSS, JS, fontes e imagens.
- Definir `dataGroups` para cachear chamadas API com estratégias `performance` (dashboard) e `freshness` (dados de edição).
- Integrar com `localforage` (já presente) para cache de dados de usuário, configurações e preferências.

### 3. Offline UX
- Detectar estado de conectividade (`navigator.onLine` + eventos `online`/`offline`).
- Mostrar banner/snackbar de offline e sincronização pendente.
- Armazenar ações do usuário (ex: mensagens de chat) em fila offline e sincronizar quando online.

### 4. Notificações Push
- Configurar `PushSubscription` no service worker.
- Backend EAF: endpoint para registrar subscrições e enviar push via VAPID.
- Usar notificações push para chat, alertas de sistema e aprovações.

### 5. Instalação e ícones
- Adicionar `beforeinstallprompt` para sugerir instalação.
- Criar ícones 192x192 e 512x512 e splash screens.
- Configurar `display: standalone`, `theme_color`, `background_color`.

### 6. Background Sync (opcional)
- Usar `BackgroundSync` para enviar mensagens de chat e formulários offline.

## Plano de Migração
1. Verificar configuração atual de PWA no template.
2. Criar `ngsw-config.json` e `manifest.webmanifest`.
3. Ajustar `app.module.ts` para registrar service worker.
4. Implementar cache de API e UI de offline.
5. Adicionar notificações push backend + frontend.
6. Testar em Lighthouse e dispositivos reais.

## Impacto
- **Médio**: altera build e assets, adiciona configuração.
- **Alto**: melhora significativa de UX mobile.
- **Médio**: backend precisa expor endpoints push.

## Riscos
- Cache agressivo pode causar dados desatualizados; requer estratégias por endpoint.
- Multi-tenancy com Service Worker exige cuidado com cache por tenant.
- Notificações push exigem HTTPS e VAPID keys.

## Referências
- <https://angular.io/guide/service-worker-intro>
- <https://angular.io/guide/service-worker-config>
- `/home/ubuntu/repos/EAF/Templates/Angular/Eaf.ProjectName.UI/package.json` — `@angular/pwa`, `@angular/service-worker`.
