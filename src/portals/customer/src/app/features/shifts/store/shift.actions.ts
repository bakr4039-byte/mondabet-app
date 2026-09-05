import { createAction, props } from '@ngrx/store';

export interface Shift {
  id: string;
  name: string;
  startTime: string;
  endTime: string;
  latitude: number;
  longitude: number;
  radiusMeters: number;
  daysOfWeekJson: string;
  gracePeriodMinutes: number;
  windowStartMinutes: number;
  windowEndMinutes: number;
  isSplitShift: boolean;
  firstStartTime?: string | null;
  firstEndTime?: string | null;
  secondStartTime?: string | null;
  secondEndTime?: string | null;
}

export type ShiftUpsertDto = Omit<Shift, 'id'>;

export const loadShifts = createAction('[Shifts] Load');
export const loadShiftsSuccess = createAction('[Shifts] Load Success', props<{ shifts: Shift[] }>());
export const loadShiftsFailure = createAction('[Shifts] Load Failure', props<{ error: string }>());

export const createShift = createAction('[Shifts] Create', props<{ dto: ShiftUpsertDto }>());
export const createShiftSuccess = createAction('[Shifts] Create Success', props<{ shift: Shift }>());

export const updateShift = createAction('[Shifts] Update', props<{ id: string; dto: ShiftUpsertDto }>());
export const updateShiftSuccess = createAction('[Shifts] Update Success', props<{ shift: Shift }>());
