import { Injectable, Injector } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';

export class AvailableTenantDto {
  tenantId: number;
  tenantName: string | undefined;
  tenancyName: string | undefined;
  isDefault: boolean;
}

export class TenantJoinRequestDto {
  id: number;
  userId: number;
  tenantId: number;
  tenantUserId: number;
  status: TenantJoinRequestStatus;
  message: string | undefined;
  approverUserId: number | undefined;
  userName: string | undefined;
  tenantName: string | undefined;
  creationTime: string;
}

export enum TenantJoinRequestStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
}

export class CreateTenantJoinRequestInput {
  tenantId: number;
  message: string | undefined;
}

export class ApproveTenantJoinRequestInput {
  requestId: number;
  isApproved: boolean;
}

@Injectable()
export class TenantJoinRequestService extends AppComponentBase {
  private readonly _baseUrl = `${AppConsts.remoteServiceBaseUrl}/api/services/app/TenantJoinRequest`;

  constructor(
    injector: Injector,
    private readonly _httpClient: HttpClient,
  ) {
    super(injector);
  }

  getAvailableTenants(): Observable<AvailableTenantDto[]> {
    return this._httpClient.get<AvailableTenantDto[]>(`${this._baseUrl}/GetAvailableTenants`);
  }

  getMyRequests(): Observable<TenantJoinRequestDto[]> {
    return this._httpClient.get<TenantJoinRequestDto[]>(`${this._baseUrl}/GetMyRequests`);
  }

  getPendingRequestsForCurrentTenant(): Observable<TenantJoinRequestDto[]> {
    return this._httpClient.get<TenantJoinRequestDto[]>(`${this._baseUrl}/GetPendingRequestsForCurrentTenant`);
  }

  createRequest(input: CreateTenantJoinRequestInput): Observable<TenantJoinRequestDto> {
    return this._httpClient.post<TenantJoinRequestDto>(`${this._baseUrl}/CreateRequest`, input);
  }

  approve(input: ApproveTenantJoinRequestInput): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/Approve`, input);
  }
}
