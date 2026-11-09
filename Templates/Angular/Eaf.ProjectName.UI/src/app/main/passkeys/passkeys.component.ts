import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { PasskeyService, PasskeyDto } from '@shared/service-proxies/passkey.service';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
    standalone: false,
    templateUrl: './passkeys.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class PasskeysComponent extends AppComponentBase implements OnInit {
    @ViewChild('createModal', { static: true }) createModal: ModalDirective;

    passkeys: PasskeyDto[] = [];
    loading = false;
    saving = false;
    newKeyName = '';

    get webauthnSupported(): boolean {
        return PasskeyService.isSupported();
    }

    constructor(injector: Injector, private _passkeyService: PasskeyService) {
        super(injector);
    }

    ngOnInit(): void {
        this.loadPasskeys();
    }

    loadPasskeys(): void {
        this.loading = true;
        this._passkeyService.getMyPasskeys()
            .pipe(finalize(() => this.loading = false))
            .subscribe(result => this.passkeys = result);
    }

    openCreateModal(): void {
        this.newKeyName = '';
        this.createModal.show();
    }

    async save(): Promise<void> {
        this.saving = true;
        try {
            const passkey = await this._passkeyService.registerPasskey(this.newKeyName);
            this.passkeys = [...this.passkeys, passkey];
            this.createModal.hide();
            this.notify.success(this.l('PasskeyRegistered'));
        } catch (e) {
            if (e && (e as any).name === 'NotAllowedError') {
                this.notify.warn(this.l('PasskeyCancelled'));
            } else {
                this.notify.error(this.l('PasskeyRegistrationFailed'));
            }
        } finally {
            this.saving = false;
        }
    }

    deletePasskey(passkey: PasskeyDto): void {
        this.message.confirm(
            this.l('PasskeyDeleteWarningMessage', passkey.name),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                    this._passkeyService.deletePasskey(passkey.id).subscribe(() => {
                        this.passkeys = this.passkeys.filter(k => k.id !== passkey.id);
                        this.notify.success(this.l('SuccessfullyDeleted'));
                    });
                }
            }
        );
    }
}
