# Execution Plan: Continuação das implementações nos Templates Angular

## 1. Goal and context

Prosseguir com a implementação das telas do template Angular EAF para as funcionalidades do gap do ASP.NET Zero que já possuem backend (`Eaf.Middleware.*`). Nesta rodada o foco é em:

- Gerenciamento de **membros e roles das Organization Units** (já há endpoints backend).
- Complemento dos fluxos de **User Delegation** e **Payments** (campos, listas, modais).
- Adição de **testes unitários Angular** para os novos componentes admin.
- Ajustes de **mobile/responsivo** nas novas telas.

## 2. Impacted files and modules

- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/organization-units/organization-units.component.{ts,html}`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/user-delegations/user-delegations.component.{ts,html}`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/payments/payments.component.{ts,html}`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/mass-notifications/mass-notifications.component.spec.ts`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/organization-units/organization-units.component.spec.ts`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/payments/payments.component.spec.ts`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/user-delegations/user-delegations.component.spec.ts`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/editions/editions.component.spec.ts`
- `Templates/Angular/Eaf.ProjectName.UI/src/assets/common/styles/styles.css`
- `src/Eaf.Middleware.Core/Localization/Source/EafCore.xml` e `EafCore-pt-BR.xml` (se novas chaves forem necessárias)

## 3. Implementation strategy

1. **Organization Units — membros e roles**
   - Adicionar modais de membros e roles no `organization-units.component.html`.
   - Usar `UserServiceProxy.getUsers` e `RoleServiceProxy.getRoles` para popular dropdowns.
   - Usar `OrganizationUnitServiceProxy.getOrganizationUnitUsers`, `addUserToOrganizationUnit`, `removeUserFromOrganizationUnit`, `getOrganizationUnitRoles`, `addRoleToOrganizationUnit`, `removeRoleFromOrganizationUnit`.
   - Implementar `showMembersModal`, `showRolesModal`, `loadMembers`, `loadRoles`, `addMember`, `removeMember`, `addRole`, `removeRole`.
2. **User Delegation / Payments**
   - Melhorar UX dos modais (seleção de usuário por nome, exibição de status, filtros básicos).
3. **Responsivo**
   - Garantir touch targets >= 44px e quebra de colunas em mobile.
4. **Testes Angular**
   - Criar specs para os quatro novos componentes admin com mocks dos service proxies.
5. **Localização**
   - Reutilizar chaves existentes (`ManageMembers`, `ManageRoles`, `UserName`, `RoleName`, `Actions`, `Select`) e adicionar apenas chaves realmente novas.

## 4. Risks and mitigations

- **Risco**: Modais do `ngx-bootstrap` podem conflitar com a regra Sonar S6819.  
  **Mitigação**: manter elementos `<div>` sem `role="dialog"`/`role="document"` (já removidos) e evitar reintroduzi-los.
- **Risco**: `service-proxies.ts` é gerado; importar tipos manualmente pode quebrar se o arquivo for regenerado.  
  **Mitigação**: usar apenas exports já presentes e tipos simples (`any` quando necessário) para evitar dependência de classes internas.
- **Risco**: Duplicar chaves de localização gera exceção no ABP.  
  **Mitigação**: verificar `EafCore.xml` antes de inserir novas chaves.

## 5. Validation steps

- `npx tsc -p src/tsconfig.app.json --noEmit`
- `npx ng build --configuration=production`
- `dotnet test Eaf.sln --configuration Release --no-build` (já executado no passo anterior; reexecutar após alterações .NET, se houver)
- `npm run lint` ou `npx ng lint` se disponível
