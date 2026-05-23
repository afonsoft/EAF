using Eaf.Middleware.Worker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.ProjectName.WorkerService
{
    public class Worker : EafWorkerBase
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Logger.InfoFormat("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}