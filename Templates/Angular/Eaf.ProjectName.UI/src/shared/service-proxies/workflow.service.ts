import { Injectable, Injector } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';

export class WorkflowTaskDto {
  id: string;
  instanceId: string;
  stepId: string;
  assigneeUserId: number;
  status: string;
  comment: string | undefined;
  dueTime: string | undefined;
  creationTime: string;
}

export class ListResultOfWorkflowTaskDto {
  items: WorkflowTaskDto[];
}

/** Caixa de entrada de aprovações de workflow ("Approvals"). */
@Injectable()
export class WorkflowApprovalService extends AppComponentBase {
  private readonly _baseUrl = `${AppConsts.remoteServiceBaseUrl}/api/services/app/WorkflowApproval`;

  constructor(
    injector: Injector,
    private readonly _httpClient: HttpClient,
  ) {
    super(injector);
  }

  getMyPending(): Observable<WorkflowTaskDto[]> {
    return this._httpClient
      .get<ListResultOfWorkflowTaskDto>(`${this._baseUrl}/GetMyPending`)
      .pipe(map(r => r.items ?? []));
  }

  decide(taskId: string, decision: 'approve' | 'reject' | 'send-back', comment?: string): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/Decide`, { taskId, decision, comment });
  }
}
