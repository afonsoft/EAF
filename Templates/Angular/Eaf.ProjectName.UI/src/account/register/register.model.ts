export class RegisterModel {
  tenancyName: string;
  tenantName: string;
  tenantId: number | undefined;
  name: string;
  surname: string;
  userName: string;
  emailAddress: string;
  password: string;
  isCreatingTenant = false;

  get hasTenancyName(): boolean {
    return !!this.tenancyName;
  }

  get hasTenantId(): boolean {
    return !!this.tenantId;
  }
}

export class RegisterResult {
  canLogin: boolean;
}
