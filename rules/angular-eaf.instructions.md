---
name: 'Angular EAF UI Standards'
description: 'Padrões de desenvolvimento Angular para o template UI do EAF, incluindo componentes, serviços e integração com backend ABP'
applyTo: '**/*.ts,**/*.html,**/*.scss'
---

# Angular EAF UI Standards

## Framework

- Angular 18 com TypeScript 5.2
- PrimeNG 17 para componentes UI
- Metronic CSS/JS theme engine
- RxJS 7 para programação reativa

## Componentes

- Preferir standalone components
- Usar signals para estado reativo
- Reactive forms para formulários
- Lazy loading para rotas

## Serviços

- Service proxies gerados pelo NSwag (`service-proxies.ts`) — não editar
- Usar `HttpClient` com interceptors para autenticação
- `AbpHttpInterceptor` para tratamento de erros

## Padrões

- Seguir padrão Angular Style Guide
- Um componente por arquivo
- Imports no topo do arquivo
- Documentação TSDoc em serviços públicos

## Testes

- Karma + Jasmine
- Node 18 via `nvm use 18`
- `npm install --legacy-peer-deps` obrigatório
- CHROME_BIN configurado para ChromeHeadless

## Segurança

- Sanitizar inputs do usuário
- Usar `[innerText]` ao invés de `[innerHTML]`
- CSP headers configurados no backend
