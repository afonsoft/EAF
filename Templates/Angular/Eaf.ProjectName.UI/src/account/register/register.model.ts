export enum TenantSelectionMode {
  DefaultTenant = 'DefaultTenant',
  CreateNew = 'CreateNew',
  JoinExisting = 'JoinExisting',
}

export class RegisterModel {
  tenantSelectionMode: TenantSelectionMode = TenantSelectionMode.DefaultTenant;
  tenancyName: string;
  tenantName: string;
  existingTenantId: number | undefined;
  joinRequestMessage: string;
  name: string;
  surname: string;
  userName: string;
  emailAddress: string;
  password: string;

  get isDefaultTenant(): boolean {
    return this.tenantSelectionMode === TenantSelectionMode.DefaultTenant;
  }

  get isCreatingTenant(): boolean {
    return this.tenantSelectionMode === TenantSelectionMode.CreateNew;
  }

  get isJoiningTenant(): boolean {
    return this.tenantSelectionMode === TenantSelectionMode.JoinExisting;
  }
}

export class RegisterResult {
  canLogin: boolean;
  tenantId: number | undefined;
  tenancyName: string;
}
