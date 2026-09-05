import { Component, OnInit } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { environment } from '../../../../../environments/environment';

interface EmployeePunctuality {
  employeeId: string;
  employeeName: string;
  employeeNameAr: string;
  totalCheckIns: number;
  onTimeCount: number;
  lateCount: number;
  punctualityScore: number;
  consecutiveOnTimeDays: number;
  badge: 'Gold' | 'Silver' | 'Bronze' | 'None';
}

interface ChartDatum {
  name: string;
  value: number;
}

/// Punctuality/discipline analytics dashboard - visualizes the new
/// GET /api/v1/reports/punctuality endpoint. Idea sourced from researching Saudi K-12
/// attendance-analytics platforms (e.g. NOOR's customizable per-school/region dashboards),
/// adapted here as a per-employee punctuality view added alongside the existing file-download
/// attendance report rather than replacing it - this is the first Report screen in the portal
/// to render live JSON with a chart instead of a downloadable file.
@Component({
  selector: 'app-punctuality-analytics',
  standalone: true,
  imports: [
    DecimalPipe, ReactiveFormsModule,
    MatCardModule, MatButtonModule, MatIconModule,
    MatFormFieldModule, MatInputModule, MatTableModule, MatChipsModule,
    MatProgressBarModule, TranslateModule, NgxChartsModule,
  ],
  template: `
    <h2>{{ 'punctuality.title' | translate }}</h2>

    <mat-card style="margin-bottom:24px">
      <mat-card-content>
        <form [formGroup]="form" (ngSubmit)="load()" style="display:flex;gap:16px;align-items:flex-end;flex-wrap:wrap">
          <mat-form-field appearance="outline">
            <mat-label>{{ 'punctuality.from' | translate }}</mat-label>
            <input matInput type="date" formControlName="from" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>{{ 'punctuality.to' | translate }}</mat-label>
            <input matInput type="date" formControlName="to" />
          </mat-form-field>
          <button mat-raised-button color="primary" type="submit">{{ 'punctuality.generate' | translate }}</button>
        </form>
      </mat-card-content>
    </mat-card>

    @if (loading) {
      <mat-progress-bar mode="indeterminate"></mat-progress-bar>
    }

    @if (!loading && data.length) {
      <mat-card style="margin-bottom:24px">
        <mat-card-content>
          <ngx-charts-bar-vertical
            [results]="chartData"
            [xAxis]="true"
            [yAxis]="true"
            [showYAxisLabel]="true"
            [yAxisLabel]="'punctuality.score' | translate"
            [scheme]="colorScheme"
            [view]="chartView"
          ></ngx-charts-bar-vertical>
        </mat-card-content>
      </mat-card>

      <mat-table [dataSource]="data">
        <ng-container matColumnDef="name">
          <mat-header-cell *matHeaderCellDef>{{ 'punctuality.employee' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">{{ displayName(r) }}</mat-cell>
        </ng-container>
        <ng-container matColumnDef="checkins">
          <mat-header-cell *matHeaderCellDef>{{ 'punctuality.total_check_ins' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">{{ r.totalCheckIns }}</mat-cell>
        </ng-container>
        <ng-container matColumnDef="onTime">
          <mat-header-cell *matHeaderCellDef>{{ 'punctuality.on_time_count' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">{{ r.onTimeCount }}</mat-cell>
        </ng-container>
        <ng-container matColumnDef="late">
          <mat-header-cell *matHeaderCellDef>{{ 'punctuality.late_count' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">{{ r.lateCount }}</mat-cell>
        </ng-container>
        <ng-container matColumnDef="score">
          <mat-header-cell *matHeaderCellDef>{{ 'punctuality.score' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">{{ r.punctualityScore | number:'1.1-1' }}%</mat-cell>
        </ng-container>
        <ng-container matColumnDef="streak">
          <mat-header-cell *matHeaderCellDef>{{ 'punctuality.streak' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">{{ r.consecutiveOnTimeDays }}</mat-cell>
        </ng-container>
        <ng-container matColumnDef="badge">
          <mat-header-cell *matHeaderCellDef>{{ 'punctuality.badge' | translate }}</mat-header-cell>
          <mat-cell *matCellDef="let r">
            <mat-chip [color]="badgeColor(r.badge)" [highlighted]="r.badge !== 'None'">
              {{ ('punctuality.badges.' + r.badge) | translate }}
            </mat-chip>
          </mat-cell>
        </ng-container>
        <mat-header-row *matHeaderRowDef="columns"></mat-header-row>
        <mat-row *matRowDef="let row; columns: columns;"></mat-row>
      </mat-table>
    }

    @if (!loading && !data.length) {
      <p>{{ 'punctuality.empty' | translate }}</p>
    }
  `,
})
export class PunctualityAnalyticsComponent implements OnInit {
  columns = ['name', 'checkins', 'onTime', 'late', 'score', 'streak', 'badge'];
  data: EmployeePunctuality[] = [];
  chartData: ChartDatum[] = [];
  loading = false;
  colorScheme = { domain: ['#3f51b5', '#5c6bc0', '#7986cb'] } as any;
  chartView: [number, number] = [700, 320];

  form = this.fb.group({
    from: [this._today(-30)],
    to: [this._today(0)],
  });

  constructor(private fb: FormBuilder, private http: HttpClient, private translate: TranslateService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    const { from, to } = this.form.value;
    this.http
      .get<EmployeePunctuality[]>(`${environment.apiUrl}/reports/punctuality`, {
        params: { from: from!, to: to! },
      })
      .subscribe({
        next: (d) => {
          this.data = d;
          this.chartData = d.map((r) => ({ name: this.displayName(r), value: r.punctualityScore }));
          this.loading = false;
        },
        error: () => { this.loading = false; this.data = []; this.chartData = []; },
      });
  }

  displayName(r: EmployeePunctuality): string {
    return this.translate.currentLang === 'en' ? (r.employeeName || r.employeeNameAr) : (r.employeeNameAr || r.employeeName);
  }

  badgeColor(badge: string): 'primary' | 'accent' | 'warn' | undefined {
    return badge === 'Gold' ? 'accent' : badge === 'Silver' || badge === 'Bronze' ? 'primary' : undefined;
  }

  private _today(offsetDays: number): string {
    const d = new Date();
    d.setDate(d.getDate() + offsetDays);
    return d.toISOString().split('T')[0];
  }
}
