# 11 — Aplicar ChangeDetectionStrategy.OnPush em Componentes Stateless

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 3 — Performance Angular |
| **Complexidade** | MÉDIA |
| **Risco** | MÉDIO — Pode quebrar componentes que dependem de Default detection |
| **Dependências** | Executar APÓS tarefa 09 (subscriptions precisam de cleanup antes) |
| **Arquivos Modificados** | ~48 arquivos TypeScript |

## Objetivo

Adicionar `changeDetection: ChangeDetectionStrategy.OnPush` a componentes que são stateless ou que só recebem dados via `@Input()`.

## Motivo

- **Performance**: `OnPush` pula re-render quando inputs não mudam — reduz ciclos de change detection
- **Apenas 2 componentes** usam `OnPush` atualmente — sub-utilizado
- **Angular 19**: Recomendação oficial é usar `OnPush` por padrão

## Critério de Seleção

Componentes elegíveis para `OnPush`:
1. **Stateless**: Não modifica estado interno (apenas exibe dados)
2. **Input-driven**: Dados vêm via `@Input()` ou `async` pipe
3. **Sem mutação direta**: Não modifica objetos/arrays in-place
4. **Sem setTimeout/setInterval** que não usam `ChangeDetectorRef`

Componentes NÃO elegíveis:
1. Componentes com `setTimeout`/`setInterval` sem `markForCheck()`
2. Componentes que modificam objetos por referência
3. Componentes raiz (`AppComponent`)
4. Componentes com lógica complexa de estado interno

## Padrão a Aplicar

```typescript
// ── ANTES ──
@Component({
    selector: 'app-some-widget',
    templateUrl: './some-widget.component.html',
})
export class SomeWidgetComponent {
    @Input() data: SomeData;
}

// ── DEPOIS ──
import { ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-some-widget',
    templateUrl: './some-widget.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SomeWidgetComponent {
    @Input() data: SomeData;
}
```

## Processo de Execução

1. **Listar** todos os componentes: `grep -rn "@Component" Templates/Angular/src/app --include="*.ts"`
2. **Filtrar** componentes sem `OnPush`: excluir os que já têm
3. **Classificar** cada um:
   - `ONPUSH`: Stateless/Input-driven → aplicar
   - `SKIP`: Tem mutação direta ou estado complexo → não aplicar
   - `REVIEW`: Ambíguo → documentar e pular
4. **Aplicar** em batches de 10
5. **Build** após cada batch

## Cenários de Teste

```bash
# 1. Build de produção:
npx ng build --configuration=production

# 2. Testes unitários:
npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox

# 3. Verificação manual (se possível):
# Navegar pelas telas e verificar que dados são exibidos corretamente
```

## Comandos de Verificação

```bash
cd Templates/Angular/Eaf.ProjectName.UI
npx ng build --configuration=production
npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox
```

## Critérios de Aceite

1. Pelo menos 30+ componentes com `OnPush` (de ~48 elegíveis)
2. Zero componentes quebrados (build + testes passam)
3. Componentes ambíguos documentados com `// REVIEW: OnPush candidate`
4. Build de produção passa

## Notas para o Sub-Agent

- **Conservador**: Na dúvida, NÃO aplicar `OnPush` — é melhor perder otimização que quebrar
- Se um componente usa `this.something = value` em resposta a eventos, verificar se precisa de `markForCheck()`
- Se build falhar após aplicar em um componente, reverter aquele componente e marcar como SKIP
- Trabalhar em batches de 10, fazendo build a cada batch
- **Se complexidade ultrapassar 3 horas**, reportar quantos foram feitos e sugerir continuar em nova tarefa
