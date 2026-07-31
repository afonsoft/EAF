import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams, HttpResponse, HttpEvent } from '@angular/common/http';
import { Observable, throwError as _observableThrow, of as _observableOf } from 'rxjs';
import { mergeMap as _observableMergeMap, catchError as _observableCatch } from 'rxjs/operators';
import { API_BASE_URL } from '@shared/service-proxies/service-proxies';

export interface ICreateEditionInput {
    displayName: string;
    isFree: boolean;
    monthlyPrice?: number | undefined;
    annualPrice?: number | undefined;
    trialDayCount?: number | undefined;
    waitingDayAfterExpire?: number | undefined;
    expiringEditionId?: number | undefined;
}

export interface IUpdateEditionInput extends ICreateEditionInput {
    id: number;
}

export interface IEditionDto {
    displayName: string;
    isFree: boolean;
    monthlyPrice?: number | undefined;
    annualPrice?: number | undefined;
    trialDayCount?: number | undefined;
    waitingDayAfterExpire?: number | undefined;
    expiringEditionId?: number | undefined;
    id: number;
}

export interface IPagedResultDtoOfEditionDto {
    totalCount: number;
    items: IEditionDto[];
}

@Injectable()
export class EditionServiceProxy {
    private http: HttpClient;
    private baseUrl: string;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : '';
    }

    getEditions(filter: string | undefined, sorting: string | undefined, maxResultCount: number | undefined, skipCount: number | undefined): Observable<IPagedResultDtoOfEditionDto> {
        let url_ = this.baseUrl + '/api/services/app/EditionAppService/GetEditions?';
        if (filter !== undefined && filter !== null) url_ += 'Filter=' + encodeURIComponent('' + filter) + '&';
        if (sorting !== undefined && sorting !== null) url_ += 'Sorting=' + encodeURIComponent('' + sorting) + '&';
        if (skipCount !== undefined && skipCount !== null) url_ += 'SkipCount=' + encodeURIComponent('' + skipCount) + '&';
        if (maxResultCount !== undefined && maxResultCount !== null) url_ += 'MaxResultCount=' + encodeURIComponent('' + maxResultCount) + '&';
        url_ = url_.replace(/[?&]$/, '');

        const options: any = { observe: 'response', responseType: 'json' };
        return this.http.request('get', url_, options).pipe(_observableMergeMap((response: any) => this.processGetEditions(response))).pipe(_observableCatch((response: any) => {
            if (response instanceof Error) throw response;
            return _observableThrow(response);
        }));
    }

    private processGetEditions(response: HttpResponse<any>): Observable<IPagedResultDtoOfEditionDto> {
        const status = response.status;
        const responseBlob = (response as any).body || new Blob();
        if (status === 200) {
            return _observableOf(responseBlob as IPagedResultDtoOfEditionDto);
        }
        return _observableThrow(new Error('Unexpected response: ' + status));
    }

    getEditionForEdit(id: number): Observable<IEditionDto> {
        let url_ = this.baseUrl + '/api/services/app/EditionAppService/GetEditionForEdit?';
        url_ += 'Id=' + encodeURIComponent('' + id) + '&';
        url_ = url_.replace(/[?&]$/, '');

        const options: any = { observe: 'response', responseType: 'json' };
        return this.http.request('get', url_, options).pipe(_observableMergeMap((response: any) => this.processEdition(response))).pipe(_observableCatch((response: any) => {
            if (response instanceof Error) throw response;
            return _observableThrow(response);
        }));
    }

    private processEdition(response: HttpResponse<any>): Observable<IEditionDto> {
        const status = response.status;
        const responseBlob = (response as any).body || new Blob();
        if (status === 200) {
            return _observableOf(responseBlob as IEditionDto);
        }
        return _observableThrow(new Error('Unexpected response: ' + status));
    }

    createEdition(input: ICreateEditionInput): Observable<void> {
        let url_ = this.baseUrl + '/api/services/app/EditionAppService/CreateEdition';
        url_ = url_.replace(/[?&]$/, '');

        const content_ = JSON.stringify(input);
        const options: any = { body: content_, headers: { 'Content-Type': 'application/json' }, observe: 'response', responseType: 'blob' };
        return this.http.request('post', url_, options).pipe(_observableMergeMap((response: any) => this.processAction(response))).pipe(_observableCatch((response: any) => {
            if (response instanceof Error) throw response;
            return _observableThrow(response);
        }));
    }

    updateEdition(input: IUpdateEditionInput): Observable<void> {
        let url_ = this.baseUrl + '/api/services/app/EditionAppService/UpdateEdition';
        url_ = url_.replace(/[?&]$/, '');

        const content_ = JSON.stringify(input);
        const options: any = { body: content_, headers: { 'Content-Type': 'application/json' }, observe: 'response', responseType: 'blob' };
        return this.http.request('put', url_, options).pipe(_observableMergeMap((response: any) => this.processAction(response))).pipe(_observableCatch((response: any) => {
            if (response instanceof Error) throw response;
            return _observableThrow(response);
        }));
    }

    deleteEdition(id: number): Observable<void> {
        let url_ = this.baseUrl + '/api/services/app/EditionAppService/DeleteEdition?';
        url_ += 'Id=' + encodeURIComponent('' + id) + '&';
        url_ = url_.replace(/[?&]$/, '');

        const options: any = { observe: 'response', responseType: 'blob' };
        return this.http.request('delete', url_, options).pipe(_observableMergeMap((response: any) => this.processAction(response))).pipe(_observableCatch((response: any) => {
            if (response instanceof Error) throw response;
            return _observableThrow(response);
        }));
    }

    private processAction(response: HttpResponse<any>): Observable<void> {
        const status = response.status;
        if (status === 200) {
            return _observableOf(undefined as any);
        }
        return _observableThrow(new Error('Unexpected response: ' + status));
    }
}
