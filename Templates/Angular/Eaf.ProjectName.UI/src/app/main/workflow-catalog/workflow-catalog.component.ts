import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
    WorkflowDefinitionDto,
    WorkflowFormFieldModel,
    WorkflowRequestService,
    WorkflowSimulationDto,
} from '@shared/service-proxies/workflow.service';
import { finalize } from 'rxjs/operators';

@Component({
    standalone: false,
    templateUrl: './workflow-catalog.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class WorkflowCatalogComponent extends AppComponentBase implements OnInit {
    definitions: WorkflowDefinitionDto[] = [];
    loading = false;

    selected: WorkflowDefinitionDto | undefined;
    fields: WorkflowFormFieldModel[] = [];
    values: { [id: string]: unknown } = {};
    showStartModal = false;
    submitting = false;
    simulation: WorkflowSimulationDto | undefined;

    constructor(
        injector: Injector,
        private readonly _workflowService: WorkflowRequestService,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.loading = true;
        this._workflowService
            .getCatalog()
            .pipe(finalize(() => (this.loading = false)))
            .subscribe(defs => (this.definitions = defs));
    }

    openStart(def: WorkflowDefinitionDto): void {
        this.selected = def;
        this.values = {};
        this.simulation = undefined;
        try {
            this.fields = (JSON.parse(def.formJson)?.fields ?? []) as WorkflowFormFieldModel[];
        } catch {
            this.fields = [];
        }
        this.showStartModal = true;
    }

    closeStart(): void {
        this.showStartModal = false;
        this.selected = undefined;
    }

    simulate(): void {
        this._workflowService.simulate(this.selected!.id, this.values).subscribe(r => (this.simulation = r));
    }

    start(): void {
        this.submitting = true;
        this._workflowService
            .start(this.selected!.id, this.values)
            .pipe(finalize(() => (this.submitting = false)))
            .subscribe(() => {
                this.notify.success(this.l('WorkflowStarted'));
                this.closeStart();
            });
    }
}
