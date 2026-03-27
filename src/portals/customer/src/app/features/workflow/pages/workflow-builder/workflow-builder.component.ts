import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TranslateModule } from '@ngx-translate/core';
import { environment } from '../../../../../environments/environment';

interface WorkflowStep {
  order: number;
  approverId: string;
  approverName: string;
  role: string;
}

@Component({
  selector: 'app-workflow-builder',
  standalone: true,
  imports: [
    ReactiveFormsModule, DragDropModule,
    MatCardModule, MatButtonModule, MatIconModule,
    MatFormFieldModule, MatInputModule, MatSelectModule,
    MatSnackBarModule, TranslateModule,
  ],
  template: `
    <h2>{{ 'workflow.title' | translate }}</h2>

    <div style="display:grid;grid-template-columns:1fr 1fr;gap:24px;margin-top:16px">
      <!-- Step list (drag-reorder) -->
      <mat-card>
        <mat-card-header>
          <mat-card-title>{{ 'workflow.steps' | translate }}</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div cdkDropList (cdkDropListDropped)="drop($event)" style="min-height:80px">
            @for (step of steps; track step.order) {
              <div cdkDrag style="display:flex;align-items:center;gap:8px;padding:8px;border:1px solid #ddd;border-radius:4px;margin-bottom:8px;cursor:move;background:#fff">
                <mat-icon cdkDragHandle style="cursor:grab;color:#999">drag_indicator</mat-icon>
                <span style="flex:1">{{ step.order + 1 }}. {{ step.approverName }} ({{ step.role }})</span>
                <button mat-icon-button color="warn" (click)="removeStep(step)">
                  <mat-icon>delete</mat-icon>
                </button>
              </div>
            }
            @if (!steps.length) {
              <p style="color:#999;text-align:center;padding:16px">{{ 'workflow.empty' | translate }}</p>
            }
          </div>

          <button mat-raised-button color="primary" style="margin-top:16px" (click)="save()" [disabled]="!steps.length">
            {{ 'common.save' | translate }}
          </button>
        </mat-card-content>
      </mat-card>

      <!-- Add step form -->
      <mat-card>
        <mat-card-header>
          <mat-card-title>{{ 'workflow.add_step' | translate }}</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="addStep()" style="display:grid;gap:12px;margin-top:8px">
            <mat-form-field appearance="outline">
              <mat-label>{{ 'workflow.approver_name' | translate }}</mat-label>
              <input matInput formControlName="approverName" />
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>{{ 'workflow.approver_id' | translate }}</mat-label>
              <input matInput formControlName="approverId" />
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>{{ 'workflow.role' | translate }}</mat-label>
              <mat-select formControlName="role">
                <mat-option value="Manager">Manager</mat-option>
                <mat-option value="HR">HR</mat-option>
                <mat-option value="Director">Director</mat-option>
              </mat-select>
            </mat-form-field>
            <button mat-raised-button color="accent" type="submit" [disabled]="form.invalid">
              <mat-icon>add</mat-icon> {{ 'workflow.add_step' | translate }}
            </button>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
})
export class WorkflowBuilderComponent implements OnInit {
  steps: WorkflowStep[] = [];

  form = this.fb.group({
    approverName: ['', Validators.required],
    approverId: ['', Validators.required],
    role: ['Manager', Validators.required],
  });

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private snackbar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.http.get<WorkflowStep[]>(`${environment.apiUrl}/workflows/definition`).subscribe({
      next: (steps) => (this.steps = steps),
    });
  }

  drop(event: CdkDragDrop<WorkflowStep[]>): void {
    moveItemInArray(this.steps, event.previousIndex, event.currentIndex);
    this.steps = this.steps.map((s, i) => ({ ...s, order: i }));
  }

  addStep(): void {
    if (this.form.invalid) return;
    const v = this.form.value;
    this.steps.push({
      order: this.steps.length,
      approverId: v.approverId!,
      approverName: v.approverName!,
      role: v.role!,
    });
    this.form.reset({ role: 'Manager' });
  }

  removeStep(step: WorkflowStep): void {
    this.steps = this.steps
      .filter((s) => s !== step)
      .map((s, i) => ({ ...s, order: i }));
  }

  save(): void {
    this.http.put(`${environment.apiUrl}/workflows/definition`, { steps: this.steps }).subscribe({
      next: () => this.snackbar.open('Workflow saved', 'OK', { duration: 3000 }),
      error: () => this.snackbar.open('Save failed', 'OK', { duration: 3000 }),
    });
  }
}
