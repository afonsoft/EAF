import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

import * as _ from 'lodash';

@Component({
  standalone: false,
  selector: 'titlebar',
  template: `<h3 class="m_header_tilebar header-{{ currentTheme.baseSettings.header.headerSkin }}">ProjectName</h3>`,
})
export class TitleBarComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
