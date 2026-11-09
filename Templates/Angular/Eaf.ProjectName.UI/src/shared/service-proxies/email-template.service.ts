import { Injectable, Injector } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';

export class EmailTemplateListItemDto {
  name: string;
  displayNameLocalizationKey: string;
  culture: string;
  isOverride: boolean;
  lastModificationTime: string | undefined;
}

export class ListResultOfEmailTemplateListItemDto {
  items: EmailTemplateListItemDto[];
}

export class EmailTemplateForEditDto {
  name: string;
  culture: string;
  body: string;
  placeholders: string[];
  requiredPlaceholders: string[];
  isOverride: boolean;
}

export class EmailTemplatePreviewDto {
  html: string;
}

/** Administração de templates de e-mail (Administration > Email Templates). */
@Injectable()
export class EmailTemplateService extends AppComponentBase {
  private readonly _baseUrl = `${AppConsts.remoteServiceBaseUrl}/api/services/app/EmailTemplate`;

  constructor(
    injector: Injector,
    private readonly _httpClient: HttpClient,
  ) {
    super(injector);
  }

  getTemplates(): Observable<EmailTemplateListItemDto[]> {
    return this._httpClient
      .get<ListResultOfEmailTemplateListItemDto>(`${this._baseUrl}/GetTemplates`)
      .pipe(map(r => r.items ?? []));
  }

  getForEdit(name: string, culture: string): Observable<EmailTemplateForEditDto> {
    return this._httpClient.post<EmailTemplateForEditDto>(`${this._baseUrl}/GetForEdit`, { name, culture });
  }

  createOrUpdateOverride(name: string, culture: string, body: string): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/CreateOrUpdateOverride`, { name, culture, body });
  }

  resetToDefault(name: string, culture: string): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/ResetToDefault`, { name, culture });
  }

  getPreview(name: string, culture: string, body?: string): Observable<EmailTemplatePreviewDto> {
    return this._httpClient.post<EmailTemplatePreviewDto>(`${this._baseUrl}/GetPreview`, { name, culture, body });
  }
}
