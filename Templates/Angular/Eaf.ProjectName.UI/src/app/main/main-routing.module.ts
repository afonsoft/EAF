import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ApiKeysComponent } from './api-keys/api-keys.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        children: [
          { path: 'dashboard', component: DashboardComponent, data: { permission: 'Pages.Dashboard' } },
          { path: 'api-keys', component: ApiKeysComponent },
        ],
      },
    ]),
  ],
  exports: [RouterModule],
})
export class MainRoutingModule {}
