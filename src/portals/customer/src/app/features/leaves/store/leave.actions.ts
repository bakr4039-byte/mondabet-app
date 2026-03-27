import { createAction, props } from '@ngrx/store';

export type LeaveType = 'vacation' | 'permission' | 'excuse';
export type LeaveStatus = 'pending' | 'approved' | 'rejected';

export interface LeaveRequest {
  id: string;
  employeeId: string;
  employeeName: string;
  type: LeaveType;
  status: LeaveStatus;
  fromDate: string;
  toDate: string;
  reason: string;
  reviewComment?: string;
  createdAt: string;
}

export const loadLeaves = createAction('[Leaves] Load');
export const loadLeavesSuccess = createAction('[Leaves] Load Success', props<{ leaves: LeaveRequest[] }>());
export const loadLeavesFailure = createAction('[Leaves] Load Failure', props<{ error: string }>());

export const approveLeave = createAction('[Leaves] Approve', props<{ id: string; comment: string }>());
export const approveLeaveSuccess = createAction('[Leaves] Approve Success', props<{ leave: LeaveRequest }>());

export const rejectLeave = createAction('[Leaves] Reject', props<{ id: string; comment: string }>());
export const rejectLeaveSuccess = createAction('[Leaves] Reject Success', props<{ leave: LeaveRequest }>());
