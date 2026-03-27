import { Component } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';
import { createEmployee } from '../../store/employee.actions';

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule, TranslateModule],
  template: `
    <mat-card>
      <mat-card-header><mat-card-title>{{ 'employees.add' | translate }}</mat-card-title></mat-card-header>
      <mat-card-content>
        <form [formGroup]="form" (ngSubmit)="submit()" style="display:grid;gap:12px">
          <mat-form-field appearance="outline">
            <mat-label>{{ 'employees.first_name' | translate }}</mat-label>
            <input matInput formControlName="firstName" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>{{ 'employees.last_name' | translate }}</mat-label>
            <input matInput formControlName="lastName" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>{{ 'employees.iqama' | translate }}</mat-label>
            <input matInput formControlName="iqama" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>{{ 'employees.job_title' | translate }}</mat-label>
            <input matInput formControlName="jobTitle" />
          </mat-form-field>
          <div style="display:flex;gap:8px;justify-content:flex-end">
            <button mat-button type="button" (click)="router.navigate(['/employees'])">{{ 'common.cancel' | translate }}</button>
            <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">{{ 'common.save' | translate }}</button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
})
export class EmployeeFormComponent {
  form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    iqama: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
    jobTitle: ['', Validators.required],
    departmentId: [''],
  });

  constructor(private fb: FormBuilder, public router: Router, private store: Store) {}

  submit(): void {
    if (this.form.invalid) return;
    this.store.dispatch(createEmployee({ dto: this.form.value as any }));
    this.router.navigate(['/employees']);
  }
}
