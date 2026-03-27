import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { HttpClient } from '@angular/common/http';
import { catchError, map, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Employee } from './employee.actions';
import * as A from './employee.actions';

@Injectable()
export class EmployeeEffects {
  private base = `${environment.apiUrl}/employees`;

  load$ = createEffect(() =>
    this.actions$.pipe(
      ofType(A.loadEmployees),
      switchMap(() =>
        this.http.get<{ items: Employee[]; total: number }>(this.base).pipe(
          map(({ items }) => A.loadEmployeesSuccess({ employees: items })),
          catchError((err) => of(A.loadEmployeesFailure({ error: err.message }))),
        )
      ),
    )
  );

  create$ = createEffect(() =>
    this.actions$.pipe(
      ofType(A.createEmployee),
      switchMap(({ dto }) =>
        this.http.post<Employee>(this.base, dto).pipe(
          map((employee) => A.createEmployeeSuccess({ employee })),
          catchError((err) => of(A.loadEmployeesFailure({ error: err.message }))),
        )
      ),
    )
  );

  delete$ = createEffect(() =>
    this.actions$.pipe(
      ofType(A.deleteEmployee),
      switchMap(({ id }) =>
        this.http.delete(`${this.base}/${id}`).pipe(
          map(() => A.deleteEmployeeSuccess({ id })),
          catchError((err) => of(A.loadEmployeesFailure({ error: err.message }))),
        )
      ),
    )
  );

  constructor(private actions$: Actions, private http: HttpClient) {}
}
