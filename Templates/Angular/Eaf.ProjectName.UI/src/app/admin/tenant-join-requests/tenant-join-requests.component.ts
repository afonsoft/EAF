import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TenantJoinRequestService, TenantJoinRequestDto } from '@shared/service-proxies/tenant-join-request.service';

@Component({
  standalone: false,
  selector: 'app-tenant-join-requests',
  templateUrl: './tenant-join-requests.component.html',
})
export class TenantJoinRequestsComponent extends AppComponentBase implements OnInit {
  requests: TenantJoinRequestDto[] = [];
  loading = false;

  constructor(
    injector: Injector,
    private readonly _tenantJoinRequestService: TenantJoinRequestService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this._tenantJoinRequestService.getPendingRequestsForCurrentTenant().subscribe({
      next: result => {
        this.requests = result;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  approve(requestId: number): void {
    this.message.confirm(
      this.l('TenantJoinRequestApproveWarningMessage'),
      this.l('AreYouSure'),
      isConfirmed => {
        if (isConfirmed) {
          this._tenantJoinRequestService.approve({ requestId, isApproved: true }).subscribe(() => {
            this.message.success(this.l('SuccessfullyApproved'));
            this.load();
          });
        }
      }
    );
  }

  reject(requestId: number): void {
    this.message.confirm(
      this.l('TenantJoinRequestRejectWarningMessage'),
      this.l('AreYouSure'),
      isConfirmed => {
        if (isConfirmed) {
          this._tenantJoinRequestService.approve({ requestId, isApproved: false }).subscribe(() => {
            this.message.success(this.l('SuccessfullyRejected'));
            this.load();
          });
        }
      }
    );
  }
}
