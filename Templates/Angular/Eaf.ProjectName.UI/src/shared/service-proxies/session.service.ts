import { Injectable, Injector } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';

export class UserSessionDto {
  sessionId: string;
  ipAddress: string | undefined;
  userAgent: string | undefined;
  creationTime: string;
  lastActivityTime: string;
  isCurrent: boolean;
}

export class ListResultOfUserSessionDto {
  items: UserSessionDto[];
}

/** Sessões ativas do usuário autenticado ("Active Sessions"). */
@Injectable()
export class SessionService extends AppComponentBase {
  private readonly _baseUrl = `${AppConsts.remoteServiceBaseUrl}/api/services/app/Session`;

  constructor(
    injector: Injector,
    private readonly _httpClient: HttpClient,
  ) {
    super(injector);
  }

  getMySessions(): Observable<UserSessionDto[]> {
    return this._httpClient
      .get<ListResultOfUserSessionDto>(`${this._baseUrl}/GetMySessions`)
      .pipe(map(r => r.items ?? []));
  }

  revokeMySession(sessionId: string): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/RevokeMySession`, { sessionId });
  }

  revokeAllMySessions(keepCurrentSession: boolean): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/RevokeAllMySessions`, { keepCurrentSession });
  }
}

/** Administração de sessões do tenant ("Administration > User Sessions"). */
@Injectable()
export class AdminSessionService extends AppComponentBase {
  private readonly _baseUrl = `${AppConsts.remoteServiceBaseUrl}/api/services/app/AdminSession`;

  constructor(
    injector: Injector,
    private readonly _httpClient: HttpClient,
  ) {
    super(injector);
  }

  getUserSessions(userId: number, tenantId?: number): Observable<UserSessionDto[]> {
    return this._httpClient
      .post<ListResultOfUserSessionDto>(`${this._baseUrl}/GetUserSessions`, { userId, tenantId })
      .pipe(map(r => r.items ?? []));
  }

  revokeSession(sessionId: string): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/RevokeSession`, { sessionId });
  }

  revokeAllUserSessions(userId: number, tenantId?: number): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/RevokeAllUserSessions`, { userId, tenantId });
  }
}
