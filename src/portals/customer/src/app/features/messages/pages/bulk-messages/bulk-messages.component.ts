import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { environment } from '../../../../../environments/environment';

interface Employee {
  id: string;
  firstName: string;
  lastName: string;
  jobTitle: string;
}

@Component({
  selector: 'app-bulk-messages',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule, MatButtonModule, MatIconModule,
    MatFormFieldModule, MatInputModule, MatCheckboxModule,
    MatSelectModule, MatSnackBarModule, MatProgressBarModule,
    TranslateModule,
  ],
  template: `
    <h2>{{ 'messages.title' | translate }}</h2>

    <div style="display:grid;grid-template-columns:1fr 1fr;gap:24px;margin-top:16px">
      <!-- Employee selector -->
      <mat-card>
        <mat-card-header>
          <mat-card-title>{{ 'messages.select_employees' | translate }}</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div style="display:flex;gap:8px;margin-bottom:12px">
            <button mat-stroked-button (click)="selectAll()">{{ 'messages.select_all' | translate }}</button>
            <button mat-stroked-button (click)="clearAll()">{{ 'messages.clear' | translate }}</button>
          </div>
          @if (loading) { <mat-progress-bar mode="indeterminate"></mat-progress-bar> }
          <div style="max-height:400px;overflow-y:auto">
            @for (emp of employees; track emp.id) {
              <div style="display:flex;align-items:center;gap:8px;padding:4px 0">
                <mat-checkbox
                  [checked]="selected.has(emp.id)"
                  (change)="toggle(emp.id)">
                </mat-checkbox>
                <span>{{ emp.firstName }} {{ emp.lastName }} — {{ emp.jobTitle }}</span>
              </div>
            }
          </div>
          <p style="color:#666;margin-top:8px">{{ selected.size }} {{ 'messages.selected' | translate }}</p>
        </mat-card-content>
      </mat-card>

      <!-- Message form -->
      <div style="display:grid;gap:16px;align-content:start">
        <!-- Push notification -->
        <mat-card>
          <mat-card-header>
            <mat-card-title>{{ 'messages.push_notification' | translate }}</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <form [formGroup]="pushForm" (ngSubmit)="sendPush()" style="display:grid;gap:12px;margin-top:8px">
              <mat-form-field appearance="outline">
                <mat-label>{{ 'messages.title' | translate }}</mat-label>
                <input matInput formControlName="title" />
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>{{ 'messages.body' | translate }}</mat-label>
                <textarea matInput formControlName="body" rows="3"></textarea>
              </mat-form-field>
              <button mat-raised-button color="primary" type="submit" [disabled]="pushForm.invalid || !selected.size">
                <mat-icon>notifications</mat-icon> {{ 'messages.send' | translate }}
              </button>
            </form>
          </mat-card-content>
        </mat-card>

        <!-- Clarification request -->
        <mat-card>
          <mat-card-header>
            <mat-card-title>{{ 'messages.clarification_request' | translate }}</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <form [formGroup]="clarForm" (ngSubmit)="sendClarification()" style="display:grid;gap:12px;margin-top:8px">
              <mat-form-field appearance="outline">
                <mat-label>{{ 'messages.question' | translate }}</mat-label>
                <textarea matInput formControlName="question" rows="3"></textarea>
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>{{ 'messages.deadline' | translate }}</mat-label>
                <input matInput type="date" formControlName="deadline" />
              </mat-form-field>
              <button mat-raised-button color="accent" type="submit" [disabled]="clarForm.invalid || !selected.size">
                <mat-icon>help_outline</mat-icon> {{ 'messages.send_clarification' | translate }}
              </button>
            </form>
          </mat-card-content>
        </mat-card>
      </div>
    </div>
  `,
})
export class BulkMessagesComponent implements OnInit {
  employees: Employee[] = [];
  selected = new Set<string>();
  loading = false;

  pushForm = this.fb.group({
    title: ['', Validators.required],
    body: ['', Validators.required],
  });

  clarForm = this.fb.group({
    question: ['', [Validators.required, Validators.maxLength(2000)]],
    deadline: ['', Validators.required],
  });

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private snackbar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.loading = true;
    this.http.get<Employee[]>(`${environment.apiUrl}/employees`).subscribe({
      next: (e) => { this.employees = e; this.loading = false; },
      error: () => { this.loading = false; },
    });
  }

  toggle(id: string): void {
    if (this.selected.has(id)) this.selected.delete(id);
    else this.selected.add(id);
  }

  selectAll(): void { this.employees.forEach((e) => this.selected.add(e.id)); }
  clearAll(): void { this.selected.clear(); }

  sendPush(): void {
    const payload = { ...this.pushForm.value, employeeIds: [...this.selected] };
    this.http.post(`${environment.apiUrl}/notifications/bulk`, payload).subscribe({
      next: () => {
        this.snackbar.open('Notifications sent', 'OK', { duration: 3000 });
        this.pushForm.reset();
      },
      error: () => this.snackbar.open('Failed to send', 'OK', { duration: 3000 }),
    });
  }

  sendClarification(): void {
    const requests = [...this.selected].map((employeeId) => ({
      employeeId,
      ...this.clarForm.value,
    }));
    this.http.post(`${environment.apiUrl}/clarifications/bulk`, { requests }).subscribe({
      next: () => {
        this.snackbar.open('Clarification requests sent', 'OK', { duration: 3000 });
        this.clarForm.reset();
      },
      error: () => this.snackbar.open('Failed to send', 'OK', { duration: 3000 }),
    });
  }
}
