using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.EntityHistory;
using Abp.Hangfire;
using Eaf.Middleware.Configuration;
using Abp.Timing;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using System;
using System.Linq;
using Eaf.Auditing.hangfire;

namespace Eaf.Middleware.Web.Auditing.hangfire
{
    /// <summary>
    /// Representa a classe ExpiredEntityLogDeleterWorker.
    /// </summary>
    public class ExpiredEntityLogDeleterWorker : DomainService, IExpiredEntityLogDeleterWorker
    {
        private int MaxDeletionCount = 30000;

        private readonly IRepository<EntityChange, long> _auditLogRepository;
        private readonly ISettingManager _settingManager;
        private TimeSpan _logExpireTime;
        private bool isEnabled;

        /// <summary>
        /// ExpiredEntityLogDeleterWorker.
        /// </summary>
        /// <param name="auditLogRepository">Parâmetro auditLogRepository.</param>
        /// <param name="historyConfiguration">Parâmetro historyConfiguration.</param>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <returns>Resultado da operação.</returns>
        public ExpiredEntityLogDeleterWorker(IRepository<EntityChange, long> auditLogRepository, IEntityHistoryConfiguration historyConfiguration, ISettingManager settingManager)
        {
            _auditLogRepository = auditLogRepository;
            _settingManager = settingManager;

            _logExpireTime = TimeSpan.FromDays(180);
            isEnabled = historyConfiguration.IsEnabled;
        }

        [UnitOfWork]
        [DisableConcurrentExecution(900)]
        [AutomaticRetry(Attempts = 0)]
        [JobDisplayName("Expired Entity Log Deleter")]
        public void DoWork(PerformContext context)
        {
            var expireDate = Clock.Now - _logExpireTime;
            DeleteAuditLogs(expireDate, context);
        }

        private void DeleteAuditLogs(DateTime expireDate, PerformContext context)
        {
            try
            {
                isEnabled = bool.Parse(_settingManager.GetSettingValue(EafMiddlewareSettingNames.LogDeleter.IsEnabled));
                MaxDeletionCount = Convert.ToInt32(_settingManager.GetSettingValue(EafMiddlewareSettingNames.LogDeleter.DeletedQuantity));
                _logExpireTime = TimeSpan.FromDays(Convert.ToInt32(_settingManager.GetSettingValue(EafMiddlewareSettingNames.LogDeleter.ExpiredDays)));
                expireDate = Clock.Now - _logExpireTime;
            }
            catch
            {
                context.WriteLine("No parameters configured. User Default Configuration");
            }

            context.WriteLine("Max Deletion Count : " + MaxDeletionCount.ToString());
            context.WriteLine("Enabled: " + isEnabled.ToString());
            context.WriteLine("Expired Time: " + _logExpireTime.Days.ToString());

            var expiredEntryCount = _auditLogRepository.LongCount(l => l.ChangeTime < expireDate);
            long count = 0;
            if (expiredEntryCount == 0)
            {
                context.WriteLine("No expired registration to be removed.");
                return;
            }

            context.WriteLine($"Total registration expired to be removed. -> {expiredEntryCount}");

            if (!isEnabled)
            {
                context.WriteLine("Disabled Job -> Configuration.EntityHistory.LogExpireEnabled");
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
                        var deleteItens = _auditLogRepository.GetAll().Where(l => l.ChangeTime < expireDate);
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