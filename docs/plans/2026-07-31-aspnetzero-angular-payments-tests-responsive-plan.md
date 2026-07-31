# Execution Plan: Payments UX, Angular Tests, Responsive, Dashboard

## 1. Goal and context

Aplicar melhorias no template Angular EAF para as funcionalidades de Payment, Dashboard, responsividade mobile e cobertura de testes unitários, continuando o fechamento do gap com ASP.NET Zero.

## 2. Impacted files and modules

- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/payments/payments.component.{ts,html}`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/main/dashboard/dashboard.component.{ts,html}`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/organization-units/organization-units.component.spec.ts` (novo)
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/payments/payments.component.spec.ts` (novo)
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/user-delegations/user-delegations.component.spec.ts` (novo)
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/mass-notifications/mass-notifications.component.spec.ts` (novo)
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/editions/editions.component.spec.ts` (já existe; revisar)
- `Templates/Angular/Eaf.ProjectName.UI/src/assets/common/styles/styles.css`
- `src/Eaf.Middleware.Core/Localization/Source/EafCore.xml` e `EafCore-pt-BR.xml`

## 3. Implementation strategy

1. **Payments UX**
   - Exibir nome da edição ao invés do `editionId` usando a lista já carregada.
   - Adicionar badge de status colorido (`Pending`, `Processing`, `Completed`, `Canceled`, `Failed`).
   - Adicionar chave de localização `Gateway`.
   - Garantir que os botões de ação tenham rótulos acessíveis.
2. **Dashboard**
   - Adicionar estado vazio (`empty-state`) quando não houver tiles.
   - Melhorar responsividade das cards (colunas `col-xl-3 col-lg-4 col-md-6` já existem; garantir que não quebrem em mobile).
3. **Responsivo mobile**
   - Adicionar regras CSS para garantir touch targets >= 44px nas novas telas admin.
   - Garantir que modais não extrapolem a viewport em telas pequenas.
4. **Testes unitários Angular**
   - Criar specs para `payments`, `organization-units`, `user-delegations`, `mass-notifications` usando os mocks em `src/test-helpers/mock-services.ts`.
   - Testar inicialização, abertura de modais e chamadas aos service proxies.
5. **Plano de ajustes reutilizável**
   - Gerar `docs/plans/2026-07-31-eaf-angular-template-adjustments-reuse-plan.md` com passos genéricos para aplicar as mesmas mudanças em outro repositório EAF/ABP.

## 4. Risks and mitigations

- **Risco**: `ng test` não roda localmente sem Chrome.  
  **Mitigação**: os specs serão validados via `npx tsc` e `npx ng build`.
- **Risco**: Chaves de localização duplicadas.  
  **Mitigação**: verificar `EafCore.xml` e `EafCore-pt-BR.xml` antes de inserir.
- **Risco**: Modais do ngx-bootstrap causarem alertas Sonar.  
  **Mitigação**: manter `role="dialog"`/`role="document"` removidos.

## 5. Validation steps

- `npx tsc -p src/tsconfig.app.json --noEmit`
- `npx ng build --configuration=production`
- `dotnet build Eaf.sln --configuration Release`
- `dotnet test Eaf.sln --configuration Release --no-build`
