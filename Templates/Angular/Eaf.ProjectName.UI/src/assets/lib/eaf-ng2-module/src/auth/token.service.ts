///<reference path="../../../eaf-web-resources/Eaf/Framework/scripts/eaf.d.ts"/>

import { Injectable } from '@angular/core';
import { StorageService } from '@eaf/utils/storage.service';

export interface TokenPayload {
  sub?: string;
  unique_name?: string;
  name?: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'?: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'?: string;
  nameidentifier?: string;
  role?: string | string[];
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string | string[];
  exp?: number;
  tenantid?: string;
}

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  constructor(private readonly storageService: StorageService) {}

  getToken(): string {
    return this.storageService.getCookieValue(eaf.auth.tokenCookieName);
  }

  getTokenCookieName(): string {
    return eaf.auth.tokenCookieName;
  }

  clearToken(): void {
    eaf.auth.clearToken();
    this.storageService.deleteCookie(eaf.auth.tokenCookieName);
  }

  setToken(authToken: string, expireDate?: Date): void {
    this.storageService.setCookieValue(eaf.auth.tokenCookieName, authToken, expireDate, eaf.appPath, eaf.domain);
  }

  /**
   * Decodes and returns the JWT payload without validating the signature.
   */
  getPayload(token?: string): TokenPayload | null {
    const t = token ?? this.getToken();
    if (!t) {
      return null;
    }

    const parts = t.split('.');
    if (parts.length < 2) {
      return null;
    }

    let base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    while (base64.length % 4 !== 0) {
      base64 += '=';
    }

    try {
      const json = atob(base64);
      return JSON.parse(json) as TokenPayload;
    } catch {
      return null;
    }
  }

  /**
   * Returns true when the token can be decoded and, if it has an exp claim, is not expired.
   */
  isValid(): boolean {
    const payload = this.getPayload();
    if (!payload) {
      return false;
    }

    if (payload.exp == null) {
      return true;
    }

    return payload.exp * 1000 > Date.now();
  }

  /**
   * Extracts the user id from the sub or nameidentifier claims.
   */
  getUserId(): number | null {
    const payload = this.getPayload();
    const raw =
      payload?.sub ??
      payload?.nameidentifier ??
      payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];

    if (!raw) {
      return null;
    }

    const parsed = Number(raw);
    return Number.isNaN(parsed) ? null : parsed;
  }

  /**
   * Extracts the tenant id from the tenantid claim.
   */
  getTenantId(): number | null {
    const payload = this.getPayload();
    if (!payload?.tenantid) {
      return null;
    }

    const parsed = Number(payload.tenantid);
    return Number.isNaN(parsed) ? null : parsed;
  }

  /**
   * Extracts the user name from the available name claims.
   */
  getUserName(): string | null {
    const payload = this.getPayload();
    return (
      payload?.unique_name ??
      payload?.name ??
      payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ??
      null
    );
  }

  /**
   * Extracts the roles from the JWT, normalizing a single string to an array.
   */
  getRoles(): string[] {
    const payload = this.getPayload();
    const role =
      payload?.role ??
      payload?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    if (!role) {
      return [];
    }

    return Array.isArray(role) ? role : [role];
  }

  /**
   * Checks whether the user has the given role (case-insensitive).
   */
  isInRole(role: string): boolean {
    if (!role) {
      return false;
    }

    const target = role.toLowerCase();
    return this.getRoles().some(r => r.toLowerCase() === target);
  }
}