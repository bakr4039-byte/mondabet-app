import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCardModule } from '@angular/material/card';
import { TranslateModule } from '@ngx-translate/core';
import { Store } from '@ngrx/store';
import { createTenant, updateTenant } from '../../store/tenant.actions';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-tenant-form',
  standalone: true,
  imports: [
    ReactiveFormsModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatSelectModule, MatDatepickerModule,
    MatNativeDateModule, MatCardModule, TranslateModule,
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>{{ (isEdit ? 'tenants.edit' : 'tenants.add') | translate }}</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <form [formGroup]="form" (ngSubmit)="submit()" style="display:grid;gap:12px">
          <mat-form-field appearance="outline">
            <mat-label>{{ 'tenants.name' | translate }}</mat-label>
            <input matInput formControlName="name" />
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>{{ 'tenants.code' | translate }}</mat-label>
            <input matInput formControlName="code" [readonly]="isEdit" />
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>{{ 'tenants.admin_email' | translate }}</mat-label>
            <input matInput formControlName="adminEmail" type="email" />
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>{{ 'tenants.admin_mobile' | translate }}</mat-label>
            <input matInput formControlName="adminMobile" />
          </mat-form-field>

          <div style="display:flex;gap:12px">
            <mat-form-field appearance="outline" style="flex:1">
              <mat-label>{{ 'tenants.primary_color' | translate }}</mat-label>
              <input matInput formControlName="primaryColor" placeholder="#1976D2" />
            </mat-form-field>
            <mat-form-field appearance="outline" style="flex:1">
              <mat-label>{{ 'tenants.secondary_color' | translate }}</mat-label>
              <input matInput formControlName="secondaryColor" placeholder="#424242" />
            </mat-form-field>
          </div>

          <mat-form-field appearance="outline">
            <mat-label>{{ 'tenants.package' | translate }}</mat-label>
            <mat-select formControlName="packageId">
              <mat-option *ngFor="let p of packages" [value]="p.id">{{ p.name }}</mat-option>
            </mat-select>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>{{ 'tenants.subscription_ends' | translate }}</mat-label>
            <input matInput [matDatepicker]="picker" formControlName="subscriptionEndsAt" />
            <mat-datepicker-toggle matSuffix [for]="picker" />
            <mat-datepicker #picker />
          </mat-form-field>

          <div style="display:flex;gap:12px;justify-content:flex-end">
            <button mat-button type="button" (click)="router.navigate(['/tenants'])">
              {{ 'common.cancel' | translate }}
            </button>
            <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid || loading">
              {{ 'common.save' | translate }}
            </button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
})
export class TenantFormComponent implements OnInit {
  form = this.fb.group({
    name: ['', Validators.required],
    code: ['', [Validators.required, Validators.pattern(/^[a-z0-9-]+$/)]],
    adminEmail: ['', [Validators.required, Validators.email]],
    adminMobile: ['', Validators.required],
    primaryColor: ['#1976D2'],
    secondaryColor: ['#424242'],
    packageId: ['', Validators.required],
    subscriptionEndsAt: ['', Validators.required],
  });

  isEdit = false;
  loading = false;
  packages: { id: string; name: string }[] = [];
  private tenantId?: string;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    public router: Router,
    private http: HttpClient,
    private store: Store,
  ) {}

  ngOnInit(): void {
    this.tenantId = this.route.snapshot.params['id'];
    this.isEdit = !!this.tenantId;

    this.http
      .get<{ items: { id: string; name: string }[] }>(`${environment.apiUrl}/packages`)
      .subscribe((r) => (this.packages = r.items));

    if (this.isEdit) {
      this.http
        .get<any>(`${environment.apiUrl}/tenants/${this.tenantId}`)
        .subscribe((t) => this.form.patchValue(t));
    }
  }

  submit(): void {
    if (this.form.invalid) return;
    const dto = this.form.value as any;
    if (this.isEdit) {
      this.store.dispatch(updateTenant({ id: this.tenantId!, dto }));
    } else {
      this.store.dispatch(createTenant({ dto }));
    }
    this.router.navigate(['/tenants']);
  }
}
