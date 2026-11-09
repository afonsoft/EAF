import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AdminSessionService, UserSessionDto } from '@shared/service-proxies/session.service';
import { finalize } from 'rxjs/operators';

@Component({
    standalone: false,
    templateUrl: './user-sessions.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class UserSessionsComponent extends AppComponentBase {
    userIdInput: number | undefined;
    sessions: UserSessionDto[] = [];
    loading = false;
    searched = false;

    constructor(injector: Injector, private _adminSessionService: AdminSessionService) {
        super(injector);
    }

    search(): void {
        if (this.userIdInput === undefined || this.userIdInput === null) {
            return;
        }
        this.loading = true;
        this.searched = true;
        this._adminSessionService.getUserSessions(this.userIdInput, undefined)
            .pipe(finalize(() => this.loading = false))
            .subscribe(result => this.sessions = result);
    }

    revokeSession(session: UserSessionDto): void {
        this.message.confirm(
            this.l('SessionRevokeWarningMessage'),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                    this._adminSessionService.revokeSession(session.sessionId).subscribe(() => {
                        this.sessions = this.sessions.filter(s => s.sessionId !== session.sessionId);
                        this.notify.success(this.l('SessionRevoked'));
                    });
                }
            }
        );
    }

    revokeAllForUser(): void {
        this.message.confirm(
            this.l('RevokeAllSessionsWarningMessage'),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed && this.userIdInput !== undefined) {
                    this._adminSessionService
                        .revokeAllUserSessions(this.userIdInput, undefined)
                        .subscribe(() => {
                            this.sessions = [];
                            this.notify.success(this.l('SessionsRevoked'));
                        });
                }
            }
        );
    }
}
