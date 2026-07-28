import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { LoginService, AvailableTenantResult } from './login.service';
import { AppConsts } from '@shared/AppConsts';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { TokenService } from '@eaf/auth/token.service';
import { StorageService } from '@eaf/utils/storage.service';
import { LocalizationService } from '@eaf/localization/localization.service';
import { LogService } from '@eaf/log/log.service';
import { OAuthService } from 'angular-oauth2-oidc';
import { setupEafGlobals } from '../../test-helpers/mock-services';

describe('LoginService', () => {
  let service: LoginService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    setupEafGlobals();
    AppConsts.remoteServiceBaseUrl = 'http://localhost:8001';

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        LoginService,
        { provide: TokenAuthServiceProxy, useValue: {} },
        { provide: TokenService, useValue: { getToken: () => '', setToken: () => {}, clearToken: () => {} } },
        { provide: StorageService, useValue: { getCookieValue: () => '', setCookieValue: () => {}, deleteCookie: () => {} } },
        { provide: LocalizationService, useValue: { localize: (key: string) => key } },
        { provide: LogService, useValue: {} },
        { provide: OAuthService, useValue: {} },
      ],
    });

    service = TestBed.inject(LoginService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('deve chamar GetAvailableTenants via POST', () => {
    const model = { userNameOrEmailAddress: 'test', password: 'password' };
    const expected: AvailableTenantResult[] = [
      { tenantId: 1, tenantName: 'Tenant', tenancyName: 'tenant', isDefault: true },
    ];

    service.availableTenants(model).subscribe(result => {
      expect(result).toEqual(expected);
    });

    const req = httpMock.expectOne('http://localhost:8001/api/TokenAuth/GetAvailableTenants');
    expect(req.request.method).toBe('POST');
    req.flush(expected);
  });

  it('deve chamar SelectTenant via POST', () => {
    const model = { userNameOrEmailAddress: 'test', password: 'password', tenantId: 1 };
    const expected = { accessToken: 'token', encryptedAccessToken: 'enc', expireInSeconds: 3600 };

    service.selectTenant(model).subscribe(result => {
      expect(result.accessToken).toBe('token');
    });

    const req = httpMock.expectOne('http://localhost:8001/api/TokenAuth/SelectTenant');
    expect(req.request.method).toBe('POST');
    req.flush(expected);
  });
});
