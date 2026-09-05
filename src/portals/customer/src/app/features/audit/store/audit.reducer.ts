import { createReducer, on } from '@ngrx/store';
import * as A from './audit.actions';
import { AuditLogEntry, AuditSource } from './audit.actions';

export interface AuditSourceState {
  items: AuditLogEntry[];
  total: number;
  page: number;
  size: number;
  loading: boolean;
  error: string | null;
  loaded: boolean;
}

export interface AuditState {
  identity: AuditSourceState;
  employees: AuditSourceState;
  leaves: AuditSourceState;
}

const emptySource = (): AuditSourceState => ({
  items: [], total: 0, page: 1, size: 20, loading: false, error: null, loaded: false,
});

const initialState: AuditState = {
  identity: emptySource(),
  employees: emptySource(),
  leaves: emptySource(),
};

// Explicit per-source branching instead of a computed `[source]: ...` spread key - keeps the
// reducer unambiguous without relying on TypeScript's inference for a union-typed object key
// (this codebase has no CI type-check step to catch a subtle mistake there before it ships).
function withSource(state: AuditState, source: AuditSource, patch: Partial<AuditSourceState>): AuditState {
  const updated: AuditSourceState = { ...state[source], ...patch };
  switch (source) {
    case 'identity':
      return { ...state, identity: updated };
    case 'employees':
      return { ...state, employees: updated };
    case 'leaves':
      return { ...state, leaves: updated };
  }
}

export const auditReducer = createReducer(
  initialState,
  on(A.loadAuditLogs, (s, { source }) => withSource(s, source, { loading: true, error: null })),
  on(A.loadAuditLogsSuccess, (s, { source, items, total, page, size }) =>
    withSource(s, source, { items, total, page, size, loading: false, loaded: true }),
  ),
  on(A.loadAuditLogsFailure, (s, { source, error }) => withSource(s, source, { loading: false, error })),
);

export function selectAuditSource(state: AuditState, source: AuditSource): AuditSourceState {
  return state[source];
}
