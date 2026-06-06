# 10 — Corrigir Preload e Adicionar Bundle Budgets

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 3 — Performance Angular |
| **Complexidade** | BAIXA |
| **Risco** | BAIXO — Mudanças de configuração |
| **Dependências** | Nenhuma |
| **Arquivos Modificados** | 2 arquivos |

## Objetivo

1. Remover `preload: true` da rota `admin` (anula lazy loading)
2. Adicionar bundle budgets no `angular.json` para controlar tamanho

## Motivo

- `data: { preload: true }` em TODAS as rotas lazy (main + admin) carrega todos os módulos no startup — derrota o propósito de lazy loading
- Zero bundle budgets = sem controle de tamanho — builds podem crescer sem limites

## Arquivos Afetados

### 1. Routing

**`Templates/Angular/src/app/app-routing.module.ts`**

```typescript
// ── ANTES (linhas 26-35) ──
{
    path: '',
    loadChildren: () => import('app/main/main.module').then(m => m.MainModule),
    data: { preload: true }, // OK — rota principal, faz sentido precarregar
},
{
    path: 'admin',
    loadChildren: () => import('app/admin/admin.module').then(m => m.AdminModule),
    data: { preload: true }, // RUIM — admin é usado por poucos, não precisa precarregar
}

// ── DEPOIS ──
{
    path: '',
    loadChildren: () => import('app/main/main.module').then(m => m.MainModule),
    data: { preload: true }, // Main route — preloaded for fast initial navigation
},
{
    path: 'admin',
    loadChildren: () => import('app/admin/admin.module').then(m => m.AdminModule),
    // Removed preload: admin module loaded on-demand when admin area is accessed
}
```

### 2. Bundle Budgets

**`Templates/Angular/angular.json`**

```json
// ── ADICIONAR em architect.build.configurations.production ──
"budgets": [
    {
        "type": "initial",
        "maximumWarning": "2mb",
        "maximumError": "5mb"
    },
    {
        "type": "anyComponentStyle",
        "maximumWarning": "6kb",
        "maximumError": "10kb"
    }
]
```

### Teste

## Cenários de Teste

```bash
# 1. Verificar que build de produção passa com budgets:
npx ng build --configuration=production
# Se ultrapassar budgets, ajustar os valores ou otimizar

# 2. Verificar que lazy loading funciona:
# - Abrir DevTools → Network
# - Carregar app → admin module NÃO deve aparecer nos requests iniciais
# - Navegar para /admin → admin module deve ser carregado sob demanda
```

## Comandos de Verificação

```bash
cd Templates/Angular/Eaf.ProjectName.UI
npx ng build --configuration=production
```

## Critérios de Aceite

1. Apenas rota main tem `preload: true`
2. Rota admin é lazy-loaded sob demanda
3. Bundle budgets configurados no angular.json
4. Build de produção passa sem erros
5. Se budget exceder, ajustar valores realistas (não remover budgets)

## Notas para o Sub-Agent

- Manter `preload: true` na rota main — é o conteúdo principal
- Se houver outras rotas com `preload: true`, avaliar caso a caso
- Valores de budget (2mb/5mb) são sugestões — ajustar conforme tamanho real do bundle
- Se build de produção já ultrapassar 5mb, aumentar o error budget e criar issue separada
