import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { HttpClient } from '@angular/common/http';
import { catchError, map, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Shift } from './shift.actions';
import * as A from './shift.actions';

@Injectable()
export class ShiftEffects {
  private base = `${environment.apiUrl}/shifts`;

  load$ = createEffect(() =>
    this.actions$.pipe(
      ofType(A.loadShifts),
      switchMap(() =>
        this.http.get<Shift[]>(this.base).pipe(
          map((shifts) => A.loadShiftsSuccess({ shifts })),
          catchError((err) => of(A.loadShiftsFailure({ error: err.message }))),
        )
      ),
    )
  );

  create$ = createEffect(() =>
    this.actions$.pipe(
      ofType(A.createShift),
      switchMap(({ dto }) =>
        this.http.post<Shift>(this.base, dto).pipe(
          map((shift) => A.createShiftSuccess({ shift })),
          catchError((err) => of(A.loadShiftsFailure({ error: err.message }))),
        )
      ),
    )
  );

  update$ = createEffect(() =>
    this.actions$.pipe(
      ofType(A.updateShift),
      switchMap(({ id, dto }) =>
        this.http.put<Shift>(`${this.base}/${id}`, dto).pipe(
          map((shift) => A.updateShiftSuccess({ shift })),
          catchError((err) => of(A.loadShiftsFailure({ error: err.message }))),
        )
      ),
    )
  );

  constructor(private actions$: Actions, private http: HttpClient) {}
}
