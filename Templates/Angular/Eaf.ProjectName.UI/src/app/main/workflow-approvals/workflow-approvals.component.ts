import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { WorkflowApprovalService, WorkflowTaskDto } from '@shared/service-proxies/workflow.service';
import { finalize } from 'rxjs/operators';

@Component({
    standalone: false,
    templateUrl: './workflow-approvals.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class WorkflowApprovalsComponent extends AppComponentBase implements OnInit {
    tasks: WorkflowTaskDto[] = [];
    loading = false;

    constructor(injector: Injector, private _workflowService: WorkflowApprovalService) {
        super(injector);
    }

    ngOnInit(): void {
        this.loadTasks();
    }

    loadTasks(): void {
        this.loading = true;
        this._workflowService.getMyPending()
            .pipe(finalize(() => this.loading = false))
            .subscribe(result => this.tasks = result);
    }

    decide(task: WorkflowTaskDto, decision: 'approve' | 'reject' | 'send-back'): void {
        this.message.confirm(
            this.l('WorkflowDecideWarningMessage', this.l(decision === 'approve' ? 'Approve' : decision === 'reject' ? 'Reject' : 'SendBack')),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                    this._workflowService.decide(task.id, decision, task.comment).subscribe(() => {
                        this.tasks = this.tasks.filter(t => t.id !== task.id);
                        this.notify.success(this.l('WorkflowDecided'));
                    });
                }
            }
        );
    }

    isOverdue(task: WorkflowTaskDto): boolean {
        return !!task.dueTime && new Date(task.dueTime) < new Date();
    }
}
