import { Injectable, Injector } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';

export class PasskeyDto {
  id: number;
  name: string;
  transports: string | undefined;
  isBackupEligible: boolean;
  lastUsedTime: string | undefined;
  creationTime: string;
}

export class PasskeyCeremonyOptionsDto {
  ceremonyId: string;
  options: any;
}

export class PasskeyAuthenticateResult {
  accessToken: string;
  encryptedAccessToken: string;
  expireInSeconds: number;
  userId: number;
}

function bufferToBase64Url(buffer: ArrayBuffer): string {
  const bytes = new Uint8Array(buffer);
  let binary = '';
  bytes.forEach(b => binary += String.fromCharCode(b));
  return btoa(binary).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
}

function base64UrlToBuffer(value: string): ArrayBuffer {
  const b64 = value.replace(/-/g, '+').replace(/_/g, '/');
  const padded = b64 + '='.repeat((4 - (b64.length % 4)) % 4);
  const binary = atob(padded);
  const bytes = new Uint8Array(binary.length);
  for (let i = 0; i < binary.length; i++) {
    bytes[i] = binary.charCodeAt(i);
  }
  return bytes.buffer;
}

/** Converte campos base64url das options do servidor em ArrayBuffer (WebAuthn). */
function decodeCreateOptions(options: any): any {
  const o = JSON.parse(JSON.stringify(options));
  o.challenge = base64UrlToBuffer(o.challenge);
  o.user.id = base64UrlToBuffer(o.user.id);
  if (o.excludeCredentials) {
    o.excludeCredentials = o.excludeCredentials.map((c: any) => ({
      ...c, id: base64UrlToBuffer(c.id)
    }));
  }
  return o;
}

function decodeGetOptions(options: any): any {
  const o = JSON.parse(JSON.stringify(options));
  o.challenge = base64UrlToBuffer(o.challenge);
  if (o.allowCredentials) {
    o.allowCredentials = o.allowCredentials.map((c: any) => ({
      ...c, id: base64UrlToBuffer(c.id)
    }));
  }
  return o;
}

function encodeAttestationResponse(credential: any): any {
  return {
    id: credential.id,
    rawId: bufferToBase64Url(credential.rawId),
    type: credential.type,
    response: {
      attestationObject: bufferToBase64Url(credential.response.attestationObject),
      clientDataJSON: bufferToBase64Url(credential.response.clientDataJSON),
    },
  };
}

function encodeAssertionResponse(credential: any): any {
  return {
    id: credential.id,
    rawId: bufferToBase64Url(credential.rawId),
    type: credential.type,
    response: {
      authenticatorData: bufferToBase64Url(credential.response.authenticatorData),
      clientDataJSON: bufferToBase64Url(credential.response.clientDataJSON),
      signature: bufferToBase64Url(credential.response.signature),
      userHandle: credential.response.userHandle
        ? bufferToBase64Url(credential.response.userHandle)
        : null,
    },
  };
}

@Injectable()
export class PasskeyService extends AppComponentBase {
  private readonly baseUrl: string;

  constructor(injector: Injector, private _http: HttpClient) {
    super(injector);
    this.baseUrl = AppConsts.remoteServiceBaseUrl + '/api/Passkey';
  }

  static isSupported(): boolean {
    return typeof (window as any).PublicKeyCredential !== 'undefined'
      && typeof navigator !== 'undefined'
      && !!navigator.credentials;
  }

  getMyPasskeys(): Observable<PasskeyDto[]> {
    return this._http.get<PasskeyDto[]>(`${this.baseUrl}/GetMyPasskeys`);
  }

  deletePasskey(id: number): Observable<void> {
    return this._http.post<void>(`${this.baseUrl}/Delete`, { id });
  }

  /** Fluxo completo: options → navigator.credentials.create → register. */
  async registerPasskey(name: string): Promise<PasskeyDto> {
    const ceremony = await this._http
      .post<PasskeyCeremonyOptionsDto>(`${this.baseUrl}/RegisterOptions`, { name })
      .toPromise();

    const credential = await navigator.credentials.create({
      publicKey: decodeCreateOptions(ceremony.options)
    }) as any;

    return this._http
      .post<PasskeyDto>(`${this.baseUrl}/Register`, {
        ceremonyId: ceremony.ceremonyId,
        name,
        response: encodeAttestationResponse(credential)
      })
      .toPromise();
  }

  /** Login sem senha: options → navigator.credentials.get → authenticate. */
  async authenticatePasskey(userNameOrEmailAddress?: string, tenancyName?: string): Promise<PasskeyAuthenticateResult> {
    const ceremony = await this._http
      .post<PasskeyCeremonyOptionsDto>(`${this.baseUrl}/AssertionOptions`, {
        userNameOrEmailAddress, tenancyName
      })
      .toPromise();

    const credential = await navigator.credentials.get({
      publicKey: decodeGetOptions(ceremony.options)
    }) as any;

    return this._http
      .post<PasskeyAuthenticateResult>(`${this.baseUrl}/AuthenticatePasskey`, {
        ceremonyId: ceremony.ceremonyId,
        response: encodeAssertionResponse(credential)
      })
      .toPromise();
  }
}
