using System;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Uow;
using Abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Eaf.ProjectName.EntityFrameworkCore;

namespace Eaf.ProjectName.Migrations.Seed
{
    public static class SeedHelper
    {
        public static void SeedHostDb(IIocResolver iocResolver)
        {
            WithDbContext<ProjectNameDbContext>(iocResolver, SeedHostDb);
        }

        public static void SeedHostDb(ProjectNameDbContext context)
        {
            context.SuppressAutoSetTenantId = true;

            //seed
        }

        private static void WithDbContext<TDbContext>(IIocResolver iocResolver, Action<TDbContext> contextAction) where TDbContext : DbContext
        {
            using (var uowManager = iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
            using (var uow = uowManager.Object.Begin(TransactionScopeOption.Suppress))
            {
                var context = uowManager.Object.Current.GetDbContext<TDbContext>(MultiTenancySides.Host);
                contextAction(context);
                uow.Complete();
            }
        }
    }
}