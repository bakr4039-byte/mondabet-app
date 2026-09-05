import { Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    NgIf, ReactiveFormsModule, MatCardModule, MatFormFieldModule,
    MatInputModule, MatButtonModule, TranslateModule,
  ],
  template: `
    <div class="login-wrapper">
      <mat-card style="width:360px">
        <mat-card-header>
          <mat-card-title>{{ 'auth.login' | translate }}</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="submit()">
            <mat-form-field appearance="outline" style="width:100%">
              <mat-label>{{ 'auth.username' | translate }}</mat-label>
              <input matInput formControlName="identifier" autocomplete="username" />
            </mat-form-field>
            <mat-form-field appearance="outline" style="width:100%">
              <mat-label>{{ 'auth.password' | translate }}</mat-label>
              <input matInput type="password" formControlName="password" autocomplete="current-password" />
            </mat-form-field>
            <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid || loading" style="width:100%">
              {{ 'auth.sign_in' | translate }}
            </button>
            <p *ngIf="error" style="color:red;margin-top:8px">{{ error }}</p>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .login-wrapper { display:flex; justify-content:center; align-items:center; height:100vh; background:#f5f5f5; }
  `],
})
export class LoginComponent {
  form = this.fb.group({
    identifier: ['', Validators.required],
    password: ['', Validators.required],
  });
  loading = false;
  error = '';

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router,
  ) {}

  submit(): void {
    if (this.form.invalid) return;
    this.loading = true;
    this.error = '';

    // Note: every backend response is wrapped in an envelope by ResultExtensions.ToApiResult:
    // { data: T, error: {...} | null, traceId: string }. The actual payload lives under "data".
    interface LoginData {
      accessToken: string | null;
      refreshToken: string | null;
      mfaRequired: boolean;
      sessionToken: string | null;
    }
    interface AuthTokensData {
      accessToken: string;
      refreshToken: string;
    }
    interface ApiEnvelope<T> {
      data: T;
      error: { code: string; message: string } | null;
      traceId: string;
    }

    this.http
      .post<ApiEnvelope<LoginData>>(
        `${environment.apiUrl}/auth/login`,
        {
          identifier: this.form.value.identifier,
          password: this.form.value.password,
          tenantCode: 'superadmin',
        },
      )
      .subscribe({
        next: (res) => {
          const data = res.data;
          if (data.mfaRequired && data.sessionToken) {
            // TODO: replace with a proper OTP entry screen. The backend enforces
            // mandatory OTP verification (dev: check the Identity service console
            // for a line like "MFA session ... OTP: 123456").
            const otp = window.prompt('Enter the OTP code (see Identity service console log)');
            if (!otp) {
              this.loading = false;
              this.error = 'OTP is required to complete sign-in.';
              return;
            }
            this.http
              .post<ApiEnvelope<AuthTokensData>>(`${environment.apiUrl}/auth/mfa/verify`, {
                sessionToken: data.sessionToken,
                otp,
              })
              .subscribe({
                next: (verifyRes) => {
                  localStorage.setItem('access_token', verifyRes.data.accessToken);
                  localStorage.setItem('refresh_token', verifyRes.data.refreshToken);
                  this.router.navigate(['/tenants']);
                },
                error: () => {
                  this.loading = false;
                  this.error = 'Invalid or expired OTP.';
                },
              });
            return;
          }

          if (data.accessToken) {
            localStorage.setItem('access_token', data.accessToken);
            if (data.refreshToken) localStorage.setItem('refresh_token', data.refreshToken);
            this.router.navigate(['/tenants']);
          } else {
            this.loading = false;
            this.error = 'Login failed.';
          }
        },
        error: () => {
          this.loading = false;
          this.error = 'Invalid credentials';
        },
      });
  }
}
