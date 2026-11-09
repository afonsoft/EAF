import { Component, Injector, OnInit } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EmailTemplateService, EmailTemplateListItemDto, EmailTemplateForEditDto } from '@shared/service-proxies/email-template.service';
import { appModuleAnimation } from '@shared/animations/routerTransition';

class TemplateGroup {
  name: string;
  displayName: string;
  rows: EmailTemplateListItemDto[];
  newCulture = '';
}

@Component({
  selector: 'app-email-templates',
  templateUrl: './email-templates.component.html',
  animations: [appModuleAnimation()],
  standalone: false,
})
export class EmailTemplatesComponent extends AppComponentBase implements OnInit {
  groups: TemplateGroup[] = [];
  loading = false;
  saving = false;

  edit: EmailTemplateForEditDto | undefined;
  editBody = '';
  previewHtml: SafeHtml | undefined;

  constructor(
    injector: Injector,
    private readonly _service: EmailTemplateService,
    private readonly _sanitizer: DomSanitizer,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this._service.getTemplates().pipe(finalize(() => (this.loading = false))).subscribe(items => {
      const names = [...new Set(items.map(i => i.name))];
      this.groups = names.map(name => {
        const rows = items.filter(i => i.name === name);
        return {
          name,
          displayName: this.l(rows[0]?.displayNameLocalizationKey || name),
          rows,
          newCulture: '',
        };
      });
    });
  }

  openEdit(name: string, culture: string): void {
    this.previewHtml = undefined;
    this._service.getForEdit(name, culture).subscribe(dto => {
      this.edit = dto;
      this.editBody = dto.body;
    });
  }

  openEditForNewCulture(group: TemplateGroup): void {
    const culture = (group.newCulture || '').trim();
    if (!culture) {
      return;
    }
    this.openEdit(group.name, culture);
  }

  preview(): void {
    if (!this.edit) {
      return;
    }
    this._service.getPreview(this.edit.name, this.edit.culture, this.editBody).subscribe(dto => {
      this.previewHtml = this._sanitizer.bypassSecurityTrustHtml(dto.html);
    });
  }

  save(): void {
    if (!this.edit) {
      return;
    }
    this.saving = true;
    this._service
      .createOrUpdateOverride(this.edit.name, this.edit.culture, this.editBody)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: () => {
          this.notify.success(this.l('SavedSuccessfully'));
          this.closeEdit();
          this.load();
        },
        error: () => {},
      });
  }

  reset(item: EmailTemplateListItemDto): void {
    this.message.confirm(this.l('EmailTemplateResetWarningMessage'), isConfirmed => {
      if (!isConfirmed) {
        return;
      }
      this._service.resetToDefault(item.name, item.culture).subscribe(() => {
        this.notify.success(this.l('SuccessfullyDeleted'));
        this.load();
      });
    });
  }

  closeEdit(): void {
    this.edit = undefined;
    this.previewHtml = undefined;
  }
}
