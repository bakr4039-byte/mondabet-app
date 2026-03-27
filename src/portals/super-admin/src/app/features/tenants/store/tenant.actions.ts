import { createAction, props } from '@ngrx/store';
import { Tenant, TenantCreateDto } from '../models/tenant.model';

export const loadTenants = createAction('[Tenants] Load');
export const loadTenantsSuccess = createAction(
  '[Tenants] Load Success',
  props<{ tenants: Tenant[] }>()
);
export const loadTenantsFailure = createAction(
  '[Tenants] Load Failure',
  props<{ error: string }>()
);

export const createTenant = createAction(
  '[Tenants] Create',
  props<{ dto: TenantCreateDto }>()
);
export const createTenantSuccess = createAction(
  '[Tenants] Create Success',
  props<{ tenant: Tenant }>()
);
export const createTenantFailure = createAction(
  '[Tenants] Create Failure',
  props<{ error: string }>()
);

export const updateTenant = createAction(
  '[Tenants] Update',
  props<{ id: string; dto: Partial<TenantCreateDto> }>()
);
export const updateTenantSuccess = createAction(
  '[Tenants] Update Success',
  props<{ tenant: Tenant }>()
);
export const updateTenantFailure = createAction(
  '[Tenants] Update Failure',
  props<{ error: string }>()
);
