import { leaveReducer, LeaveState, selectAllLeaves } from './leave.reducer';
import {
  loadLeaves, loadLeavesSuccess, loadLeavesFailure,
  approveLeaveSuccess, rejectLeaveSuccess, LeaveRequest,
} from './leave.actions';

const mockLeave: LeaveRequest = {
  id: 'leave-1',
  employeeId: 'emp-1',
  employeeName: 'Ali Hassan',
  type: 'vacation',
  status: 'pending',
  fromDate: '2025-01-10',
  toDate: '2025-01-15',
  reason: 'Annual leave',
  createdAt: '2025-01-01T00:00:00Z',
};

describe('leaveReducer', () => {
  it('should return initial state', () => {
    const state = leaveReducer(undefined, { type: '@@INIT' } as any);
    expect(state.loading).toBe(false);
    expect(state.error).toBeNull();
    expect(selectAllLeaves(state)).toEqual([]);
  });

  it('should set loading on loadLeaves', () => {
    const state = leaveReducer(undefined, loadLeaves());
    expect(state.loading).toBe(true);
    expect(state.error).toBeNull();
  });

  it('should populate leaves on loadLeavesSuccess', () => {
    const state = leaveReducer(undefined, loadLeavesSuccess({ leaves: [mockLeave] }));
    expect(state.loading).toBe(false);
    expect(selectAllLeaves(state)).toHaveLength(1);
    expect(selectAllLeaves(state)[0].employeeName).toBe('Ali Hassan');
  });

  it('should set error on loadLeavesFailure', () => {
    const state = leaveReducer(undefined, loadLeavesFailure({ error: 'Network error' }));
    expect(state.loading).toBe(false);
    expect(state.error).toBe('Network error');
  });

  it('should upsert leave on approveLeaveSuccess', () => {
    const loaded = leaveReducer(undefined, loadLeavesSuccess({ leaves: [mockLeave] }));
    const approved: LeaveRequest = { ...mockLeave, status: 'approved', reviewComment: 'Granted' };
    const state = leaveReducer(loaded, approveLeaveSuccess({ leave: approved }));
    expect(selectAllLeaves(state)[0].status).toBe('approved');
    expect(selectAllLeaves(state)[0].reviewComment).toBe('Granted');
  });

  it('should upsert leave on rejectLeaveSuccess', () => {
    const loaded = leaveReducer(undefined, loadLeavesSuccess({ leaves: [mockLeave] }));
    const rejected: LeaveRequest = { ...mockLeave, status: 'rejected', reviewComment: 'Insufficient balance' };
    const state = leaveReducer(loaded, rejectLeaveSuccess({ leave: rejected }));
    expect(selectAllLeaves(state)[0].status).toBe('rejected');
  });
});
