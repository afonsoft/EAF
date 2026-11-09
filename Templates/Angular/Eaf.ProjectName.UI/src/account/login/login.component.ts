import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { EafSessionService } from '@eaf/session/eaf-session.service';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  AccountServiceProxy,
  SessionServiceProxy,
  TenantListDto,
  UpdateUserSignInTokenOutput,
} from '@shared/service-proxies/service-proxies';
import { ReCaptchaV3Service } from 'ngx-captcha';
import { UrlHelper } from 'shared/helpers/UrlHelper';

import { PasskeyService } from '@shared/service-proxies/passkey.service';
import { QrLoginService } from '@shared/service-proxies/qr-login.service';
import * as QRCode from 'qrcode';
import { ExternalLoginProvider, LoginService } from './login.service';
@Component({
  standalone: false,
  templateUrl: './login.component.html',
  animations: [accountModuleAnimation()],
})
export class LoginComponent extends AppComponentBase implements OnInit, OnDestroy {
  submitting = false;
  isMultiTenancyEnabled: boolean = this.multiTenancy.isEnabled;
  recaptchaSiteKey: string = AppConsts.recaptchaSiteKey;
  isSocialLogin = true;

  tenants: TenantListDto[] = [];
  selectedTenant: TenantListDto;

  get isTwoStepLogin(): boolean {
    return AppConsts.multiTenancy?.twoStepLogin ?? false;
  }

  constructor(
    injector: Injector,
    public loginService: LoginService,
    public passkeyService: PasskeyService,
    public qrLoginService: QrLoginService,
    private readonly _sessionService: EafSessionService,
    private readonly _accountService: AccountServiceProxy,
    private readonly _sessionAppService: SessionServiceProxy,
    private readonly _reCaptchaV3Service: ReCaptchaV3Service,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.dataTableHelper.showLoadingIndicator();
    this.submitting = true;
    this.clearSession();
    if (this.isMultiTenancyEnabled) {
      eaf.multiTenancy.setTenantIdCookie(null);

      this._accountService.getAllTenants().subscribe(result => {
        this.tenants = result;
        this.dataTableHelper.hideLoadingIndicator();
        eaf.ui.clearBusy();
      });
    } else {
      this.dataTableHelper.hideLoadingIndicator();
      eaf.ui.clearBusy();
    }

    if (this._sessionService.userId > 0 && UrlHelper.getReturnUrl() && UrlHelper.getSingleSignIn()) {
      this._sessionAppService.updateUserSignInToken().subscribe((result: UpdateUserSignInTokenOutput) => {
        const initialReturnUrl = UrlHelper.getReturnUrl();
        const returnUrl =
          initialReturnUrl +
          (initialReturnUrl.includes('?') ? '&' : '?') +
          'accessToken=' +
          result.signInToken +
          '&userId=' +
          result.encodedUserId +
          '&tenantId=' +
          result.encodedTenantId;

        location.href = returnUrl;
      });
    }
    this.qrLoginService.isEnabled().subscribe(enabled => {
      this.qrLoginEnabled = enabled;
    });
    this.handleExternalLoginCallbacks();
    this.loginService.init();
    this.submitting = false;
    this.dataTableHelper.hideLoadingIndicator();
  }
  clearSession() {
    eaf.utils.deleteCookie(AppConsts.authorization.encrptedAuthTokenName, eaf.appPath);
    eaf.utils.deleteCookie(eaf.auth.tokenCookieName, eaf.appPath);
    eaf.utils.deleteCookie(eaf.multiTenancy.tenantIdCookieName, eaf.appPath);
    eaf.auth.clearToken();
  }

  ChangeLogin(): void {
    this.isSocialLogin = !this.isSocialLogin;
  }

  handleExternalLoginCallbacks(): void {
    const state = UrlHelper.getQueryParametersUsingHash().state;
    const parameters = UrlHelper.getQueryParameters();
    this.submitting = true;
    if (state?.includes('openIdConnect') || parameters['openIdConnect'] !== undefined) {
      this.loginService.openIdConnectLoginCallback();
    }
  }

  login(): void {
    if (this.isTwoStepLogin) {
      this.twoStepLogin();
      return;
    }

    this.normalLogin();
  }

  private normalLogin(): void {
    const recaptchaCallback = (token: string) => {
      this.submitting = true;
      this.loginService.authenticate(
        () => {
          this.submitting = false;
          this.dataTableHelper.hideLoadingIndicator();
        },
        null,
        token,
      );
    };

    if (this.useCaptcha) {
      this._reCaptchaV3Service.execute(this.recaptchaSiteKey, 'login', token => {
        recaptchaCallback(token);
      });
    } else {
      recaptchaCallback(null);
    }
  }

  private twoStepLogin(): void {
    const recaptchaCallback = (token: string) => {
      this.submitting = true;
      this.dataTableHelper.showLoadingIndicator();

      const model = {
        userNameOrEmailAddress: this.loginService.authenticateModel.userNameOrEmailAddress,
        password: this.loginService.authenticateModel.password,
      };

      this.loginService
        .availableTenants(model)
        .subscribe({
          next: tenants => {
            this.loginService.availableTenantsResult = tenants;

            if (tenants.length === 0) {
              // Host user with no associated tenants: fall back to legacy TokenAuth/Authenticate
              this.loginService.authenticate(
                () => {
                  this.submitting = false;
                  this.dataTableHelper.hideLoadingIndicator();
                },
                undefined,
                token,
              );
            } else if (tenants.length === 1 && AppConsts.autoSelectSingleTenant) {
              this.loginService
                .selectTenant({
                  ...model,
                  tenantId: tenants[0].tenantId,
                })
                .subscribe({
                  next: result => this.loginService.loginTenant(result, tenants[0].tenantId),
                  error: () => {
                    this.submitting = false;
                    this.dataTableHelper.hideLoadingIndicator();
                  },
                });
            } else {
              this.submitting = false;
              this.dataTableHelper.hideLoadingIndicator();
              this.loginService.navigateToSelectTenant();
            }
          },
          error: () => {
            this.submitting = false;
            this.dataTableHelper.hideLoadingIndicator();
          },
        });
    };

    if (this.useCaptcha) {
      this._reCaptchaV3Service.execute(this.recaptchaSiteKey, 'login', token => {
        recaptchaCallback(token);
      });
    } else {
      recaptchaCallback(null);
    }
  }

  selectTenant(tenant: TenantListDto): void {
    this.selectedTenant = tenant;
    eaf.multiTenancy.setTenantIdCookie(this.selectedTenant.id);
  }

  selectHost(): void {
    this.selectedTenant = undefined;
    eaf.multiTenancy.setTenantIdCookie(null);
  }

  truncateStringWithPostfix(name: string): string {
    return eaf.utils.truncateStringWithPostfix(name, 30);
  }

  get useCaptcha(): boolean {
    return this.setting.getBoolean('App.UserManagement.UseCaptchaOnLogin');
  }

  externalLogin(provider: ExternalLoginProvider) {
    this.dataTableHelper.showLoadingIndicator();
    this.submitting = true;
    this.loginService.externalAuthenticate(provider);
  }


get passkeySupported(): boolean {
    return PasskeyService.isSupported();
  }

  async loginWithPasskey(): Promise<void> {
    this.submitting = true;
    try {
      const result = await this.passkeyService.authenticatePasskey(
        this.loginService.authenticateModel.userNameOrEmailAddress,
        this.loginService.authenticateModel.tenancyName,
      );
      this.loginService.login(
        result.accessToken,
        result.encryptedAccessToken,
        result.expireInSeconds,
        this.loginService.rememberMe,
        undefined,
      );
    } catch (e: any) {
      if (e && e.name !== 'NotAllowedError') {
        this.message.error(this.l('PasskeyLoginFailed'));
      }
    } finally {
      this.submitting = false;
    }
  }

  qrLoginEnabled = false;
  showQrLogin = false;
  qrCodeDataUrl: string | undefined;
  qrExpired = false;
  private qrTokenId: string | undefined;
  private qrDeadline = 0;
  private qrPollTimer: any;

  ngOnDestroy(): void {
    this.stopQrPolling();
  }

  startQrLogin(): void {
    this.showQrLogin = true;
    this.generateQrToken();
  }

  cancelQrLogin(): void {
    this.showQrLogin = false;
    this.stopQrPolling();
  }

  generateQrToken(): void {
    this.stopQrPolling();
    this.qrExpired = false;
    this.qrCodeDataUrl = undefined;
    this.qrLoginService.create().subscribe(result => {
      this.qrTokenId = result.tokenId;
      this.qrDeadline = Date.now() + result.expiresInSeconds * 1000;
      QRCode.toDataURL(result.tokenId, { width: 220, margin: 1 }).then(url => {
        this.qrCodeDataUrl = url;
      });
      this.qrPollTimer = setInterval(() => this.pollQrStatus(), 3000);
    });
  }

  private pollQrStatus(): void {
    if (!this.qrTokenId) {
      return;
    }
    if (Date.now() >= this.qrDeadline) {
      this.qrExpired = true;
      this.stopQrPolling();
      return;
    }
    this.qrLoginService.status(this.qrTokenId).subscribe(result => {
      if (result.status === 'approved' && result.accessToken) {
        this.stopQrPolling();
        this.loginService.login(
          result.accessToken,
          result.encryptedAccessToken,
          result.expireInSeconds ?? 0,
          this.loginService.rememberMe,
          undefined,
        );
      } else if (result.status === 'expired') {
        this.qrExpired = true;
        this.stopQrPolling();
      }
    });
  }

  private stopQrPolling(): void {
    if (this.qrPollTimer) {
      clearInterval(this.qrPollTimer);
      this.qrPollTimer = undefined;
    }
  }
}
