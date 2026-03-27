import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { environment } from '../../../../../environments/environment';

interface AttendanceSummary {
  employeeId: string;
  employeeName: string;
  daysPresent: number;
  daysAbsent: number;
  totalHours: number;
  lateCount: number;
}

@Component({
  selector: 'app-attendance-reports',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule, MatButtonModule, MatIconModule,
    MatFormFieldModule, MatInputModule, MatTableModule,
    MatProgressBarModule, TranslateModule,
  ],
  template: `
    <h2>{{ 'reports.title' | translate }}</h2>

    <mat-card style="margin-bottom:24px">
      <mat-card-content>
        <form [formGroup]="form" (ngSubmit)="load()" style="display:flex;gap:16px;align-items:flex-end;flex-wrap:wrap">
          <mat-form-field appearance="outline">
            <mat-label>{{ 'reports.from' | translate }}</mat-label>
            <input matInput type="date" formControlName="from" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>{{ 'reports.to' | translate }}</mat-label>
            <input matInput type="date" formControlName="to" />
          </mat-form-field>
          <button mat-raised-button color="primary" type="submit">{{ 'reports.generate' | translate }}</button>
          <button mat-stroked-button type="button" (click)="download('pdf')">
            <mat-icon>picture_as_pdf</mat-icon> PDF
          </button>
          <button mat-stroked-button type="button" (click)="download('excel')">
            <mat-icon>table_view</mat-icon> Excel
          </button>
        </form>
      </mat-card-content>
    </mat-card>

    @if (loading) {
      <mat-progress-bar mode="indeterminate"></mat-progress-bar>
    }

    @if (data.length) {
      <mat-table [dataSource]="data">
        <ng-container matColumnDef="name">
          <mat-header-cell *matHeaderCellDef>{{ 'reports.employee' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">{{ r.employeeName }}</mat-cell>
        </ng-container>
        <ng-container matColumnDef="present">
          <mat-header-cell *matHeaderCellDef>{{ 'reports.days_present' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">{{ r.daysPresent }}</mat-cell>
        </ng-container>
        <ng-container matColumnDef="absent">
          <mat-header-cell *matHeaderCellDef>{{ 'reports.days_absent' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">{{ r.daysAbsent }}</mat-cell>
        </ng-container>
        <ng-container matColumnDef="hours">
          <mat-header-cell *matHeaderCellDef>{{ 'reports.total_hours' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">{{ r.totalHours | number:'1.1-1' }}</mat-cell>
        </ng-container>
        <ng-container matColumnDef="late">
          <mat-header-cell *matHeaderCellDef>{{ 'reports.late_count' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">{{ r.lateCount }}</mat-cell>
        </ng-container>
        <mat-header-row *matHeaderRowDef="columns"></mat-header-row>
        <mat-row *matRowDef="let row; columns: columns;"></mat-row>
      </mat-table>
    }
  `,
})
export class AttendanceReportsComponent implements OnInit {
  columns = ['name', 'present', 'absent', 'hours', 'late'];
  data: AttendanceSummary[] = [];
  loading = false;

  form = this.fb.group({
    from: [this._today(-30)],
    to: [this._today(0)],
  });

  constructor(private fb: FormBuilder, private http: HttpClient) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    const { from, to } = this.form.value;
    this.http
      .get<AttendanceSummary[]>(`${environment.apiUrl}/reports/attendance`, {
        params: { from: from!, to: to! },
      })
      .subscribe({
        next: (d) => { this.data = d; this.loading = false; },
        error: () => { this.loading = false; },
      });
  }

  download(format: 'pdf' | 'excel'): void {
    const { from, to } = this.form.value;
    window.open(`${environment.apiUrl}/reports/attendance/export?format=${format}&from=${from}&to=${to}`);
  }

  private _today(offsetDays: number): string {
    const d = new Date();
    d.setDate(d.getDate() + offsetDays);
    return d.toISOString().split('T')[0];
  }
}
