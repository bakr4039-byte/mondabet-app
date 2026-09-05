import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { Shift } from './shift.actions';
import * as A from './shift.actions';

export interface ShiftState extends EntityState<Shift> { loading: boolean; error: string | null; }
const adapter: EntityAdapter<Shift> = createEntityAdapter<Shift>();
const initialState: ShiftState = adapter.getInitialState({ loading: false, error: null });

export const shiftReducer = createReducer(
  initialState,
  on(A.loadShifts, (s) => ({ ...s, loading: true, error: null })),
  on(A.loadShiftsSuccess, (s, { shifts }) => adapter.setAll(shifts, { ...s, loading: false })),
  on(A.loadShiftsFailure, (s, { error }) => ({ ...s, loading: false, error })),
  on(A.createShiftSuccess, (s, { shift }) => adapter.addOne(shift, s)),
  on(A.updateShiftSuccess, (s, { shift }) => adapter.upsertOne(shift, s)),
);

export const { selectAll: selectAllShifts, selectEntities: selectShiftEntities } = adapter.getSelectors();
