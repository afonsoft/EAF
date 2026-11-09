# Eaf.Quartz

Módulo EAF que registra um scheduler [Quartz.NET](https://www.quartz-scheduler.net/) como implementação alternativa de `IBackgroundJobManager` do ABP — alternativa opt-in ao Hangfire.

## Uso

```csharp
[DependsOn(typeof(EafQuartzModule))]
public class MeuModulo : AbpModule { }
```

Configuração:

```csharp
var options = IocManager.Resolve<EafQuartzOptions>(); // ou injete IOptions<EafQuartzOptions>
options.SchedulerName = "EafQuartz";
options.JobStoreType = EafQuartzJobStoreType.Ram; // Ram | Ado | Mongo
options.ConnectionString = "...";               // Ado/Mongo
options.InstanceId = "AUTO";                    // clustering
```

- `EnqueueAsync<TJob,TArgs>` → job one-shot (delay opcional)
- `ScheduleAsync<TJob,TArgs>` → job recorrente via expressão cron
- `DeleteAsync` → remove o job agendado

Os argumentos são serializados em JSON no `JobDataMap`. Não serialize secrets/PII.
