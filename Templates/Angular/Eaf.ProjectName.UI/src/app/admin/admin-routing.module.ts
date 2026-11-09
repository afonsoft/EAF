import { NgModule } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';

import { AuditLogsComponent } from './audit-logs/audit-logs.component';
import { DynamicPropertiesComponent } from './dynamic-properties/dynamic-properties.component';
import { FeaturesComponent } from './features/features.component';
import { LanguageTextsComponent } from './languages/language-texts.component';
import { LanguagesComponent } from './languages/languages.component';
import { EditionsComponent } from './editions/editions.component';
import { MaintenanceComponent } from './maintenance/maintenance.component';
import { MassNotificationsComponent } from './mass-notifications/mass-notifications.component';
import { OrganizationUnitsComponent } from './organization-units/organization-units.component';
import { PaymentsComponent } from './payments/payments.component';
import { SubscriptionsComponent } from './subscriptions/subscriptions.component';
import { RolesComponent } from './roles/roles.component';
import { UserDelegationsComponent } from './user-delegations/user-delegations.component';
import { SettingsComponent } from './settings/settings.component';
import { TenantsComponent } from './tenants/tenants.component';
import { UiCustomizationComponent } from './ui-customization/ui-customization.component';
import { UsersComponent } from './users/users.component';
import { TenantJoinRequestsComponent } from './tenant-join-requests/tenant-join-requests.component';
import { UserSessionsComponent } from './user-sessions/user-sessions.component';
import { AdminApiKeysComponent } from './api-keys/admin-api-keys.component';
import { WorkflowsComponent } from './workflows/workflows.component';
import { WorkflowDesignerComponent } from './workflows/designer.component';
import { EmailTemplatesComponent } from './email-templates/email-templates.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        children: [
          { path: 'users', component: UsersComponent, data: { permission: 'Pages.Administration.Users' } },
          { path: 'roles', component: RolesComponent, data: { permission: 'Pages.Administration.Roles' } },
          { path: 'auditLogs', component: AuditLogsComponent, data: { permission: 'Pages.Administration.AuditLogs' } },
          { path: 'dynamic-properties', component: DynamicPropertiesComponent, data: { permission: 'Pages.Administration.DynamicProperties' } },
          { path: 'languages', component: LanguagesComponent, data: { permission: 'Pages.Administration.Languages' } },
          {
            path: 'languages/:name/texts',
            component: LanguageTextsComponent,
            data: { permission: 'Pages.Administration.Languages.ChangeTexts' },
          },
          { path: 'tenants', component: TenantsComponent, data: { permission: 'Pages.Tenants' } },
          { path: 'editions', component: EditionsComponent, data: { permission: 'Pages.Administration.Editions' } },
          { path: 'features', component: FeaturesComponent, data: { permission: 'Pages.Administration.Editions' } },
          { path: 'organization-units', component: OrganizationUnitsComponent, data: { permission: 'Pages.Administration.OrganizationUnits' } },
          { path: 'mass-notifications', component: MassNotificationsComponent, data: { permission: 'Pages.Administration.MassNotifications' } },
          { path: 'user-delegations', component: UserDelegationsComponent, data: { permission: 'Pages.Administration.Users.Delegation' } },
          { path: 'payments', component: PaymentsComponent, data: { permission: 'Pages.Administration.Payments' } },
          { path: 'subscriptions', component: SubscriptionsComponent, data: { permission: 'Pages.Administration.Payments' } },
          { path: 'settings', component: SettingsComponent, data: { permission: 'Pages.Administration.Settings' } },
          { path: 'maintenance', component: MaintenanceComponent, data: { permission: 'Pages.Administration.Maintenance' } },
          { path: 'tenant-join-requests', component: TenantJoinRequestsComponent, data: { permission: 'Pages.Administration.Users' } },
          { path: 'ui-customization', component: UiCustomizationComponent, data: { permission: 'Pages.Administration.UiCustomization' } },
          { path: 'user-sessions', component: UserSessionsComponent, data: { permission: 'Pages.Administration.UserSessions' } },
                    { path: 'api-keys', component: AdminApiKeysComponent, data: { permission: 'Pages.Administration.ApiKeys' } },
          { path: 'workflows', component: WorkflowsComponent, data: { permission: 'Pages.Administration.Workflows' } },
          { path: 'email-templates', component: EmailTemplatesComponent, data: { permission: 'Pages.Administration.EmailTemplates' } },
          { path: 'workflows/new', component: WorkflowDesignerComponent, data: { permission: 'Pages.Administration.Workflows' } },
          { path: 'workflows/:id', component: WorkflowDesignerComponent, data: { permission: 'Pages.Administration.Workflows' } },
        ],
      },
    ]),
  ],
  exports: [RouterModule],
})
export class AdminRoutingModule {
  constructor(private readonly router: Router) {
    router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        window.scroll(0, 0);
      }
    });
  }
}
