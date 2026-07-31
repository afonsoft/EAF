import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { IOrganizationUnitDto, OrganizationUnitServiceProxy } from '@shared/service-proxies/organization-unit.service-proxy';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

interface IFlatOrganizationUnit extends IOrganizationUnitDto {
    level: number;
}

@Component({
    standalone: false,
    templateUrl: './organization-units.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class OrganizationUnitsComponent extends AppComponentBase implements OnInit {
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    organizationUnits: IOrganizationUnitDto[] = [];
    flatOrganizationUnits: IFlatOrganizationUnit[] = [];
    loading = false;
    saving = false;

    activeOu: IOrganizationUnitDto = { id: 0, displayName: '', code: '', children: [] };
    isEdit = false;

    constructor(
        injector: Injector,
        private readonly _organizationUnitService: OrganizationUnitServiceProxy,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.loadOrganizationUnits();
    }

    loadOrganizationUnits(): void {
        this.loading = true;
        this._organizationUnitService
            .getOrganizationUnits()
            .pipe(finalize(() => (this.loading = false)))
            .subscribe(result => {
                this.organizationUnits = result ?? [];
                this.flattenOrganizationUnits();
            });
    }

    flattenOrganizationUnits(): void {
        this.flatOrganizationUnits = [];
        const walk = (items: IOrganizationUnitDto[], level: number) => {
            if (!items) return;
            for (const item of items) {
                this.flatOrganizationUnits.push({ ...item, level });
                walk(item.children, level + 1);
            }
        };
        walk(this.organizationUnits, 0);
    }

    showCreateModal(parentId?: number): void {
        this.isEdit = false;
        this.activeOu = { id: 0, displayName: '', code: '', parentId, children: [] };
        this.modal.show();
    }

    showEditModal(ou: IOrganizationUnitDto): void {
        this.isEdit = true;
        this.activeOu = { ...ou, children: ou.children ?? [] };
        this.modal.show();
    }

    save(): void {
        if (!this.activeOu.displayName) {
            this.notify.warn(this.l('RequiredField', this.l('Name')));
            return;
        }

        this.saving = true;
        const request = this.isEdit
            ? this._organizationUnitService.update({ id: this.activeOu.id, displayName: this.activeOu.displayName })
            : this._organizationUnitService.create({ displayName: this.activeOu.displayName, parentId: this.activeOu.parentId });

        request.pipe(finalize(() => (this.saving = false))).subscribe(() => {
            this.notify.success(this.l('SavedSuccessfully'));
            this.closeModal();
            this.loadOrganizationUnits();
        });
    }

    deleteOu(ou: IOrganizationUnitDto): void {
        this.message.confirm('', this.l('OrganizationUnitDeleteWarningMessage', ou.displayName), isConfirmed => {
            if (isConfirmed) {
                this._organizationUnitService.delete(ou.id).subscribe(() => {
                    this.notify.success(this.l('SuccessfullyDeleted'));
                    this.loadOrganizationUnits();
                });
            }
        });
    }

    closeModal(): void {
        this.modal.hide();
    }

    indent(level: number): string {
        return 'padding-left: ' + level * 24 + 'px;';
    }
}
