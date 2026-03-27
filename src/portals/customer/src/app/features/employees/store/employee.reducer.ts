import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { Employee } from './employee.actions';
import * as A from './employee.actions';

export interface EmployeeState extends EntityState<Employee> { loading: boolean; error: string | null; }
const adapter: EntityAdapter<Employee> = createEntityAdapter<Employee>();
const initialState: EmployeeState = adapter.getInitialState({ loading: false, error: null });

export const employeeReducer = createReducer(
  initialState,
  on(A.loadEmployees, (s) => ({ ...s, loading: true, error: null })),
  on(A.loadEmployeesSuccess, (s, { employees }) => adapter.setAll(employees, { ...s, loading: false })),
  on(A.loadEmployeesFailure, (s, { error }) => ({ ...s, loading: false, error })),
  on(A.createEmployeeSuccess, (s, { employee }) => adapter.addOne(employee, s)),
  on(A.deleteEmployeeSuccess, (s, { id }) => adapter.removeOne(id, s)),
);

export const { selectAll: selectAllEmployees } = adapter.getSelectors();
