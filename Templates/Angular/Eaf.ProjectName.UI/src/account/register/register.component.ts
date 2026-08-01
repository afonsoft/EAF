import { Component, Injector, OnInit } from '@angular/core';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AccountServiceProxy, TenantListDto } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';
import { RegisterModel } from './register.model';
import { RegisterService } from './register.service';

@Component({
  standalone: false,
  templateUrl: './register.component.html',
  animations: [accountModuleAnimation()],
})
export class RegisterComponent extends AppComponentBase implements OnInit {
  model = new RegisterModel();
  submitting = false;
  tenants: TenantListDto[] = [];

  constructor(
    injector: Injector,
    private readonly _registerService: RegisterService,
    private readonly _accountService: AccountServiceProxy,
    private readonly _router: Router,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.clearSession();
    if (this.multiTenancy.isEnabled) {
      this._accountService.getAllTenants().subscribe(result => {
        this.tenants = result;
      });
    }
  }

  clearSession() {
    eaf.utils.deleteCookie(AppConsts.authorization.encrptedAuthTokenName, eaf.appPath);
    eaf.utils.deleteCookie(eaf.auth.tokenCookieName, eaf.appPath);
    eaf.utils.deleteCookie(eaf.multiTenancy.tenantIdCookieName, eaf.appPath);
    eaf.auth.clearToken();
  }

  register(): void {
    if (this.model.isCreatingTenant) {
      this.model.tenantId = undefined;
    } else {
      this.model.tenancyName = '';
      this.model.tenantName = '';
    }

    this.submitting = true;
    this._registerService.register(this.model).subscribe({
      next: result => {
        this.submitting = false;
        if (result.canLogin) {
          this.message.success(this.l('SuccessfullyRegistered'));
          this._router.navigate(['/account/login']);
        } else {
          this.message.info(this.l('RegistrationWaitingForApproval'));
        }
      },
      error: () => {
        this.submitting = false;
      },
    });
  }

  toggleCreateTenant(): void {
    this.model.isCreatingTenant = !this.model.isCreatingTenant;
    this.model.tenantId = undefined;
  }
}
