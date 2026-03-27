import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { HttpClient } from '@angular/common/http';
import { catchError, map, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Package } from '../models/package.model';
import * as PackageActions from './package.actions';

@Injectable()
export class PackageEffects {
  private readonly base = `${environment.apiUrl}/packages`;

  load$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PackageActions.loadPackages),
      switchMap(() =>
        this.http.get<{ items: Package[] }>(this.base).pipe(
          map(({ items }) => PackageActions.loadPackagesSuccess({ packages: items })),
          catchError((err) => of(PackageActions.loadPackagesFailure({ error: err.message }))),
        )
      ),
    )
  );

  create$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PackageActions.createPackage),
      switchMap(({ dto }) =>
        this.http.post<Package>(this.base, dto).pipe(
          map((pkg) => PackageActions.createPackageSuccess({ pkg })),
          catchError(() => of(PackageActions.loadPackagesFailure({ error: 'Create failed' }))),
        )
      ),
    )
  );

  delete$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PackageActions.deletePackage),
      switchMap(({ id }) =>
        this.http.delete(`${this.base}/${id}`).pipe(
          map(() => PackageActions.deletePackageSuccess({ id })),
          catchError(() => of(PackageActions.loadPackagesFailure({ error: 'Delete failed' }))),
        )
      ),
    )
  );

  constructor(private actions$: Actions, private http: HttpClient) {}
}
