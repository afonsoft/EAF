using Abp.Application.Editions;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Timing;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Core.Editions;
using Eaf.Middleware.Dashboard.Dto;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Payments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Dashboard
{
    /// <summary>
    /// Serviço de aplicação que retorna estatísticas para o dashboard.
    /// </summary>
    [AbpAuthorize(MiddlewarePermissions.Pages_Dashboard)]
    public class DashboardAppService : MiddlewareAppServiceBase, IDashboardAppService
    {
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Edition, int> _editionRepository;
        private readonly IRepository<SubscriptionPayment, long> _subscriptionPaymentRepository;

        /// <summary>
        /// DashboardAppService.
        /// </summary>
        /// <param name="tenantRepository">Repositório de tenants.</param>
        /// <param name="userRepository">Repositório de usuários.</param>
        /// <param name="editionRepository">Repositório de edições.</param>
        /// <param name="subscriptionPaymentRepository">Repositório de pagamentos.</param>
        public DashboardAppService(
            IRepository<Tenant, int> tenantRepository,
            IRepository<User, long> userRepository,
            IRepository<Edition, int> editionRepository,
            IRepository<SubscriptionPayment, long> subscriptionPaymentRepository = null)
        {
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _editionRepository = editionRepository;
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
        }

        /// <summary>
        /// Obtém os dados do dashboard para o host.
        /// </summary>
        /// <returns>Dados do dashboard do host.</returns>
        public virtual async Task<DashboardOutput> GetHostDashboardAsync()
        {
            var output = new DashboardOutput { IsHostDashboard = true };

            output.Tiles.Add(new DashboardTileDto
            {
                Id = "totalTenants",
                Title = L("TotalTenants"),
                Count = await _tenantRepository.CountAsync(),
                Style = "primary",
                Icon = "flaticon-users-1"
            });

            output.Tiles.Add(new DashboardTileDto
            {
                Id = "totalUsers",
                Title = L("TotalUsers"),
                Count = await _userRepository.CountAsync(),
                Style = "success",
                Icon = "flaticon-users"
            });

            output.Tiles.Add(new DashboardTileDto
            {
                Id = "totalEditions",
                Title = L("TotalEditions"),
                Count = await _editionRepository.CountAsync(),
                Style = "warning",
                Icon = "flaticon-layers"
            });

            if (_subscriptionPaymentRepository != null)
            {
                output.Tiles.AddRange(await GetPaymentTilesAsync());
                output.Tiles.AddRange(await GetSubscriptionTilesAsync());
            }

            return output;
        }

        /// <summary>
        /// Obtém os dados do dashboard para o tenant atual.
        /// </summary>
        /// <returns>Dados do dashboard do tenant.</returns>
        public virtual async Task<DashboardOutput> GetTenantDashboardAsync()
        {
            var output = new DashboardOutput { IsHostDashboard = false };

            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                output.Tiles.Add(new DashboardTileDto
                {
                    Id = "totalUsers",
                    Title = L("TotalUsers"),
                    Count = await _userRepository.CountAsync(),
                    Style = "success",
                    Icon = "flaticon-users"
                });

                var mySubscription = await GetMySubscriptionTileAsync();
                if (mySubscription != null)
                {
                    output.Tiles.Add(mySubscription);
                }
            }

            return output;
        }

        private async Task<DashboardTileDto> GetMySubscriptionTileAsync()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            var tenant = await _tenantRepository.GetAsync(AbpSession.TenantId.Value);
            if (tenant == null)
            {
                return null;
            }

            var edition = tenant.EditionId.HasValue ? await _editionRepository.GetAsync(tenant.EditionId.Value) : null;
            var remainingDays = tenant.SubscriptionEndDateUtc.HasValue
                ? (int)(tenant.SubscriptionEndDateUtc.Value.Date - Clock.Now.Date).TotalDays
                : (int?)null;

            return new DashboardTileDto
            {
                Id = "mySubscription",
                Title = edition != null ? $"{L("MySubscription")} - {edition.DisplayName}" : L("MySubscription"),
                Count = remainingDays ?? 0,
                Style = remainingDays == null || remainingDays >= 0 ? "info" : "danger",
                Icon = "flaticon-layers"
            };
        }

        private async Task<List<DashboardTileDto>> GetPaymentTilesAsync()
        {
            var payments = await _subscriptionPaymentRepository.GetAllListAsync();

            var total = payments.Count;
            var pending = payments.Count(p => p.Status == SubscriptionPaymentStatus.Pending);
            var completed = payments.Count(p => p.Status == SubscriptionPaymentStatus.Completed);
            var mrr = CalculateMrr(payments.Where(p => p.Status == SubscriptionPaymentStatus.Completed));

            return new List<DashboardTileDto>
            {
                new DashboardTileDto
                {
                    Id = "totalPayments",
                    Title = L("TotalPayments"),
                    Count = total,
                    Style = "info",
                    Icon = "flaticon-coins"
                },
                new DashboardTileDto
                {
                    Id = "pendingPayments",
                    Title = L("PendingPayments"),
                    Count = pending,
                    Style = "warning",
                    Icon = "flaticon-time"
                },
                new DashboardTileDto
                {
                    Id = "completedPayments",
                    Title = L("CompletedPayments"),
                    Count = completed,
                    Style = "success",
                    Icon = "flaticon-check"
                },
                new DashboardTileDto
                {
                    Id = "monthlyRecurringRevenue",
                    Title = L("MonthlyRecurringRevenue"),
                    Count = (long)mrr,
                    Style = "primary",
                    Icon = "flaticon-graph"
                }
            };
        }

        private async Task<List<DashboardTileDto>> GetSubscriptionTilesAsync()
        {
            var tenants = await _tenantRepository.GetAllListAsync(t => t.EditionId != null);
            var now = Clock.Now;

            var active = tenants.Count(t => !t.SubscriptionEndDateUtc.HasValue || t.SubscriptionEndDateUtc.Value.Date >= now.Date);
            var expired = tenants.Count(t => t.SubscriptionEndDateUtc.HasValue && t.SubscriptionEndDateUtc.Value.Date < now.Date);

            return new List<DashboardTileDto>
            {
                new DashboardTileDto
                {
                    Id = "tenantsWithActiveSubscription",
                    Title = L("TenantsWithActiveSubscription"),
                    Count = active,
                    Style = "success",
                    Icon = "flaticon-users-1"
                },
                new DashboardTileDto
                {
                    Id = "tenantsWithExpiredSubscription",
                    Title = L("TenantsWithExpiredSubscription"),
                    Count = expired,
                    Style = "danger",
                    Icon = "flaticon-warning"
                }
            };
        }

        private static decimal CalculateMrr(IEnumerable<SubscriptionPayment> completedPayments)
        {
            decimal mrr = 0;
            foreach (var payment in completedPayments)
            {
                mrr += payment.PaymentPeriodType switch
                {
                    PaymentPeriodType.Monthly => payment.Amount,
                    PaymentPeriodType.Quarterly => payment.Amount / 3,
                    PaymentPeriodType.Biannual => payment.Amount / 6,
                    PaymentPeriodType.Annual => payment.Amount / 12,
                    _ => 0
                };
            }

            return mrr;
        }
    }
}
