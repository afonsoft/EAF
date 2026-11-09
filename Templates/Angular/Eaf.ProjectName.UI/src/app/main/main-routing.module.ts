import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { SessionsComponent } from './sessions/sessions.component';
import { PasskeysComponent } from './passkeys/passkeys.component';
import { ApiKeysComponent } from './api-keys/api-keys.component';
import { WorkflowApprovalsComponent } from './workflow-approvals/workflow-approvals.component';
import { WorkflowCatalogComponent } from './workflow-catalog/workflow-catalog.component';
import { MyRequestsComponent } from './my-requests/my-requests.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        children: [
          { path: 'dashboard', component: DashboardComponent, data: { permission: 'Pages.Dashboard' } },
          { path: 'passkeys', component: PasskeysComponent },
          { path: 'sessions', component: SessionsComponent },
          { path: 'workflow-approvals', component: WorkflowApprovalsComponent, data: { permission: 'Pages.Workflows.Approvals' } },
          { path: 'workflow-catalog', component: WorkflowCatalogComponent, data: { permission: 'Pages.Workflows' } },
          { path: 'my-requests', component: MyRequestsComponent, data: { permission: 'Pages.Workflows' } },
                    {
                        path: 'api-keys', component: ApiKeysComponent },
        ],
      },
    ]),
  ],
  exports: [RouterModule],
})
export class MainRoutingModule {}
