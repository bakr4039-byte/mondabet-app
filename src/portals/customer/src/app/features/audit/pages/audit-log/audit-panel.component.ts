import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AsyncPipe, DatePipe } from '@angular/common';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { AuditSourceState } from '../../store/audit.reducer';

/// One source's tab body (filter form + table + paginator) for the Audit Log screen. Split out
/// as its own component - with typed @Input()/@Output() - rather than reusing one <ng-template>
/// across the three tabs via *ngTemplateOutlet, because Angular's strict template checker does
/// not carry concrete property types through an inline ngTemplateOutlet context object here
/// (it resolved to `{}`), which failed the build. A real child component sidesteps that.
@Component({
  selector: 'app-audit-panel',
  standalone: true,
  imports: [
    AsyncPipe, DatePipe, ReactiveFormsModule,
    MatTableModule, MatPaginatorModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule, MatProgressBarModule, TranslateModule,
  ],
  template: `
    <form
      [formGroup]="filterForm"
      (ngSubmit)="search.emit()"
      style="display:flex;gap:16px;align-items:flex-end;flex-wrap:wrap;margin:16px 0"
    >
      <mat-form-field appearance="outline">
        <mat-label>{{ 'audit.action' | translate }}</mat-label>
        <input matInput formControlName="action" [placeholder]="'audit.action_placeholder' | translate" />
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>{{ 'audit.from' | translate }}</mat-label>
        <input matInput type="date" formControlName="from" />
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>{{ 'audit.to' | translate }}</mat-label>
        <input matInput type="date" formControlName="to" />
      </mat-form-field>
      <button mat-raised-button color="primary" type="submit">
        <mat-icon>search</mat-icon> {{ 'common.search' | translate }}
      </button>
    </form>

    @if ((state$ | async)?.loading) {
      <mat-progress-bar mode="indeterminate" style="margin-bottom:8px"></mat-progress-bar>
    }

    <mat-table [dataSource]="(state$ | async)?.items ?? []">
      <ng-container matColumnDef="occurredAt">
        <mat-header-cell *matHeaderCellDef>{{ 'audit.occurred_at' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let r">{{ r.occurredAt | date: 'medium' }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="action">
        <mat-header-cell *matHeaderCellDef>{{ 'audit.action' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let r">{{ r.action }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="entityType">
        <mat-header-cell *matHeaderCellDef>{{ 'audit.entity_type' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let r">{{ r.entityType }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="entityId">
        <mat-header-cell *matHeaderCellDef>{{ 'audit.entity_id' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let r">{{ r.entityId }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="userId">
        <mat-header-cell *matHeaderCellDef>{{ 'audit.user_id' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let r">{{ r.userId }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="ipAddress">
        <mat-header-cell *matHeaderCellDef>{{ 'audit.ip_address' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let r">{{ r.ipAddress || '—' }}</mat-cell>
      </ng-container>
      <mat-header-row *matHeaderRowDef="columns"></mat-header-row>
      <mat-row *matRowDef="let row; columns: columns;"></mat-row>
    </mat-table>

    @if ((state$ | async)?.items?.length === 0 && !(state$ | async)?.loading) {
      <p style="text-align:center;color:#888;margin-top:24px">{{ 'common.no_data' | translate }}</p>
    }

    <mat-paginator
      [length]="(state$ | async)?.total ?? 0"
      [pageSize]="(state$ | async)?.size ?? 20"
      [pageSizeOptions]="[20, 50, 100]"
      [pageIndex]="((state$ | async)?.page ?? 1) - 1"
      (page)="pageEvent.emit($event)"
    ></mat-paginator>
  `,
})
export class AuditPanelComponent {
  @Input({ required: true }) filterForm!: FormGroup;
  @Input({ required: true }) state$!: Observable<AuditSourceState>;
  @Output() search = new EventEmitter<void>();
  @Output() pageEvent = new EventEmitter<PageEvent>();

  columns = ['occurredAt', 'action', 'entityType', 'entityId', 'userId', 'ipAddress'];
}
