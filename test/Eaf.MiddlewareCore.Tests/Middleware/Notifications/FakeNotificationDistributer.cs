using Abp.Configuration;
using Abp.Notifications;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Notifications
{
    public class FakeNotificationDistributer : INotificationDistributer
    {
        public bool IsDistributeCalled { get; set; }

        public void Distribute(Guid notificationId)
        {
            IsDistributeCalled = true;
        }

        public async Task DistributeAsync(Guid notificationId)
        {
            await Task.CompletedTask;

            IsDistributeCalled = true;
        }
    }

    public class FakeNotificationProvider : NotificationProvider
    {
        public override void SetNotifications(INotificationDefinitionContext context)
        {
            return;
        }
    }

    public class FakeAzureActiveDirectorySettingProvider : Eaf.Middleware.AzureActiveDirectory.Configuration.AzureActiveDirectorySettingProvider
    {
        public override System.Collections.Generic.IEnumerable<Abp.Configuration.SettingDefinition> GetSettingDefinitions(Abp.Configuration.SettingDefinitionProviderContext context)
        {
            return new[]
                {
                    new Abp.Configuration.SettingDefinition(Eaf.Middleware.AzureActiveDirectory.Configuration.AzureActiveDirectorySettingNames.IsEnabled, "true", L("AzureActiveDirectory_IsEnabled"), scopes: Abp.Configuration.SettingScopes.Application),
                    new Abp.Configuration.SettingDefinition(Eaf.Middleware.AzureActiveDirectory.Configuration.AzureActiveDirectorySettingNames.ClientId, "fake-client-id", L("AzureActiveDirectory_ClientId"), scopes: Abp.Configuration.SettingScopes.Application),
                    new Abp.Configuration.SettingDefinition(Eaf.Middleware.AzureActiveDirectory.Configuration.AzureActiveDirectorySettingNames.Tenant, "fake-tenant", L("AzureActiveDirectory_Tenant"), scopes: Abp.Configuration.SettingScopes.Application),
                    new Abp.Configuration.SettingDefinition(Eaf.Middleware.AzureActiveDirectory.Configuration.AzureActiveDirectorySettingNames.ClientSecret, "fake-client-secret", L("AzureActiveDirectory_ClientSecret"), scopes: Abp.Configuration.SettingScopes.Application),
                };
        }
    }
}