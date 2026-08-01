import { Component, Injector, OnInit } from '@angular/core';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';
import { RegisterModel, RegisterResult, TenantSelectionMode } from './register.model';
import { RegisterService } from './register.service';
import { TenantJoinRequestService, AvailableTenantDto } from '@shared/service-proxies/tenant-join-request.service';

@Component({
  standalone: false,
  selector: 'app-register',
  templateUrl: './register.component.html',
  animations: [accountModuleAnimation()],
})
export class RegisterComponent extends AppComponentBase implements OnInit {
  model = new RegisterModel();
  submitting = false;
  tenants: AvailableTenantDto[] = [];
  tenantSelectionMode = TenantSelectionMode;

  constructor(
    injector: Injector,
    private readonly _registerService: RegisterService,
    private readonly _tenantJoinRequestService: TenantJoinRequestService,
    private readonly _router: Router,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.clearSession();
    if (this.multiTenancy.isEnabled) {
      this._tenantJoinRequestService.getAvailableTenants().subscribe(result => {
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
    this.submitting = true;
    this._registerService.register(this.model).subscribe({
      next: (result: RegisterResult) => {
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

  setTenantSelectionMode(mode: TenantSelectionMode): void {
    this.model.tenantSelectionMode = mode;
  }
}
