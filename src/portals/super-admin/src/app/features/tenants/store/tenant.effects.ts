import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { HttpClient } from '@angular/common/http';
import { catchError, map, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Tenant } from '../models/tenant.model';
import * as TenantActions from './tenant.actions';

@Injectable()
export class TenantEffects {
  private readonly base = `${environment.apiUrl}/tenants`;

  loadTenants$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TenantActions.loadTenants),
      switchMap(() =>
        this.http.get<{ items: Tenant[] }>(this.base).pipe(
          map(({ items }) => TenantActions.loadTenantsSuccess({ tenants: items })),
          catchError((err) =>
            of(TenantActions.loadTenantsFailure({ error: err.message }))
          ),
        )
      ),
    )
  );

  createTenant$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TenantActions.createTenant),
      switchMap(({ dto }) =>
        this.http.post<Tenant>(this.base, dto).pipe(
          map((tenant) => TenantActions.createTenantSuccess({ tenant })),
          catchError((err) =>
            of(TenantActions.createTenantFailure({ error: err.message }))
          ),
        )
      ),
    )
  );

  updateTenant$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TenantActions.updateTenant),
      switchMap(({ id, dto }) =>
        this.http.put<Tenant>(`${this.base}/${id}`, dto).pipe(
          map((tenant) => TenantActions.updateTenantSuccess({ tenant })),
          catchError((err) =>
            of(TenantActions.updateTenantFailure({ error: err.message }))
          ),
        )
      ),
    )
  );

  constructor(private actions$: Actions, private http: HttpClient) {}
}
