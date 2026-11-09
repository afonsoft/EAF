import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AdminWorkflowService, WorkflowDefinitionDto } from '@shared/service-proxies/workflow.service';
import { finalize } from 'rxjs/operators';

@Component({
    standalone: false,
    templateUrl: './workflows.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class WorkflowsComponent extends AppComponentBase implements OnInit {
    definitions: WorkflowDefinitionDto[] = [];
    loading = false;

    constructor(
        injector: Injector,
        private readonly _workflowService: AdminWorkflowService,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.load();
    }

    load(): void {
        this.loading = true;
        this._workflowService
            .getAll()
            .pipe(finalize(() => (this.loading = false)))
            .subscribe(defs => (this.definitions = defs));
    }

    publish(def: WorkflowDefinitionDto): void {
        this.message.confirm(
            this.l('WorkflowPublishConfirmMessage', def.name),
            this.l('AreYouSure'),
            isConfirmed => {
                if (!isConfirmed) {
                    return;
                }
                this._workflowService.publish(def.id).subscribe(() => {
                    this.notify.success(this.l('WorkflowPublished'));
                    this.load();
                });
            },
        );
    }

    archive(def: WorkflowDefinitionDto): void {
        this.message.confirm(
            this.l('WorkflowArchiveConfirmMessage', def.name),
            this.l('AreYouSure'),
            isConfirmed => {
                if (!isConfirmed) {
                    return;
                }
                this._workflowService.archive(def.id).subscribe(() => {
                    this.notify.success(this.l('WorkflowArchived'));
                    this.load();
                });
            },
        );
    }

    delete(def: WorkflowDefinitionDto): void {
        this.message.confirm(
            this.l('WorkflowDeleteConfirmMessage', def.name),
            this.l('AreYouSure'),
            isConfirmed => {
                if (!isConfirmed) {
                    return;
                }
                this._workflowService.delete(def.id).subscribe(() => {
                    this.notify.success(this.l('WorkflowDeleted'));
                    this.load();
                });
            },
        );
    }

    statusClass(status: string): string {
        switch (status) {
            case 'Published': return 'badge-success';
            case 'Draft': return 'badge-warning';
            default: return 'badge-secondary';
        }
    }
}
