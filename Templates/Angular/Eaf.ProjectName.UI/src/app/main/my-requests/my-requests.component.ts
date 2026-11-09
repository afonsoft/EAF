import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
    WorkflowInstanceDetailDto,
    WorkflowInstanceDto,
    WorkflowRequestService,
} from '@shared/service-proxies/workflow.service';
import { finalize } from 'rxjs/operators';

@Component({
    standalone: false,
    templateUrl: './my-requests.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class MyRequestsComponent extends AppComponentBase implements OnInit {
    requests: WorkflowInstanceDto[] = [];
    loading = false;

    detail: WorkflowInstanceDetailDto | undefined;
    showDetailModal = false;
    resubmitValues: { [id: string]: unknown } = {};
    resubmitFields: { id: string; label: string; type: string; required: boolean }[] = [];

    constructor(
        injector: Injector,
        private readonly _workflowService: WorkflowRequestService,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.load();
    }

    load(): void {
        this.loading = true;
        this._workflowService
            .getMyRequests()
            .pipe(finalize(() => (this.loading = false)))
            .subscribe(r => (this.requests = r));
    }

    openDetail(instance: WorkflowInstanceDto): void {
        this._workflowService.getDetail(instance.id).subscribe(d => {
            this.detail = d;
            try {
                this.resubmitValues = JSON.parse(instance.formValuesJson || '{}');
            } catch {
                this.resubmitValues = {};
            }
            this.showDetailModal = true;
        });
    }

    closeDetail(): void {
        this.showDetailModal = false;
        this.detail = undefined;
    }

    cancel(instance: WorkflowInstanceDto): void {
        this.message.confirm(
            this.l('WorkflowCancelConfirmMessage'),
            this.l('AreYouSure'),
            isConfirmed => {
                if (!isConfirmed) {
                    return;
                }
                this._workflowService.cancel(instance.id).subscribe(() => {
                    this.notify.success(this.l('WorkflowCancelled'));
                    this.load();
                });
            },
        );
    }

    resubmit(): void {
        if (!this.detail) {
            return;
        }
        this._workflowService.resubmit(this.detail.instance.id, this.resubmitValues).subscribe(() => {
            this.notify.success(this.l('WorkflowResubmitted'));
            this.closeDetail();
            this.load();
        });
    }

    statusClass(status: string): string {
        switch (status) {
            case 'Completed': return 'badge-success';
            case 'InProgress': return 'badge-primary';
            case 'SentBack': return 'badge-warning';
            case 'Rejected': return 'badge-danger';
            default: return 'badge-secondary';
        }
    }
}
