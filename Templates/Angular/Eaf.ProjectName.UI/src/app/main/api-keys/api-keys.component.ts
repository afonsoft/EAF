import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ApiKeyService, ApiKeyDto, ApiKeyStatus, CreateApiKeyInput, CreatedApiKeyDto } from '@shared/service-proxies/api-key.service';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
    standalone: false,
    templateUrl: './api-keys.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class ApiKeysComponent extends AppComponentBase implements OnInit {
    @ViewChild('createModal', { static: true }) createModal: ModalDirective;
    @ViewChild('keyModal', { static: true }) keyModal: ModalDirective;

    apiKeys: ApiKeyDto[] = [];
    loading = false;
    saving = false;

    newKey: CreateApiKeyInput = { name: '', permissionScopes: undefined, allowedIpRanges: undefined, expirationTime: undefined };
    scopesText = '';
    ipRangesText = '';

    createdKey: CreatedApiKeyDto | undefined;
    keyCopied = false;

    readonly ApiKeyStatus = ApiKeyStatus;

    constructor(
        injector: Injector,
        private readonly _apiKeyService: ApiKeyService,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.load();
    }

    load(): void {
        this.loading = true;
        this._apiKeyService
            .getMyApiKeys()
            .pipe(finalize(() => (this.loading = false)))
            .subscribe(keys => (this.apiKeys = keys));
    }

    showCreateModal(): void {
        this.newKey = { name: '', permissionScopes: undefined, allowedIpRanges: undefined, expirationTime: undefined };
        this.scopesText = '';
        this.ipRangesText = '';
        this.createModal.show();
    }

    closeCreateModal(): void {
        this.createModal.hide();
    }

    save(): void {
        const input: CreateApiKeyInput = {
            name: this.newKey.name?.trim(),
            permissionScopes: this.parseCsv(this.scopesText),
            allowedIpRanges: this.parseCsv(this.ipRangesText),
            expirationTime: this.newKey.expirationTime ? new Date(this.newKey.expirationTime).toISOString() : undefined,
        };
        this.saving = true;
        this._apiKeyService
            .createApiKey(input)
            .pipe(finalize(() => (this.saving = false)))
            .subscribe(created => {
                this.closeCreateModal();
                this.showCreatedKey(created);
                this.load();
            });
    }

    revoke(key: ApiKeyDto): void {
        this.message.confirm(
            this.l('ApiKeyRevokeConfirmMessage', key.name, key.keyPrefix),
            this.l('ApiKeyRevokeConfirmTitle'),
            isConfirmed => {
                if (!isConfirmed) {
                    return;
                }
                this._apiKeyService.revokeApiKey(key.id).subscribe(() => {
                    this.notify.success(this.l('ApiKeyRevokedSuccessMessage'));
                    this.load();
                });
            },
        );
    }

    rotate(key: ApiKeyDto): void {
        this.message.confirm(
            this.l('ApiKeyRotateConfirmMessage', key.name),
            this.l('ApiKeyRotateConfirmTitle'),
            isConfirmed => {
                if (!isConfirmed) {
                    return;
                }
                this._apiKeyService.rotateApiKey(key.id).subscribe(created => {
                    this.showCreatedKey(created);
                    this.load();
                });
            },
        );
    }

    showCreatedKey(created: CreatedApiKeyDto): void {
        this.createdKey = created;
        this.keyCopied = false;
        this.keyModal.show();
    }

    closeKeyModal(): void {
        this.keyModal.hide();
        this.createdKey = undefined;
    }

    copyPlainKey(): void {
        if (!this.createdKey?.plainKey) {
            return;
        }
        navigator.clipboard.writeText(this.createdKey.plainKey).then(() => {
            this.keyCopied = true;
            this.notify.success(this.l('CopiedToClipboard'));
        });
    }

    private parseCsv(text: string): string[] | undefined {
        const parts = (text ?? '')
            .split(',')
            .map(s => s.trim())
            .filter(s => s.length > 0);
        return parts.length ? parts : undefined;
    }
}
