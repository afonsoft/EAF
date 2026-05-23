import { Component, Injector, OnInit } from '@angular/core';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TenantListDto } from '@shared/service-proxies/service-proxies';
import { UrlHelper } from 'shared/helpers/UrlHelper';
import { LoginService } from './login.service';
import { Router } from '@angular/router';

@Component({
  standalone: false,
  templateUrl: './sso.component.html',
  animations: [accountModuleAnimation()],
})
export class SsoComponent extends AppComponentBase implements OnInit {
  submitting = false;
  isMultiTenancyEnabled: boolean = this.multiTenancy.isEnabled;

  tenants: TenantListDto[] = [];
  selectedTenant: TenantListDto;

  constructor(
    injector: Injector,
    public loginService: LoginService,
    private _router: Router,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    const state = UrlHelper.getQueryParametersUsingHash().state;
    const parameters = UrlHelper.getQueryParameters();
    this.submitting = true;
    if ((state && state.indexOf('openIdConnect') >= 0) || parameters['openIdConnect'] !== undefined) {
      this.loginService.openIdConnectLoginCallback();
    } else if (state && state.indexOf('state') >= 0 && state.indexOf('code') >= 0) {
      this.loginService.SSO_AuthZero_Callback();
    } else if (state) {
      this.loginService.SSO_Microsoft_Callback();
    } else {
      this._router.navigate(['/account/login']);
    }
  }
}
