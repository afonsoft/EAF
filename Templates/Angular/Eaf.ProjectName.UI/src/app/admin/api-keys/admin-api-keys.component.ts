import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AdminApiKeyService, ApiKeyDto, ApiKeyStatus } from '@shared/service-proxies/api-key.service';
import { finalize } from 'rxjs/operators';

@Component({
    standalone: false,
    templateUrl: './admin-api-keys.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class AdminApiKeysComponent extends AppComponentBase implements OnInit {
    apiKeys: ApiKeyDto[] = [];
    loading = false;

    readonly ApiKeyStatus = ApiKeyStatus;

    constructor(
        injector: Injector,
        private readonly _adminApiKeyService: AdminApiKeyService,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.load();
    }

    load(): void {
        this.loading = true;
        this._adminApiKeyService
            .getTenantApiKeys()
            .pipe(finalize(() => (this.loading = false)))
            .subscribe(keys => (this.apiKeys = keys));
    }

    revoke(key: ApiKeyDto): void {
        this.message.confirm(
            this.l('ApiKeyRevokeConfirmMessage', key.name, key.keyPrefix),
            this.l('ApiKeyRevokeConfirmTitle'),
            isConfirmed => {
                if (!isConfirmed) {
                    return;
                }
                this._adminApiKeyService.revokeApiKey(key.id).subscribe(() => {
                    this.notify.success(this.l('ApiKeyRevokedSuccessMessage'));
                    this.load();
                });
            },
        );
    }
}
