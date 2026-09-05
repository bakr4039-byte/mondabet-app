import { createAction, props } from '@ngrx/store';

export type AuditSource = 'identity' | 'employees' | 'leaves';

export interface AuditLogEntry {
  id: string;
  tenantId: string;
  userId: string;
  action: string;
  entityType: string;
  entityId: string;
  ipAddress?: string | null;
  occurredAt: string;
}

export interface AuditFilter {
  action?: string;
  from?: string;
  to?: string;
  page: number;
  size: number;
}

export const loadAuditLogs = createAction(
  '[Audit] Load',
  props<{ source: AuditSource; filter: AuditFilter }>(),
);
export const loadAuditLogsSuccess = createAction(
  '[Audit] Load Success',
  props<{ source: AuditSource; items: AuditLogEntry[]; total: number; page: number; size: number }>(),
);
export const loadAuditLogsFailure = createAction(
  '[Audit] Load Failure',
  props<{ source: AuditSource; error: string }>(),
);
