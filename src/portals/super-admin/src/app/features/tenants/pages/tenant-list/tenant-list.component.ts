import { Component, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { map, combineLatest, startWith } from 'rxjs';

import { loadTenants } from '../../store/tenant.actions';
import { selectAllTenants } from '../../store/tenant.reducer';

@Component({
  selector: 'app-tenant-list',
  standalone: true,
  imports: [
    AsyncPipe, RouterLink, ReactiveFormsModule,
    MatTableModule, MatButtonModule, MatIconModule,
    MatInputModule, MatChipsModule, MatProgressBarModule, TranslateModule,
  ],
  template: `
    <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:16px">
      <h2>{{ 'tenants.title' | translate }}</h2>
      <a mat-raised-button color="primary" routerLink="/tenants/new">
        <mat-icon>add</mat-icon> {{ 'tenants.add' | translate }}
      </a>
    </div>

    <mat-form-field appearance="outline" style="width:300px;margin-bottom:16px">
      <mat-label>{{ 'common.search' | translate }}</mat-label>
      <input matInput [formControl]="search" />
    </mat-form-field>

    <mat-table [dataSource]="(filtered$ | async) ?? []">
      <ng-container matColumnDef="name">
        <mat-header-cell *matHeaderCellDef>{{ 'tenants.name' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let t">{{ t.name }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="code">
        <mat-header-cell *matHeaderCellDef>{{ 'tenants.code' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let t">{{ t.code }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="adminEmail">
        <mat-header-cell *matHeaderCellDef>{{ 'tenants.admin_email' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let t">{{ t.adminEmail }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="status">
        <mat-header-cell *matHeaderCellDef>{{ 'common.status' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let t">
          <mat-chip [color]="t.isActive ? 'primary' : 'warn'" highlighted>
            {{ (t.isActive ? 'common.active' : 'common.inactive') | translate }}
          </mat-chip>
        </mat-cell>
      </ng-container>
      <ng-container matColumnDef="actions">
        <mat-header-cell *matHeaderCellDef></mat-header-cell>
        <mat-cell *matCellDef="let t">
          <a mat-icon-button [routerLink]="['/tenants', t.id, 'edit']">
            <mat-icon>edit</mat-icon>
          </a>
        </mat-cell>
      </ng-container>

      <mat-header-row *matHeaderRowDef="columns"></mat-header-row>
      <mat-row *matRowDef="let row; columns: columns;"></mat-row>
    </mat-table>
  `,
})
export class TenantListComponent implements OnInit {
  columns = ['name', 'code', 'adminEmail', 'status', 'actions'];
  search = new FormControl('');

  tenants$ = this.store.select((s: any) => selectAllTenants(s.tenants));

  filtered$ = combineLatest([
    this.tenants$,
    this.search.valueChanges.pipe(startWith('')),
  ]).pipe(
    map(([tenants, q]) =>
      tenants.filter(
        (t) =>
          !q ||
          t.name.toLowerCase().includes(q.toLowerCase()) ||
          t.code.toLowerCase().includes(q.toLowerCase())
      )
    )
  );

  constructor(private store: Store) {}

  ngOnInit(): void {
    this.store.dispatch(loadTenants());
  }
}
