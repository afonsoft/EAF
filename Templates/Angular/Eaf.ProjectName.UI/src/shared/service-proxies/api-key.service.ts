import { Injectable, Injector } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';

export class ApiKeyDto {
  id: number;
  name: string;
  keyPrefix: string;
  permissionScopes: string[] | undefined;
  allowedIpRanges: string[] | undefined;
  expirationTime: string | undefined;
  status: ApiKeyStatus;
  lastUsedTime: string | undefined;
  creationTime: string;
  userId: number;
  userName: string | undefined;
}

export enum ApiKeyStatus {
  Active = 0,
  Revoked = 1,
}

export class CreateApiKeyInput {
  name: string;
  permissionScopes: string[] | undefined;
  allowedIpRanges: string[] | undefined;
  expirationTime: string | undefined;
}

export class CreatedApiKeyDto {
  apiKey: ApiKeyDto;
  plainKey: string;
}

export class ListResultOfApiKeyDto {
  items: ApiKeyDto[];
}

/** Chaves do usuário autenticado ("My API Keys"). */
@Injectable()
export class ApiKeyService extends AppComponentBase {
  private readonly _baseUrl = `${AppConsts.remoteServiceBaseUrl}/api/services/app/ApiKey`;

  constructor(
    injector: Injector,
    private readonly _httpClient: HttpClient,
  ) {
    super(injector);
  }

  getMyApiKeys(): Observable<ApiKeyDto[]> {
    return this._httpClient
      .get<ListResultOfApiKeyDto>(`${this._baseUrl}/GetMyApiKeys`)
      .pipe(map(r => r.items ?? []));
  }

  createApiKey(input: CreateApiKeyInput): Observable<CreatedApiKeyDto> {
    return this._httpClient.post<CreatedApiKeyDto>(`${this._baseUrl}/CreateApiKey`, input);
  }

  revokeApiKey(id: number): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/RevokeApiKey`, { id });
  }

  rotateApiKey(id: number): Observable<CreatedApiKeyDto> {
    return this._httpClient.post<CreatedApiKeyDto>(`${this._baseUrl}/RotateApiKey`, { id });
  }
}

/** Administração de todas as API keys do tenant ("Administration > API Keys"). */
@Injectable()
export class AdminApiKeyService extends AppComponentBase {
  private readonly _baseUrl = `${AppConsts.remoteServiceBaseUrl}/api/services/app/AdminApiKey`;

  constructor(
    injector: Injector,
    private readonly _httpClient: HttpClient,
  ) {
    super(injector);
  }

  getTenantApiKeys(): Observable<ApiKeyDto[]> {
    return this._httpClient
      .get<ListResultOfApiKeyDto>(`${this._baseUrl}/GetTenantApiKeys`)
      .pipe(map(r => r.items ?? []));
  }

  revokeApiKey(id: number): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/RevokeApiKey`, { id });
  }
}
