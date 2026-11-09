import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { QrLoginService } from '@shared/service-proxies/qr-login.service';
import { finalize } from 'rxjs/operators';

@Component({
    standalone: false,
    templateUrl: './qr-approve.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class QrApproveComponent extends AppComponentBase implements OnInit {
    tokenId = '';
    approving = false;
    approved = false;

    constructor(
        injector: Injector,
        private _route: ActivatedRoute,
        private _qrLoginService: QrLoginService,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.tokenId = this._route.snapshot.queryParams['token'] || '';
    }

    approve(): void {
        if (!this.tokenId) {
            this.message.warn(this.l('QrLoginTokenRequired'));
            return;
        }
        this.approving = true;
        this._qrLoginService.approve(this.tokenId)
            .pipe(finalize(() => this.approving = false))
            .subscribe({
                next: () => {
                    this.approved = true;
                    this.notify.success(this.l('QrLoginApproved'));
                },
                error: () => this.message.error(this.l('QrLoginTokenExpired')),
            });
    }
}
