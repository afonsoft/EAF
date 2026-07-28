import { SessionServiceProxy, AuthenticateResultModel } from '@shared/service-proxies/service-proxies';
import { FormsModule } from '@angular/forms';
import { TestBed, ComponentFixture, fakeAsync, tick } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LoginComponent } from './login.component';
import { LoginService, AvailableTenantResult } from './login.service';
import { ReCaptchaV3Service } from 'ngx-captcha';
import { AppConsts } from '@shared/AppConsts';
import { of } from 'rxjs';
import { LocalizationService } from '@eaf/localization/localization.service';
import { PermissionCheckerService } from '@eaf/auth/permission-checker.service';
import { FeatureCheckerService } from '@eaf/features/feature-checker.service';
import { MessageService } from '@eaf/message/message.service';
import { NotifyService } from '@eaf/notify/notify.service';
import { SettingService } from '@eaf/settings/setting.service';
import { EafMultiTenancyService } from '@eaf/multi-tenancy/eaf-multi-tenancy.service';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { AppUiCustomizationService } from '@shared/common/ui/app-ui-customization.service';
import { AppUrlService } from '@shared/common/nav/app-url.service';
import { AccountServiceProxy } from '@shared/service-proxies/service-proxies';
import {
  MockLocalizationService,
  MockPermissionCheckerService,
  MockFeatureCheckerService,
  MockMessageService,
  MockNotifyService,
  MockSettingService,
  MockEafMultiTenancyService,
  MockAppSessionService,
  MockAppUiCustomizationService,
  MockAppUrlService,
  MockAccountServiceProxy,
  MockActivatedRoute,
  MockSessionServiceProxy,
  setupEafGlobals,
  MockLocalizePipe,
} from '../../test-helpers/mock-services';

class MockLoginService {
  authenticateModel = { userNameOrEmailAddress: 'test', password: 'password', rememberClient: false };
  authenticateResult = null;
  rememberMe = false;
  availableTenantsResult = [];
  externalLoginProviders = [];

  authenticate(callback?: () => void): void {
    if (callback) callback();
  }
  init(callback?: () => void): void {
    if (callback) callback();
  }
  availableTenants(): any {
    return of([]);
  }
  selectTenant(): any {
    return of({});
  }
  loginTenant(): void {}
  navigateToSelectTenant(): void {}
}

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  const mockRouter = { navigate: jasmine.createSpy('navigate') };

  beforeEach(() => {
    setupEafGlobals();
    TestBed.configureTestingModule({
      declarations: [LoginComponent, MockLocalizePipe],
      imports: [FormsModule],
      providers: [
        { provide: SessionServiceProxy, useClass: MockSessionServiceProxy },
        { provide: LoginService, useClass: MockLoginService },
        { provide: ReCaptchaV3Service, useValue: { execute: () => {} } },
        { provide: ActivatedRoute, useClass: MockActivatedRoute },
        { provide: Router, useValue: mockRouter },
        { provide: AccountServiceProxy, useClass: MockAccountServiceProxy },
        { provide: LocalizationService, useClass: MockLocalizationService },
        { provide: PermissionCheckerService, useClass: MockPermissionCheckerService },
        { provide: FeatureCheckerService, useClass: MockFeatureCheckerService },
        { provide: MessageService, useClass: MockMessageService },
        { provide: NotifyService, useClass: MockNotifyService },
        { provide: SettingService, useClass: MockSettingService },
        { provide: EafMultiTenancyService, useClass: MockEafMultiTenancyService },
        { provide: AppSessionService, useClass: MockAppSessionService },
        { provide: AppUiCustomizationService, useClass: MockAppUiCustomizationService },
        { provide: AppUrlService, useClass: MockAppUrlService },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have submitting set to false initially', () => {
    expect(component.submitting).toBeFalsy();
  });

  it('deve chamar authenticate quando nenhum tenant disponível no twoStepLogin', fakeAsync(() => {
    AppConsts.multiTenancy.twoStepLogin = true;
    spyOn(component['loginService'], 'availableTenants').and.returnValue(of([]));
    spyOn(component['loginService'], 'authenticate').and.callThrough();

    component.login();
    tick();

    expect(component['loginService'].authenticate).toHaveBeenCalled();
  }));

  it('deve chamar selectTenant quando houver um único tenant no twoStepLogin', fakeAsync(() => {
    AppConsts.multiTenancy.twoStepLogin = true;
    const tenant = { tenantId: 1, tenantName: 'Tenant', tenancyName: 'tenant', isDefault: true };
    spyOn(component['loginService'], 'availableTenants').and.returnValue(of([tenant] as AvailableTenantResult[]));
    spyOn(component['loginService'], 'selectTenant').and.returnValue(of({ accessToken: 'token' } as AuthenticateResultModel));

    component.login();
    tick();

    expect(component['loginService'].selectTenant).toHaveBeenCalled();
  }));

  it('deve chamar navigateToSelectTenant quando houver múltiplos tenants no twoStepLogin', fakeAsync(() => {
    AppConsts.multiTenancy.twoStepLogin = true;
    const tenants: AvailableTenantResult[] = [
      { tenantId: 1, tenantName: 'Tenant 1', tenancyName: 'tenant1', isDefault: false },
      { tenantId: 2, tenantName: 'Tenant 2', tenancyName: 'tenant2', isDefault: false },
    ];
    spyOn(component['loginService'], 'availableTenants').and.returnValue(of(tenants));
    spyOn(component['loginService'], 'navigateToSelectTenant');

    component.login();
    tick();

    expect(component['loginService'].navigateToSelectTenant).toHaveBeenCalled();
  }));
});
