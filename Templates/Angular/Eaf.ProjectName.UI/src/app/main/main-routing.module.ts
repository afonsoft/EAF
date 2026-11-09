import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { PasskeysComponent } from './passkeys/passkeys.component';
import { ApiKeysComponent } from './api-keys/api-keys.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        children: [
          { path: 'dashboard', component: DashboardComponent, data: { permission: 'Pages.Dashboard' } },
          { path: 'passkeys', component: PasskeysComponent },
                    {
                        path: 'api-keys', component: ApiKeysComponent },
        ],
      },
    ]),
  ],
  exports: [RouterModule],
})
export class MainRoutingModule {}
