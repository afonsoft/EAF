# 09 — Corrigir 117 RxJS Subscriptions sem Cleanup (Memory Leaks)

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 3 — Performance Angular |
| **Complexidade** | ALTA (volume: 117 ocorrências em ~50 componentes) |
| **Risco** | BAIXO — Cada mudança é mecânica, mas volume alto |
| **Dependências** | Nenhuma |
| **Arquivos Modificados** | ~50 arquivos TypeScript |

## Objetivo

Adicionar cleanup pattern (`takeUntilDestroyed`) a todas as 117 instâncias de `.subscribe()` sem unsubscribe nos componentes Angular, eliminando memory leaks.

## Motivo

- **Memory leaks**: Subscriptions sem cleanup continuam ativas após destruição do componente
- **117 ocorrências**: Todos os componentes que usam `.subscribe()` sem `takeUntil`, `takeUntilDestroyed`, ou `Subscription.unsubscribe()`
- **Angular 19**: `takeUntilDestroyed()` é a API recomendada (estável desde Angular 16)

## Padrão a Aplicar

```typescript
// ── ANTES (padrão problemático) ──
import { Component, OnInit } from '@angular/core';

@Component({ ... })
export class SomeComponent implements OnInit {
    ngOnInit() {
        this.someService.getData().subscribe(data => {
            this.data = data;
        });
    }
}

// ── DEPOIS (com takeUntilDestroyed) ──
import { Component, OnInit, inject, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({ ... })
export class SomeComponent implements OnInit {
    private destroyRef = inject(DestroyRef);

    ngOnInit() {
        this.someService.getData()
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(data => {
                this.data = data;
            });
    }
}
```

**ALTERNATIVA** (para componentes que não usam `inject`):
```typescript
@Component({ ... })
export class SomeComponent implements OnInit, OnDestroy {
    private destroy$ = new Subject<void>();

    ngOnInit() {
        this.someService.getData()
            .pipe(takeUntil(this.destroy$))
            .subscribe(data => { this.data = data; });
    }

    ngOnDestroy() {
        this.destroy$.next();
        this.destroy$.complete();
    }
}
```

## Exceções — NÃO aplicar em:

1. **HTTP requests simples** (`this.http.get().subscribe()`) — completam automaticamente
2. **Router events** que já usam `takeUntil`
3. **Subscriptions no construtor** com `takeUntilDestroyed()` já aplicado
4. **AppComponent** (raiz, nunca destruído)

## Localização dos Arquivos

```bash
# Comando para encontrar todas as ocorrências:
cd Templates/Angular
grep -rn "\.subscribe(" src/app --include="*.ts" | grep -v "\.spec\." | grep -v "node_modules"
```

## Processo de Execução

1. **Listar** todas as ocorrências com `grep`
2. **Classificar** cada uma:
   - `FIX`: precisa de cleanup
   - `SKIP`: HTTP one-shot ou já tem cleanup
3. **Aplicar** `takeUntilDestroyed` para todas classificadas como `FIX`
4. **Verificar** build após cada 10 arquivos modificados

## Cenários de Teste

```typescript
// Templates/Angular não tem testes unitários extensivos, mas verificar:

// 1. Build deve passar sem erros:
// npx ng build --configuration=production

// 2. Para componentes com testes existentes, verificar que passam:
// npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox

// 3. Verificar que não há erros de TypeScript:
// npx tsc --noEmit
```

## Comandos de Verificação

```bash
cd Templates/Angular/Eaf.ProjectName.UI
npm install --legacy-peer-deps
npx ng build --configuration=production
npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox
```

## Critérios de Aceite

1. Todas as subscriptions de longa duração têm cleanup pattern
2. `takeUntilDestroyed` usado preferencialmente (Angular 19 API)
3. Build de produção compila sem erros
4. Testes existentes passam
5. Zero `subscribe()` sem cleanup em componentes (exceto exceções listadas)

## Notas para o Sub-Agent

- **Volume alto**: São ~50 arquivos. Trabalhar em batches de 10, fazendo build a cada batch
- Se o build falhar após um batch, reverter e aplicar arquivo por arquivo para isolar o problema
- Preferir `takeUntilDestroyed(this.destroyRef)` pois é mais limpo que `Subject + takeUntil`
- `DestroyRef` deve ser injetado via `inject()` (não via construtor DI) quando possível
- Se um componente já usa `OnDestroy` com `Subject`, manter o padrão existente
- **Se complexidade ultrapassar 2 horas**, reportar e sugerir dividir em sub-tarefas
- Não modificar `service-proxies.ts` — é arquivo gerado
