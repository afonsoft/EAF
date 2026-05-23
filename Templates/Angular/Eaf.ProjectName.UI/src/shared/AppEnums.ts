import { UserNotificationState } from '@shared/service-proxies/service-proxies';

export class AppTimezoneScope {
  static Application = 1;
  static Tenant = 2;
  static User = 4;
  static All = 7;
}

export class AppUserNotificationState {
  static Unread: number = UserNotificationState.Unread;
  static Read: number = UserNotificationState.Read;
}
