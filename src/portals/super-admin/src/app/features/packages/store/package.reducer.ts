import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { Package } from '../models/package.model';
import * as PackageActions from './package.actions';

export interface PackageState extends EntityState<Package> {
  loading: boolean;
}

const adapter: EntityAdapter<Package> = createEntityAdapter<Package>();

const initialState: PackageState = adapter.getInitialState({ loading: false });

export const packageReducer = createReducer(
  initialState,
  on(PackageActions.loadPackages, (state) => ({ ...state, loading: true })),
  on(PackageActions.loadPackagesSuccess, (state, { packages }) =>
    adapter.setAll(packages, { ...state, loading: false })
  ),
  on(PackageActions.createPackageSuccess, (state, { pkg }) =>
    adapter.addOne(pkg, state)
  ),
  on(PackageActions.deletePackageSuccess, (state, { id }) =>
    adapter.removeOne(id, state)
  ),
);

export const { selectAll: selectAllPackages } = adapter.getSelectors();
