import { createEntityAdapter, EntityState } from '@ngrx/entity';
import { createReducer, on } from '@ngrx/store';
import { LeaveRequest, loadLeaves, loadLeavesSuccess, loadLeavesFailure, approveLeaveSuccess, rejectLeaveSuccess } from './leave.actions';

export interface LeaveState extends EntityState<LeaveRequest> {
  loading: boolean;
  error: string | null;
}

const adapter = createEntityAdapter<LeaveRequest>();

const initialState: LeaveState = adapter.getInitialState({ loading: false, error: null });

export const leaveReducer = createReducer(
  initialState,
  on(loadLeaves, (s) => ({ ...s, loading: true, error: null })),
  on(loadLeavesSuccess, (s, { leaves }) => adapter.setAll(leaves, { ...s, loading: false })),
  on(loadLeavesFailure, (s, { error }) => ({ ...s, loading: false, error })),
  on(approveLeaveSuccess, (s, { leave }) => adapter.upsertOne(leave, s)),
  on(rejectLeaveSuccess, (s, { leave }) => adapter.upsertOne(leave, s)),
);

export const { selectAll: selectAllLeaves } = adapter.getSelectors();
