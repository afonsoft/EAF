import { Component, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AuditLogServiceProxy, EntityChangeListDto, EntityPropertyChangeDto } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
  standalone: false,
  selector: 'entityChangeDetailModal',
  templateUrl: './entity-change-detail-modal.component.html',
})
export class EntityChangeDetailModalComponent extends AppComponentBase {
  @ViewChild('entityChangeDetailModal', { static: true })
  modal: ModalDirective;

  active = false;
  entityPropertyChanges: EntityPropertyChangeDto[];
  entityChange: EntityChangeListDto;

  constructor(
    injector: Injector,
    private readonly _auditLogService: AuditLogServiceProxy,
  ) {
    super(injector);
  }

  getPropertyChangeValue(propertyChangeValue, propertyTypeFullName) {
    if (!propertyChangeValue) {
      return propertyChangeValue;
    }
    propertyChangeValue = propertyChangeValue.replace(/^['"]+/, '').replace(/['"]+$/, '');
    if (this.isDate(propertyChangeValue, propertyTypeFullName)) {
      return moment(propertyChangeValue).format('YYYY-MM-DD HH:mm:ss');
    }

    if (propertyChangeValue === 'null') {
      return '';
    }

    return propertyChangeValue;
  }

  isDate(date, propertyTypeFullName): boolean {
    return propertyTypeFullName.includes('DateTime') && !isNaN(Date.parse(date).valueOf());
  }

  isUpdated(): boolean {
    return this.entityChange.changeTypeName == 'Updated';
  }

  show(record: EntityChangeListDto): void {

    this.active = true;
    this.entityChange = record;

    this._auditLogService.getEntityPropertyChanges(record.id).subscribe(result => {
      this.entityPropertyChanges = result;
    });

    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
