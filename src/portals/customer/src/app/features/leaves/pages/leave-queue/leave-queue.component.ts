import { Component, Inject, OnInit } from '@angular/core';
import { AsyncPipe, NgTemplateOutlet } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Store } from '@ngrx/store';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatChipsModule } from '@angular/material/chips';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TranslateModule } from '@ngx-translate/core';
import { map } from 'rxjs';
import { loadLeaves, approveLeave, rejectLeave, LeaveRequest } from '../../store/leave.actions';
import { selectAllLeaves } from '../../store/leave.reducer';
import { environment } from '../../../../../environments/environment';

interface SubstituteCandidate {
  employeeId: string;
  fullNameAr: string;
  fullNameEn: string;
  jobTitle: string;
}

/// Shows the substitute-coverage suggestions for an approved leave/permission request.
/// Idea sourced from researching teacher/school substitute-management software (credential-
/// matched automatic substitute routing) - added as its own small dialog rather than reworking
/// the existing approve/reject flow above.
@Component({
  selector: 'app-substitute-list-dialog',
  standalone: true,
  imports: [MatDialogModule, MatListModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, TranslateModule],
  template: `
    <h2 mat-dialog-title>{{ 'leaves.substitutes_title' | translate }}</h2>
    <mat-dialog-content>
      @if (data.loading) {
        <div style="display:flex;justify-content:center;padding:24px">
          <mat-spinner diameter="32"></mat-spinner>
        </div>
      } @else if (data.error) {
        <p>{{ 'leaves.substitutes_error' | translate }}</p>
      } @else if (!data.candidates.length) {
        <p>{{ 'leaves.substitutes_empty' | translate }}</p>
      } @else {
        <mat-nav-list>
          @for (c of data.candidates; track c.employeeId) {
            <mat-list-item>
              <mat-icon matListItemIcon>person</mat-icon>
              <span matListItemTitle>{{ c.fullNameAr || c.fullNameEn }}</span>
              <span matListItemLine>{{ c.jobTitle }}</span>
            </mat-list-item>
          }
        </mat-nav-list>
      }
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>{{ 'common.close' | translate }}</button>
    </mat-dialog-actions>
  `,
})
export class SubstituteListDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<SubstituteListDialogComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: { loading: boolean; error: boolean; candidates: SubstituteCandidate[] },
  ) {}
}

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
            @if (status === 'approved') {
              <button mat-icon-button (click)="showSubstitutes(r)" [title]="'leaves.substitutes_button' | translate">
                <mat-icon>swap_horiz</mat-icon>
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

  constructor(private store: Store, private dialog: MatDialog, private http: HttpClient) {}

  ngOnInit(): void {
    this.store.dispatch(loadLeaves());
  }

  showSubstitutes(leave: LeaveRequest): void {
    const dialogRef = this.dialog.open(SubstituteListDialogComponent, {
      width: '420px',
      data: { loading: true, error: false, candidates: [] as SubstituteCandidate[] },
    });
    this.http
      .get<SubstituteCandidate[]>(`${environment.apiUrl}/leaves/${leave.id}/substitutes`)
      .subscribe({
        next: (candidates) => { dialogRef.componentInstance.data = { loading: false, error: false, candidates }; },
        error: () => { dialogRef.componentInstance.data = { loading: false, error: true, candidates: [] }; },
      });
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
