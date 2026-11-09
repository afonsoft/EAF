import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SessionService, UserSessionDto } from '@shared/service-proxies/session.service';
import { finalize } from 'rxjs/operators';

@Component({
    standalone: false,
    templateUrl: './sessions.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class SessionsComponent extends AppComponentBase implements OnInit {
    sessions: UserSessionDto[] = [];
    loading = false;

    constructor(injector: Injector, private _sessionService: SessionService) {
        super(injector);
    }

    ngOnInit(): void {
        this.loadSessions();
    }

    loadSessions(): void {
        this.loading = true;
        this._sessionService.getMySessions()
            .pipe(finalize(() => this.loading = false))
            .subscribe(result => this.sessions = result);
    }

    revokeSession(session: UserSessionDto): void {
        this.message.confirm(
            this.l('SessionRevokeWarningMessage'),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                    this._sessionService.revokeMySession(session.sessionId).subscribe(() => {
                        this.sessions = this.sessions.filter(s => s.sessionId !== session.sessionId);
                        this.notify.success(this.l('SessionRevoked'));
                        if (session.isCurrent) {
                            location.href = 'account/login';
                        }
                    });
                }
            }
        );
    }

    revokeAllSessions(): void {
        this.message.confirm(
            this.l('RevokeAllSessionsWarningMessage'),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                    this._sessionService.revokeAllMySessions(true).subscribe(() => {
                        this.loadSessions();
                        this.notify.success(this.l('SessionsRevoked'));
                    });
                }
            }
        );
    }
}
