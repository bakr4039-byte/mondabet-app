import { createAction, props } from '@ngrx/store';
import { Package, PackageCreateDto } from '../models/package.model';

export const loadPackages = createAction('[Packages] Load');
export const loadPackagesSuccess = createAction('[Packages] Load Success', props<{ packages: Package[] }>());
export const loadPackagesFailure = createAction('[Packages] Load Failure', props<{ error: string }>());

export const createPackage = createAction('[Packages] Create', props<{ dto: PackageCreateDto }>());
export const createPackageSuccess = createAction('[Packages] Create Success', props<{ pkg: Package }>());

export const deletePackage = createAction('[Packages] Delete', props<{ id: string }>());
export const deletePackageSuccess = createAction('[Packages] Delete Success', props<{ id: string }>());
