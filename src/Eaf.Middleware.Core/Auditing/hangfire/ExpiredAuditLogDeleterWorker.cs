using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Timing;
using Eaf.Auditing.hangfire;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using System;
using System.Linq;

namespace Eaf.Middleware.Web.Auditing.hangfire
{
    /// <summary>
    /// Representa a classe ExpiredAuditLogDeleterWorker.
    /// </summary>
    public class ExpiredAuditLogDeleterWorker : DomainService, IExpiredAuditLogDeleterWorker
    {
        private readonly int MaxDeletionCount = 30000;

        private readonly IRepository<AuditLog, long> _auditLogRepository;
        private readonly TimeSpan _logExpireTime;
        private readonly bool isEnabled;

        /// <summary>
        /// ExpiredAuditLogDeleterWorker.
        /// </summary>
        /// <param name="auditLogRepository">Parâmetro auditLogRepository.</param>
        /// <param name="historyConfiguration">Parâmetro historyConfiguration.</param>
        /// <returns>Resultado da operação.</returns>
        public ExpiredAuditLogDeleterWorker(IRepository<AuditLog, long> auditLogRepository, IAuditingConfiguration historyConfiguration)
        {
            _auditLogRepository = auditLogRepository;
            _logExpireTime = TimeSpan.FromDays(180);
            isEnabled = historyConfiguration.IsEnabled;
        }

        [UnitOfWork]
        [DisableConcurrentExecution(900)]
        [AutomaticRetry(Attempts = 0)]
        [JobDisplayName("Expired Audit Log Deleter")]
        public void DoWork(PerformContext context)
        {
            var expireDate = Clock.Now - _logExpireTime;
            DeleteAuditLogs(expireDate, context);
        }

        private void DeleteAuditLogs(DateTime expireDate, PerformContext context)
        {
            var expiredEntryCount = _auditLogRepository.LongCount(l => l.ExecutionTime < expireDate);
            long count = 0;
            if (expiredEntryCount == 0)
            {
                context.WriteLine("No expired registration to be removed.");
                return;
            }

            context.WriteLine($"Total registration expired to be removed. -> {expiredEntryCount}");

            if (!isEnabled)
            {
                context.WriteLine("Disabled Job -> Configuration.Auditing.LogExpireEnabled");
                return;
            }

            using (CurrentUnitOfWork.SetTenantId(null))
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    if (expiredEntryCount > MaxDeletionCount)
                    {
                        context.WriteLine($"Limit of {MaxDeletionCount} record per process.");

                        var deleteStartId = _auditLogRepository.GetAll().OrderBy(l => l.Id).Skip(MaxDeletionCount).Select(x => x.Id).First();
                        var deleteItens = _auditLogRepository.GetAll().Where(l => l.Id < deleteStartId);
                        foreach (var del in deleteItens)
                        {
                            count++;
                            try
                            {
                                _auditLogRepository.Delete(del);
                                UnitOfWorkManager.Current.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                context.WriteLine($"Error on delete {del.Id}");
                                Logger.ErrorFormat(ex, "DeleteAuditLogs : {0}", ex.Message);
                            }
                        }
                    }
                    else
                    {
                        var deleteItens = _auditLogRepository.GetAll().Where(l => l.ExecutionTime < expireDate);
                        foreach (var del in deleteItens)
                        {
                            count++;
                            try
                            {
                                _auditLogRepository.Delete(del);
                                UnitOfWorkManager.Current.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                context.WriteLine($"Error on delete {del.Id}");
                                Logger.ErrorFormat(ex, "DeleteAuditLogs : {0}", ex.Message);
                            }
                        }
                    }
                }
            }
            context.WriteLine($"{count} Record deleted.");
            context.WriteLine("Job successfully completed.");
        }
    }
}