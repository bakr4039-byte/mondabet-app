import { Component, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { createPackage, deletePackage, loadPackages } from '../../store/package.actions';
import { selectAllPackages } from '../../store/package.reducer';

@Component({
  selector: 'app-package-list',
  standalone: true,
  imports: [
    AsyncPipe, ReactiveFormsModule,
    MatTableModule, MatButtonModule, MatIconModule,
    MatFormFieldModule, MatInputModule, MatCardModule, MatDialogModule, TranslateModule,
  ],
  template: `
    <h2>{{ 'packages.title' | translate }}</h2>

    <mat-card style="margin-bottom:24px">
      <mat-card-header><mat-card-title>{{ 'packages.add' | translate }}</mat-card-title></mat-card-header>
      <mat-card-content>
        <form [formGroup]="form" (ngSubmit)="add()" style="display:flex;gap:12px;flex-wrap:wrap">
          <mat-form-field appearance="outline">
            <mat-label>{{ 'packages.name' | translate }}</mat-label>
            <input matInput formControlName="name" />
          </mat-form-field>
          <mat-form-field appearance="outline" style="width:120px">
            <mat-label>{{ 'packages.max_users' | translate }}</mat-label>
            <input matInput type="number" formControlName="maxUsers" />
          </mat-form-field>
          <mat-form-field appearance="outline" style="width:140px">
            <mat-label>{{ 'packages.price' | translate }}</mat-label>
            <input matInput type="number" formControlName="priceMonthly" />
          </mat-form-field>
          <mat-form-field appearance="outline" style="flex:1;min-width:200px">
            <mat-label>{{ 'packages.features' | translate }}</mat-label>
            <input matInput formControlName="features" placeholder='{"attendance":true}' />
          </mat-form-field>
          <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">
            {{ 'common.add' | translate }}
          </button>
        </form>
      </mat-card-content>
    </mat-card>

    <mat-table [dataSource]="(packages$ | async) ?? []">
      <ng-container matColumnDef="name">
        <mat-header-cell *matHeaderCellDef>{{ 'packages.name' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let p">{{ p.name }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="maxUsers">
        <mat-header-cell *matHeaderCellDef>{{ 'packages.max_users' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let p">{{ p.maxUsers }}</mat-cell>
      </ng-container>
      <ng-container matColumnDef="price">
        <mat-header-cell *matHeaderCellDef>{{ 'packages.price' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let p">{{ p.priceMonthly }} SAR</mat-cell>
      </ng-container>
      <ng-container matColumnDef="actions">
        <mat-header-cell *matHeaderCellDef></mat-header-cell>
        <mat-cell *matCellDef="let p">
          <button mat-icon-button color="warn" (click)="remove(p.id)">
            <mat-icon>delete</mat-icon>
          </button>
        </mat-cell>
      </ng-container>
      <mat-header-row *matHeaderRowDef="columns"></mat-header-row>
      <mat-row *matRowDef="let row; columns: columns;"></mat-row>
    </mat-table>
  `,
})
export class PackageListComponent implements OnInit {
  columns = ['name', 'maxUsers', 'price', 'actions'];
  packages$ = this.store.select((s: any) => selectAllPackages(s.packages));

  form = this.fb.group({
    name: ['', Validators.required],
    maxUsers: [100, [Validators.required, Validators.min(1)]],
    priceMonthly: [0, Validators.required],
    features: ['{}'],
  });

  constructor(private store: Store, private fb: FormBuilder) {}

  ngOnInit(): void {
    this.store.dispatch(loadPackages());
  }

  add(): void {
    if (this.form.invalid) return;
    this.store.dispatch(createPackage({ dto: this.form.value as any }));
    this.form.reset({ maxUsers: 100, priceMonthly: 0, features: '{}' });
  }

  remove(id: string): void {
    this.store.dispatch(deletePackage({ id }));
  }
}
