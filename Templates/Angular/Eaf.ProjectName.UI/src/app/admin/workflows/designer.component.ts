import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
    AdminWorkflowService,
    WorkflowDefinitionDto,
    WorkflowFormFieldModel,
    WorkflowStepModel,
} from '@shared/service-proxies/workflow.service';
import { finalize } from 'rxjs/operators';

interface FormModel { fields: WorkflowFormFieldModel[]; }
interface FlowModel { startStepId: string; steps: WorkflowStepModel[]; }

@Component({
    standalone: false,
    templateUrl: './designer.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class WorkflowDesignerComponent extends AppComponentBase implements OnInit {
    definition: Partial<WorkflowDefinitionDto> = { name: '' };
    form: FormModel = { fields: [] };
    flow: FlowModel = { startStepId: '', steps: [] };
    loading = false;
    saving = false;
    isNew = true;

    readonly fieldTypes = ['text', 'textarea', 'number', 'date', 'select', 'checkbox'];
    readonly stepTypes = ['approval', 'condition', 'notification', 'end'];
    readonly assigneeKinds = ['users', 'role', 'ou', 'manager'];
    readonly quorumKinds = ['any', 'all', 'quorum'];

    constructor(
        injector: Injector,
        private readonly _workflowService: AdminWorkflowService,
        private readonly _route: ActivatedRoute,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        const id = this._route.snapshot.paramMap.get('id');
        if (id && id !== 'new') {
            this.isNew = false;
            this.loading = true;
            this._workflowService
                .get(id)
                .pipe(finalize(() => (this.loading = false)))
                .subscribe(def => {
                    this.definition = def;
                    this.form = this.parseJson<FormModel>(def.formJson, { fields: [] });
                    this.flow = this.parseJson<FlowModel>(def.flowJson, { startStepId: '', steps: [] });
                    this.flow.steps.forEach(s => this.ensureAssignee(s));
                });
        }
    }

    addField(): void {
        this.form.fields.push({ id: '', label: '', type: 'text', required: false, options: undefined });
    }

    removeField(index: number): void {
        this.form.fields.splice(index, 1);
    }

    addStep(): void {
        this.flow.steps.push({ id: '', name: '', type: 'approval', nextStepId: '', rejectStepId: '', condition: '', assignee: { kind: 'users', ids: [], quorum: 'any', quorumCount: undefined }, dueTimeHours: undefined });
    }

    ensureAssignee(step: WorkflowStepModel): void {
        if (!step.assignee) {
            step.assignee = { kind: 'users', ids: [], quorum: 'any', quorumCount: undefined };
        }
    }

    setFieldOptions(field: WorkflowFormFieldModel, text: string): void {
        field.options = text.split(',').map(o => o.trim()).filter(o => !!o);
    }

    removeStep(index: number): void {
        this.flow.steps.splice(index, 1);
    }

    assigneeIdsText(step: WorkflowStepModel): string {
        return (step.assignee?.ids ?? []).join(',');
    }

    setAssigneeIds(step: WorkflowStepModel, text: string): void {
        this.ensureAssignee(step);
        step.assignee!.ids = text.split(',').map(s => s.trim()).filter(s => !!s);
    }

    stepIds(): string[] {
        return this.flow.steps.map(s => s.id).filter(s => !!s);
    }

    save(publish = false): void {
        this.saving = true;
        this._workflowService
            .createOrUpdate({
                id: this.definition.id,
                name: this.definition.name,
                description: this.definition.description,
                category: this.definition.category,
                formJson: JSON.stringify(this.form),
                flowJson: JSON.stringify(this.flow),
            })
            .pipe(finalize(() => (this.saving = false)))
            .subscribe(saved => {
                this.definition = saved;
                this.isNew = false;
                this.notify.success(this.l('WorkflowSaved'));
                if (publish) {
                    this._workflowService.publish(saved.id).subscribe(() => {
                        this.notify.success(this.l('WorkflowPublished'));
                        this.definition.status = 'Published';
                    });
                }
            });
    }

    private parseJson<T>(json: string, fallback: T): T {
        try {
            return json ? JSON.parse(json) : fallback;
        } catch {
            return fallback;
        }
    }
}
