import './polyfills';
import 'moment-timezone';
import 'moment/min/locales.min';

import { enableProdMode, provideZoneChangeDetection } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { environment } from './environments/environment';
import { RootModule } from './root.module';

if (environment.production) {
  enableProdMode();
}

const bootstrap = () => {
  return platformBrowserDynamic().bootstrapModule(RootModule, { applicationProviders: [provideZoneChangeDetection()] });
};

bootstrap(); //Regular bootstrap
