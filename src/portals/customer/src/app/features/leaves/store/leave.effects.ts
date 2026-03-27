import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { HttpClient } from '@angular/common/http';
import { switchMap, map, catchError, of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  loadLeaves, loadLeavesSuccess, loadLeavesFailure,
  approveLeave, approveLeaveSuccess,
  rejectLeave, rejectLeaveSuccess,
  LeaveRequest,
} from './leave.actions';

@Injectable()
export class LeaveEffects {
  loadLeaves$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadLeaves),
      switchMap(() =>
        this.http.get<LeaveRequest[]>(`${environment.apiUrl}/leaves`).pipe(
          map((leaves) => loadLeavesSuccess({ leaves })),
          catchError((err) => of(loadLeavesFailure({ error: err.message }))),
        ),
      ),
    ),
  );

  approveLeave$ = createEffect(() =>
    this.actions$.pipe(
      ofType(approveLeave),
      switchMap(({ id, comment }) =>
        this.http
          .put<LeaveRequest>(`${environment.apiUrl}/leaves/${id}/approve`, { comment })
          .pipe(map((leave) => approveLeaveSuccess({ leave }))),
      ),
    ),
  );

  rejectLeave$ = createEffect(() =>
    this.actions$.pipe(
      ofType(rejectLeave),
      switchMap(({ id, comment }) =>
        this.http
          .put<LeaveRequest>(`${environment.apiUrl}/leaves/${id}/reject`, { comment })
          .pipe(map((leave) => rejectLeaveSuccess({ leave }))),
      ),
    ),
  );

  constructor(private actions$: Actions, private http: HttpClient) {}
}
