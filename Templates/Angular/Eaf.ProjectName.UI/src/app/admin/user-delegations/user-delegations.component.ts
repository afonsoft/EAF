import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ICreateUserDelegationInput, IUserDelegationDto, UserDelegationServiceProxy } from '@shared/service-proxies/user-delegation.service-proxy';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
    standalone: false,
    templateUrl: './user-delegations.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class UserDelegationsComponent extends AppComponentBase implements OnInit {
    @ViewChild('createModal', { static: true }) modal: ModalDirective;

    myDelegations: IUserDelegationDto[] = [];
    delegatedUsers: IUserDelegationDto[] = [];
    loading = false;
    saving = false;

    activeTab: 'myDelegations' | 'delegatedUsers' = 'myDelegations';

    newDelegation: ICreateUserDelegationInput = {
        targetUserId: 0,
        startTime: '',
        endTime: '',
        description: '',
    };

    constructor(
        injector: Injector,
        private readonly _userDelegationService: UserDelegationServiceProxy,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.loadDelegations();
    }

    loadDelegations(): void {
        this.loading = true;
        this._userDelegationService
            .getMyDelegations({ maxResultCount: 1000 })
            .pipe(finalize(() => (this.loading = false)))
            .subscribe(result => {
                this.myDelegations = result.items ?? [];
            });

        this._userDelegationService
            .getDelegatedUsers({ maxResultCount: 1000 })
            .pipe(finalize(() => (this.loading = false)))
            .subscribe(result => {
                this.delegatedUsers = result.items ?? [];
            });
    }

    showCreateModal(): void {
        this.newDelegation = { targetUserId: 0, startTime: '', endTime: '', description: '' };
        this.modal.show();
    }

    closeModal(): void {
        this.modal.hide();
    }

    save(): void {
        if (!this.newDelegation.targetUserId || !this.newDelegation.startTime || !this.newDelegation.endTime) {
            this.notify.warn(this.l('ThisFieldIsRequired'));
            return;
        }

        this.saving = true;
        this._userDelegationService
            .create(this.newDelegation)
            .pipe(finalize(() => (this.saving = false)))
            .subscribe(() => {
                this.notify.success(this.l('SavedSuccessfully'));
                this.closeModal();
                this.loadDelegations();
            });
    }

    cancel(delegation: IUserDelegationDto): void {
        this.message.confirm(this.l('AreYouSure'), this.l('UserDelegation'), isConfirmed => {
            if (isConfirmed) {
                this._userDelegationService.cancel(delegation.id).subscribe(() => {
                    this.notify.success(this.l('SuccessfullyDeleted'));
                    this.loadDelegations();
                });
            }
        });
    }
}
