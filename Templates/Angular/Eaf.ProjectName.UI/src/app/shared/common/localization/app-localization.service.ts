import { LocalizationService } from '@eaf/localization/localization.service';
import { Injectable } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';

@Injectable()
export class AppLocalizationService extends LocalizationService {
  l(key: string, ...args: any[]): string {
    args.unshift(key);
    args.unshift(AppConsts.localization.defaultLocalizationSourceName);
    return this.ls.apply(this, args);
  }

  ls(sourcename: string, key: string, ...args: any[]): string {
    let localizedText = this.localize(key, AppConsts.localization.defaultLocalizationSourceName);

    if (!localizedText || localizedText == key) localizedText = this.localize(key, AppConsts.localization.defaultLocalizationSourceNameEaf);
    if (!localizedText || localizedText == key) localizedText = this.localize(key, AppConsts.localization.defaultLocalizationSourceNameAbp);
    if (!localizedText || localizedText == key) localizedText = this.localize(key, AppConsts.localization.defaultLocalizationSourceNameAbpWeb);
    if (!localizedText || localizedText == key) localizedText = this.localize(key, AppConsts.localization.defaultLocalizationSourceNameAbpZero);
    if (!localizedText || localizedText == key)
      localizedText = this.localize(key, AppConsts.localization.defaultLocalizationSourceNameEafAzureActiveDirectory);
    if (!localizedText || localizedText == key) localizedText = this.localize(key, AppConsts.localization.defaultLocalizationSourceNameEafLdap);
    if (!localizedText || localizedText == key) localizedText = this.localize(key, sourcename);


    args.unshift(localizedText);

    return eaf.utils.formatString.apply(this, args);
  }
}
