import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppConsts } from '@shared/AppConsts';
import { RegisterModel, RegisterResult } from './register.model';

@Injectable()
export class RegisterService {
  private readonly _baseUrl = `${AppConsts.remoteServiceBaseUrl}/api/services/app/Account`;

  constructor(private readonly _httpClient: HttpClient) {}

  register(input: RegisterModel): Observable<RegisterResult> {
    return this._httpClient.post<RegisterResult>(`${this._baseUrl}/Register`, input);
  }
}
