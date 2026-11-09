import { Injectable, Injector } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';

export class QrLoginCreateResult {
  tokenId: string;
  expiresInSeconds: number;
}

export class QrLoginStatusResult {
  status: string;
  accessToken: string | undefined;
  encryptedAccessToken: string | undefined;
  expireInSeconds: number | undefined;
  userId: number | undefined;
}

@Injectable()
export class QrLoginService extends AppComponentBase {
  private readonly baseUrl: string;

  constructor(injector: Injector, private _http: HttpClient) {
    super(injector);
    this.baseUrl = AppConsts.remoteServiceBaseUrl + '/api/QrLogin';
  }

  isEnabled(): Observable<boolean> {
    return this._http.get<boolean>(`${this.baseUrl}/IsEnabled`);
  }

  create(): Observable<QrLoginCreateResult> {
    return this._http.post<QrLoginCreateResult>(`${this.baseUrl}/Create`, {});
  }

  status(tokenId: string): Observable<QrLoginStatusResult> {
    return this._http.get<QrLoginStatusResult>(`${this.baseUrl}/Status`, {
      params: { tokenId },
    });
  }

  approve(tokenId: string): Observable<void> {
    return this._http.post<void>(`${this.baseUrl}/Approve`, { tokenId });
  }
}
