using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Timing;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.MassNotifications.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.MassNotifications
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de notificações em massa.
    /// </summary>
    [AbpAuthorize(MiddlewarePermissions.Pages_Administration_MassNotifications)]
    public class MassNotificationAppService : MiddlewareAppServiceBase, IMassNotificationAppService
    {
        private readonly IRepository<MassNotification, long> _massNotificationRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;

        /// <summary>
        /// MassNotificationAppService.
        /// </summary>
        public MassNotificationAppService(
            IRepository<MassNotification, long> massNotificationRepository,
            IBackgroundJobManager backgroundJobManager)
        {
            _massNotificationRepository = massNotificationRepository;
            _backgroundJobManager = backgroundJobManager;
        }

        /// <summary>
        /// Obtém as notificações em massa paginadas.
        /// </summary>
        public virtual async Task<PagedResultDto<MassNotificationDto>> GetAllAsync(GetMassNotificationsInput input)
        {
            var query = (await _massNotificationRepository.GetAllAsync())
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(), n =>
                    n.Subject.Contains(input.Filter) ||
                    n.Message.Contains(input.Filter))
                .WhereIf(!input.Status.IsNullOrWhiteSpace(), n => n.Status.ToString() == input.Status);

            var total = await query.CountAsync();
            var ordered = System.Linq.Dynamic.Core.DynamicQueryableExtensions.OrderBy(query, input.Sorting ?? "CreationTime desc");
            var items = await ordered.PageBy(input).ToListAsync();

            return new PagedResultDto<MassNotificationDto>(total, ObjectMapper.Map<List<MassNotificationDto>>(items));
        }

        /// <summary>
        /// Cria uma nova notificação em massa e agenda o envio.
        /// </summary>
        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_MassNotifications_Create)]
        public virtual async Task<MassNotificationDto> CreateAsync(CreateMassNotificationInput input)
        {
            var massNotification = ObjectMapper.Map<MassNotification>(input);
            massNotification.TenantId = AbpSession.TenantId;
            massNotification.Status = MassNotificationStatus.Pending;

            await _massNotificationRepository.InsertAsync(massNotification);

            var delay = input.ScheduledTime.HasValue && input.ScheduledTime.Value > Clock.Now
                ? input.ScheduledTime.Value - Clock.Now
                : (TimeSpan?)null;

            await _backgroundJobManager.EnqueueAsync<MassNotificationJob, MassNotificationJobArgs>(
                new MassNotificationJobArgs { MassNotificationId = massNotification.Id },
                delay: delay.HasValue && delay.Value > TimeSpan.Zero ? delay.Value : (TimeSpan?)null);

            return ObjectMapper.Map<MassNotificationDto>(massNotification);
        }

        /// <summary>
        /// Cancela uma notificação em massa.
        /// </summary>
        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_MassNotifications_Delete)]
        public virtual async Task CancelAsync(EntityDto<long> input)
        {
            var massNotification = await _massNotificationRepository.GetAsync(input.Id);
            massNotification.Status = MassNotificationStatus.Canceled;
            await _massNotificationRepository.UpdateAsync(massNotification);
        }
    }
}
