import { Component, OnInit } from '@angular/core';
import { AsyncPipe, NgTemplateOutlet } from '@angular/common';
import { Store } from '@ngrx/store';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatChipsModule } from '@angular/material/chips';
import { TranslateModule } from '@ngx-translate/core';
import { map } from 'rxjs';
import { loadLeaves, approveLeave, rejectLeave, LeaveRequest } from '../../store/leave.actions';
import { selectAllLeaves } from '../../store/leave.reducer';

@Component({
  selector: 'app-leave-queue',
  standalone: true,
  imports: [
    AsyncPipe, NgTemplateOutlet, ReactiveFormsModule,
    MatTabsModule, MatTableModule, MatButtonModule, MatIconModule,
    MatDialogModule, MatFormFieldModule, MatInputModule, MatChipsModule,
    TranslateModule,
  ],
  template: `
    <h2>{{ 'leaves.title' | translate }}</h2>

    <mat-tab-group>
      <mat-tab [label]="'leaves.pending' | translate">
        <ng-container *ngTemplateOutlet="table; context: { $implicit: pending$ | async, status: 'pending' }"></ng-container>
      </mat-tab>
      <mat-tab [label]="'leaves.approved' | translate">
        <ng-container *ngTemplateOutlet="table; context: { $implicit: approved$ | async, status: 'approved' }"></ng-container>
      </mat-tab>
      <mat-tab [label]="'leaves.rejected' | translate">
        <ng-container *ngTemplateOutlet="table; context: { $implicit: rejected$ | async, status: 'rejected' }"></ng-container>
      </mat-tab>
    </mat-tab-group>

    <ng-template #table let-rows let-status="status">
      <mat-table [dataSource]="rows ?? []" style="margin-top:16px">
        <ng-container matColumnDef="employee">
          <mat-header-cell *matHeaderCellDef>{{ 'leaves.employee' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">{{ r.employeeName }}</mat-cell>
        </ng-container>
        <ng-container matColumnDef="type">
          <mat-header-cell *matHeaderCellDef>{{ 'leaves.type' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">
            <mat-chip>{{ 'leaves.types.' + r.type | translate }}</mat-chip>
          </mat-cell>
        </ng-container>
        <ng-container matColumnDef="dates">
          <mat-header-cell *matHeaderCellDef>{{ 'leaves.dates' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">{{ r.fromDate }} → {{ r.toDate }}</mat-cell>
        </ng-container>
        <ng-container matColumnDef="reason">
          <mat-header-cell *matHeaderCellDef>{{ 'leaves.reason' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">{{ r.reason }}</mat-cell>
        </ng-container>
        <ng-container matColumnDef="actions">
          <mat-header-cell *matHeaderCellDef></mat-header-cell>
          <mat-cell *matCellDef="let r">
            @if (status === 'pending') {
              <button mat-icon-button color="primary" (click)="openDialog(r, 'approve')" [title]="'leaves.approve' | translate">
                <mat-icon>check_circle</mat-icon>
              </button>
              <button mat-icon-button color="warn" (click)="openDialog(r, 'reject')" [title]="'leaves.reject' | translate">
                <mat-icon>cancel</mat-icon>
              </button>
            }
          </mat-cell>
        </ng-container>
        <mat-header-row *matHeaderRowDef="columns"></mat-header-row>
        <mat-row *matRowDef="let row; columns: columns;"></mat-row>
      </mat-table>
    </ng-template>
  `,
})
export class LeaveQueueComponent implements OnInit {
  columns = ['employee', 'type', 'dates', 'reason', 'actions'];

  private all$ = this.store.select((s: any) => selectAllLeaves(s.leaves));
  pending$ = this.all$.pipe(map((l) => l.filter((x) => x.status === 'pending')));
  approved$ = this.all$.pipe(map((l) => l.filter((x) => x.status === 'approved')));
  rejected$ = this.all$.pipe(map((l) => l.filter((x) => x.status === 'rejected')));

  constructor(private store: Store, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.store.dispatch(loadLeaves());
  }

  openDialog(leave: LeaveRequest, action: 'approve' | 'reject'): void {
    const comment = window.prompt(
      action === 'approve' ? 'Approval comment (optional):' : 'Rejection reason (required):',
      '',
    );
    if (action === 'reject' && !comment) return;
    if (action === 'approve') {
      this.store.dispatch(approveLeave({ id: leave.id, comment: comment ?? '' }));
    } else {
      this.store.dispatch(rejectLeave({ id: leave.id, comment: comment! }));
    }
  }
}
