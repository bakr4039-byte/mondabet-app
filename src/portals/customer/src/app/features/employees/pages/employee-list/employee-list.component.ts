import { Component, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { HttpClient } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TranslateModule } from '@ngx-translate/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { map, combineLatest, startWith } from 'rxjs';
import { loadEmployees, deleteEmployee } from '../../store/employee.actions';
import { selectAllEmployees } from '../../store/employee.reducer';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [
    AsyncPipe, RouterLink, ReactiveFormsModule,
    MatTableModule, MatButtonModule, MatIconModule,
    MatInputModule, MatFormFieldModule, MatProgressBarModule,
    MatSnackBarModule, TranslateModule,
  ],
  template: `
    <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:16px">
      <h2>{{ 'employees.title' | translate }}</h2>
      <div style="display:flex;gap:8px">
        <button mat-raised-button color="accent" (click)="importFile.click()">
          <mat-icon>upload</mat-icon> {{ 'employees.import' | translate }}
        </button>
        <input #importFile type="file" accept=".xlsx,.xls" hidden (change)="onImport($event)" />
        <a mat-raised-button color="primary" routerLink="/employees/new">
          <mat-icon>add</mat-icon> {{ 'employees.add' | translate }}
        </a>
      </div>
    </div>

    <mat-form-field appearance="outline" style="width:300px;margin-bottom:16px">
      <mat-label>{{ 'common.search' | translate }}</mat-label>
      <input matInput [formControl]="search" />
    </mat-form-field>

    <mat-table [dataSource]="(filtered$ | async) ?? []">
      <ng-container matColumnDef="name">
        <mat-header-cell *matHeaderCellDef>{{ 'employees.name' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let e">{{ e.firstName }} {{ e.lastName }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="iqama">
        <mat-header-cell *matHeaderCellDef>{{ 'employees.iqama' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let e">{{ e.iqama }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="jobTitle">
        <mat-header-cell *matHeaderCellDef>{{ 'employees.job_title' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let e">{{ e.jobTitle }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="actions">
        <mat-header-cell *matHeaderCellDef></mat-header-cell>
        <mat-cell *matCellDef="let e">
          <button mat-icon-button color="warn" (click)="remove(e.id)">
            <mat-icon>delete</mat-icon>
          </button>
        </mat-cell>
      </ng-container>
      <mat-header-row *matHeaderRowDef="columns"></mat-header-row>
      <mat-row *matRowDef="let row; columns: columns;"></mat-row>
    </mat-table>
  `,
})
export class EmployeeListComponent implements OnInit {
  columns = ['name', 'iqama', 'jobTitle', 'actions'];
  search = new FormControl('');

  employees$ = this.store.select((s: any) => selectAllEmployees(s.employees));
  filtered$ = combineLatest([
    this.employees$,
    this.search.valueChanges.pipe(startWith('')),
  ]).pipe(
    map(([emps, q]) =>
      emps.filter(
        (e) =>
          !q ||
          `${e.firstName} ${e.lastName}`.toLowerCase().includes(q.toLowerCase()) ||
          e.iqama.includes(q ?? '')
      )
    )
  );

  constructor(
    private store: Store,
    private http: HttpClient,
    private snackbar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.store.dispatch(loadEmployees());
  }

  remove(id: string): void {
    this.store.dispatch(deleteEmployee({ id }));
  }

  onImport(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    const fd = new FormData();
    fd.append('file', input.files[0]);
    this.http.post(`${environment.apiUrl}/employees/import`, fd).subscribe({
      next: (res: any) =>
        this.snackbar.open(
          `Imported: ${res.successCount}, Errors: ${res.errors?.length ?? 0}`,
          'OK',
          { duration: 4000 },
        ),
      error: () =>
        this.snackbar.open('Import failed', 'OK', { duration: 3000 }),
    });
  }
}
