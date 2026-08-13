import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AuditLogListDto } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';

@Component({
  standalone: false,
  selector: 'auditLogDetailModal',
  templateUrl: './audit-log-detail-modal.component.html',
})
export class AuditLogDetailModalComponent extends AppComponentBase {
  active = false;
  visible = false;
  auditLog: AuditLogListDto;

  constructor(injector: Injector) {
    super(injector);
  }

  getExecutionTime(): string {
    return moment(this.auditLog.executionTime).fromNow() + ' (' + moment(this.auditLog.executionTime).format('YYYY-MM-DD HH:mm:ss') + ')';
  }

  getDurationAsMs(): string {
    return this.l('Xms', this.auditLog.executionDuration);
  }

  getFormattedParameters(): string {
    try {
      const json = JSON.parse(this.auditLog.parameters);
      return JSON.stringify(json, null, 4);
    } catch (e) {
      eaf.log.warn(e);
      return this.auditLog.parameters;
    }
  }

  getFormattedCustomData(): string {
    if (!this.auditLog.customData) {
      return '';
    }
    try {
      const json = JSON.parse(this.auditLog.customData);
      return JSON.stringify(json, null, 4);
    } catch (e) {
      return this.auditLog.customData;
    }
  }

  getFormattedException(): string {
    return this.auditLog.exception || '';
  }

  show(record: AuditLogListDto): void {
    this.active = true;
    this.auditLog = record;
    this.visible = true;
  }

  close(): void {
    this.active = false;
    this.visible = false;
  }

  copyToClipboard(text: string): void {
    if (!text) {
      return;
    }
    navigator.clipboard.writeText(text).then(
      () => this.notify.success(this.l('CopiedToClipboard')),
      () => this.notify.error(this.l('CopyToClipboardFailed'))
    );
  }
}
