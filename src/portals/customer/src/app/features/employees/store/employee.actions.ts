import { createAction, props } from '@ngrx/store';

export interface Employee {
  id: string;
  firstName: string;
  lastName: string;
  iqama: string;
  jobTitle: string;
  departmentId: string;
  isActive: boolean;
}

export interface EmployeeCreateDto {
  firstName: string;
  lastName: string;
  iqama: string;
  jobTitle: string;
  departmentId: string;
}

export const loadEmployees = createAction('[Employees] Load');
export const loadEmployeesSuccess = createAction('[Employees] Load Success', props<{ employees: Employee[] }>());
export const loadEmployeesFailure = createAction('[Employees] Load Failure', props<{ error: string }>());
export const createEmployee = createAction('[Employees] Create', props<{ dto: EmployeeCreateDto }>());
export const createEmployeeSuccess = createAction('[Employees] Create Success', props<{ employee: Employee }>());
export const deleteEmployee = createAction('[Employees] Delete', props<{ id: string }>());
export const deleteEmployeeSuccess = createAction('[Employees] Delete Success', props<{ id: string }>());
