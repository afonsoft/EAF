import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root',
})
export class StorageService {
  constructor(private readonly cookieService: CookieService) {}

  public setValue(key: string, value: any): void {
    this.cookieService.set(key, JSON.stringify(value), new Date(Date.now() + 86400000), '/', '', true, 'Lax');
  }

  public getValue(key: string): any {
    const value = this.cookieService.get(key);
    if (value === '' || value === null || value === undefined) return null;
    try {
      return JSON.parse(value);
    } catch {
      return value;
    }
  }

  public removeValue(key: string): void {
    this.cookieService.delete(key);
  }

  public getCookieValue(key: string): string {
    return this.cookieService.get(key);
  }

  public setCookieValue(key: string, value: string, expireDate?: Date, path?: string, domain?: string): void {
    this.cookieService.set(key, value, expireDate, path, domain, true, 'Lax');
  }

  public deleteCookie(key: string, path?: string): void {
    this.cookieService.delete(key, path);
  }

  public Clear() {
    this.cookieService.deleteAll();
  }
}
