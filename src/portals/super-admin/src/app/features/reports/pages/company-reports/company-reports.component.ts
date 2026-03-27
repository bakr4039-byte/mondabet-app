import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { environment } from '../../../../../environments/environment';

interface CompanyReport {
  tenantName: string;
  totalEmployees: number;
  activeToday: number;
  subscriptionStatus: string;
}

@Component({
  selector: 'app-company-reports',
  standalone: true,
  imports: [NgxChartsModule, MatTableModule, MatButtonModule, MatIconModule, TranslateModule],
  template: `
    <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:16px">
      <h2>{{ 'reports.company_title' | translate }}</h2>
      <div style="display:flex;gap:8px">
        <button mat-raised-button (click)="download('pdf')">
          <mat-icon>picture_as_pdf</mat-icon> {{ 'reports.download_pdf' | translate }}
        </button>
        <button mat-raised-button (click)="download('xlsx')">
          <mat-icon>table_chart</mat-icon> {{ 'reports.download_excel' | translate }}
        </button>
      </div>
    </div>

    <ngx-charts-bar-vertical
      *ngIf="chartData.length"
      [results]="chartData"
      [xAxis]="true"
      [yAxis]="true"
      [legend]="false"
      [showXAxisLabel]="true"
      [xAxisLabel]="'reports.company' | translate"
      [yAxisLabel]="'reports.employees' | translate"
      style="display:block;height:250px;margin-bottom:24px">
    </ngx-charts-bar-vertical>

    <mat-table [dataSource]="rows">
      <ng-container matColumnDef="name">
        <mat-header-cell *matHeaderCellDef>{{ 'tenants.name' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let r">{{ r.tenantName }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="employees">
        <mat-header-cell *matHeaderCellDef>{{ 'reports.employees' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let r">{{ r.totalEmployees }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="active">
        <mat-header-cell *matHeaderCellDef>{{ 'reports.active_today' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let r">{{ r.activeToday }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="status">
        <mat-header-cell *matHeaderCellDef>{{ 'common.status' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let r">{{ r.subscriptionStatus }}</mat-cell>
      </ng-container>
      <mat-header-row *matHeaderRowDef="columns"></mat-header-row>
      <mat-row *matRowDef="let row; columns: columns;"></mat-row>
    </mat-table>
  `,
})
export class CompanyReportsComponent implements OnInit {
  columns = ['name', 'employees', 'active', 'status'];
  rows: CompanyReport[] = [];
  chartData: { name: string; value: number }[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.http
      .get<{ items: CompanyReport[] }>(`${environment.apiUrl}/reports/companies`)
      .subscribe((r) => {
        this.rows = r.items;
        this.chartData = r.items.map((c) => ({
          name: c.tenantName,
          value: c.totalEmployees,
        }));
      });
  }

  download(format: string): void {
    window.open(`${environment.apiUrl}/reports/companies?format=${format}`, '_blank');
  }
}
