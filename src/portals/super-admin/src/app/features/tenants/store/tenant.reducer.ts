import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { Tenant } from '../models/tenant.model';
import * as TenantActions from './tenant.actions';

export interface TenantState extends EntityState<Tenant> {
  loading: boolean;
  error: string | null;
}

const adapter: EntityAdapter<Tenant> = createEntityAdapter<Tenant>();

const initialState: TenantState = adapter.getInitialState({
  loading: false,
  error: null,
});

export const tenantReducer = createReducer(
  initialState,
  on(TenantActions.loadTenants, (state) => ({ ...state, loading: true, error: null })),
  on(TenantActions.loadTenantsSuccess, (state, { tenants }) =>
    adapter.setAll(tenants, { ...state, loading: false })
  ),
  on(TenantActions.loadTenantsFailure, (state, { error }) => ({
    ...state, loading: false, error,
  })),
  on(TenantActions.createTenantSuccess, (state, { tenant }) =>
    adapter.addOne(tenant, state)
  ),
  on(TenantActions.updateTenantSuccess, (state, { tenant }) =>
    adapter.upsertOne(tenant, state)
  ),
);

export const { selectAll: selectAllTenants } = adapter.getSelectors();
