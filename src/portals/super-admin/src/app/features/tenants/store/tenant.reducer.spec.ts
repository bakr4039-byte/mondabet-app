import { tenantReducer, TenantState } from './tenant.reducer';
import * as TenantActions from './tenant.actions';
import { Tenant } from '../models/tenant.model';

const mockTenant: Tenant = {
  id: 'tenant-1',
  name: 'ACME Corp',
  code: 'acme',
  adminEmail: 'admin@acme.com',
  adminMobile: '+966500000001',
  primaryColor: '#1976D2',
  secondaryColor: '#424242',
  logoUrl: '',
  isActive: true,
  subscriptionEndsAt: '2026-12-31',
  packageId: 'pkg-1',
};

describe('tenantReducer', () => {
  it('should return initial state', () => {
    const state = tenantReducer(undefined, { type: '@@INIT' } as any);
    expect(state.loading).toBe(false);
    expect(state.ids).toHaveLength(0);
  });

  it('should set loading on loadTenants', () => {
    const state = tenantReducer(undefined, TenantActions.loadTenants());
    expect(state.loading).toBe(true);
  });

  it('should populate entities on loadTenantsSuccess', () => {
    const state = tenantReducer(
      undefined,
      TenantActions.loadTenantsSuccess({ tenants: [mockTenant] }),
    );
    expect(state.ids).toContain('tenant-1');
    expect(state.loading).toBe(false);
  });

  it('should add tenant on createTenantSuccess', () => {
    const state = tenantReducer(
      undefined,
      TenantActions.createTenantSuccess({ tenant: mockTenant }),
    );
    expect(state.ids).toContain('tenant-1');
  });

  it('should upsert tenant on updateTenantSuccess', () => {
    const updated = { ...mockTenant, name: 'ACME Updated' };
    const after = tenantReducer(
      undefined,
      TenantActions.updateTenantSuccess({ tenant: updated }),
    );
    expect((after.entities['tenant-1'] as Tenant).name).toBe('ACME Updated');
  });

  it('should set error on loadTenantsFailure', () => {
    const state = tenantReducer(
      undefined,
      TenantActions.loadTenantsFailure({ error: 'Network error' }),
    );
    expect(state.error).toBe('Network error');
    expect(state.loading).toBe(false);
  });
});
