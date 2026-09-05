import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { HttpClient, HttpParams } from '@angular/common/http';
import { catchError, map, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import * as A from './audit.actions';
import { AuditLogEntry, AuditSource } from './audit.actions';

interface AuditLogPageResponse {
  items: AuditLogEntry[];
  total: number;
  page: number;
  size: number;
}

const SOURCE_PATH: Record<AuditSource, string> = {
  identity: 'audit/identity/logs',
  employees: 'audit/employees/logs',
  leaves: 'audit/leaves/logs',
};

@Injectable()
export class AuditEffects {
  load$ = createEffect(() =>
    this.actions$.pipe(
      ofType(A.loadAuditLogs),
      switchMap(({ source, filter }) => {
        let params = new HttpParams()
          .set('page', filter.page)
          .set('size', filter.size);
        if (filter.action) params = params.set('action', filter.action);
        if (filter.from) params = params.set('from', filter.from);
        if (filter.to) params = params.set('to', filter.to);

        return this.http
          .get<AuditLogPageResponse>(`${environment.apiUrl}/${SOURCE_PATH[source]}`, { params })
          .pipe(
            map((res) =>
              A.loadAuditLogsSuccess({
                source,
                items: res.items,
                total: res.total,
                page: res.page,
                size: res.size,
              }),
            ),
            catchError((err) => of(A.loadAuditLogsFailure({ source, error: err.message }))),
          );
      }),
    ),
  );

  constructor(private actions$: Actions, private http: HttpClient) {}
}
