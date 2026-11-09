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

export class WorkflowDefinitionDto {
  id: string;
  name: string;
  description: string | undefined;
  category: string | undefined;
  status: string;
  version: number;
  formJson: string;
  flowJson: string;
}

export class ListResultOfWorkflowDefinitionDto {
  items: WorkflowDefinitionDto[];
}

export class WorkflowInstanceDto {
  id: string;
  definitionId: string;
  version: number;
  requesterUserId: number;
  formValuesJson: string;
  currentStepId: string;
  status: string;
  creationTime: string;
  completedTime: string | undefined;
}

export class ListResultOfWorkflowInstanceDto {
  items: WorkflowInstanceDto[];
}

export class WorkflowTimelineEntryDto {
  eventType: string;
  stepId: string | undefined;
  actorUserId: number | undefined;
  dataJson: string | undefined;
  creationTime: string;
}

export class WorkflowInstanceDetailDto {
  instance: WorkflowInstanceDto;
  tasks: WorkflowTaskDto[];
  timeline: WorkflowTimelineEntryDto[];
}

export class WorkflowSimulationDto {
  path: string[];
  assignees: { [stepId: string]: number[] };
}

export class WorkflowFormFieldModel {
  id: string;
  label: string;
  type: string;
  required: boolean;
  options: string[] | undefined;
}

export class WorkflowAssigneeModel {
  kind: 'users' | 'role' | 'ou' | 'manager';
  ids: string[];
  quorum: 'any' | 'all' | 'quorum';
  quorumCount: number | undefined;
}

export class WorkflowStepModel {
  id: string;
  name: string;
  type: 'approval' | 'condition' | 'notification' | 'end';
  nextStepId: string | undefined;
  rejectStepId: string | undefined;
  condition: string | undefined;
  assignee: WorkflowAssigneeModel | undefined;
  dueTimeHours: number | undefined;
}

/** Administração de definições de workflow ("Administration > Workflows"). */
@Injectable()
export class AdminWorkflowService extends AppComponentBase {
  private readonly _baseUrl = `${AppConsts.remoteServiceBaseUrl}/api/services/app/WorkflowDefinition`;

  constructor(
    injector: Injector,
    private readonly _httpClient: HttpClient,
  ) {
    super(injector);
  }

  getAll(): Observable<WorkflowDefinitionDto[]> {
    return this._httpClient
      .get<ListResultOfWorkflowDefinitionDto>(`${this._baseUrl}/GetAll`)
      .pipe(map(r => r.items ?? []));
  }

  get(id: string): Observable<WorkflowDefinitionDto> {
    return this._httpClient.get<WorkflowDefinitionDto>(`${this._baseUrl}/Get`, { params: { id } });
  }

  createOrUpdate(input: Partial<WorkflowDefinitionDto>): Observable<WorkflowDefinitionDto> {
    return input.id
      ? this._httpClient.post<WorkflowDefinitionDto>(`${this._baseUrl}/Update`, input)
      : this._httpClient.post<WorkflowDefinitionDto>(`${this._baseUrl}/Create`, input);
  }

  publish(id: string): Observable<WorkflowDefinitionDto> {
    return this._httpClient.post<WorkflowDefinitionDto>(`${this._baseUrl}/Publish`, { id });
  }

  archive(id: string): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/Archive`, { id });
  }

  delete(id: string): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/Delete`, { id });
  }
}

/** Catálogo e solicitações do usuário ("Workflows"). */
@Injectable()
export class WorkflowRequestService extends AppComponentBase {
  private readonly _baseUrl = `${AppConsts.remoteServiceBaseUrl}/api/services/app/WorkflowRequest`;

  constructor(
    injector: Injector,
    private readonly _httpClient: HttpClient,
  ) {
    super(injector);
  }

  getCatalog(): Observable<WorkflowDefinitionDto[]> {
    return this._httpClient
      .get<ListResultOfWorkflowDefinitionDto>(`${this._baseUrl}/GetCatalog`)
      .pipe(map(r => r.items ?? []));
  }

  start(definitionId: string, formValues: object): Observable<WorkflowInstanceDto> {
    return this._httpClient.post<WorkflowInstanceDto>(`${this._baseUrl}/Start`, {
      definitionId,
      formValuesJson: JSON.stringify(formValues),
    });
  }

  simulate(definitionId: string, formValues: object): Observable<WorkflowSimulationDto> {
    return this._httpClient.post<WorkflowSimulationDto>(`${this._baseUrl}/Simulate`, {
      definitionId,
      formValuesJson: JSON.stringify(formValues),
    });
  }

  getMyRequests(): Observable<WorkflowInstanceDto[]> {
    return this._httpClient
      .get<ListResultOfWorkflowInstanceDto>(`${this._baseUrl}/GetMyRequests`)
      .pipe(map(r => r.items ?? []));
  }

  getDetail(id: string): Observable<WorkflowInstanceDetailDto> {
    return this._httpClient.get<WorkflowInstanceDetailDto>(`${this._baseUrl}/GetDetail`, { params: { id } });
  }

  cancel(id: string): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/Cancel`, { id });
  }

  resubmit(instanceId: string, formValues: object): Observable<WorkflowInstanceDto> {
    return this._httpClient.post<WorkflowInstanceDto>(`${this._baseUrl}/Resubmit`, {
      instanceId,
      formValuesJson: JSON.stringify(formValues),
    });
  }
}
