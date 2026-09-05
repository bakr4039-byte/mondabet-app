import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { filter, take } from 'rxjs';
import { createShift, loadShifts, updateShift, Shift, ShiftUpsertDto } from '../../store/shift.actions';
import { selectShiftEntities } from '../../store/shift.reducer';

const DAY_KEYS = ['sun', 'mon', 'tue', 'wed', 'thu', 'fri', 'sat'];

@Component({
  selector: 'app-shift-form',
  standalone: true,
  imports: [
    CommonModule, RouterLink, ReactiveFormsModule,
    MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule,
    MatCheckboxModule, MatSlideToggleModule, MatIconModule, TranslateModule,
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>{{ (isEdit ? 'shifts.edit' : 'shifts.add') | translate }}</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <form [formGroup]="form" (ngSubmit)="submit()" style="display:grid;gap:16px;max-width:640px">
          <mat-form-field appearance="outline">
            <mat-label>{{ 'shifts.name' | translate }}</mat-label>
            <input matInput formControlName="name" />
          </mat-form-field>

          <div style="display:flex;gap:16px;flex-wrap:wrap">
            <mat-form-field appearance="outline">
              <mat-label>{{ 'shifts.start_time' | translate }}</mat-label>
              <input matInput type="time" formControlName="startTime" />
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>{{ 'shifts.end_time' | translate }}</mat-label>
              <input matInput type="time" formControlName="endTime" />
            </mat-form-field>
          </div>

          <div>
            <mat-slide-toggle formControlName="isSplitShift">
              {{ 'shifts.is_split' | translate }}
            </mat-slide-toggle>
            <p style="color:#888;font-size:12px;margin-top:4px">{{ 'shifts.split_hint' | translate }}</p>
          </div>

          @if (form.value.isSplitShift) {
            <div style="border:1px solid #e0e0e0;border-radius:8px;padding:16px;display:grid;gap:12px">
              <strong>{{ 'shifts.first_period' | translate }}</strong>
              <div style="display:flex;gap:16px;flex-wrap:wrap">
                <mat-form-field appearance="outline">
                  <mat-label>{{ 'shifts.start_time' | translate }}</mat-label>
                  <input matInput type="time" formControlName="firstStartTime" />
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>{{ 'shifts.end_time' | translate }}</mat-label>
                  <input matInput type="time" formControlName="firstEndTime" />
                </mat-form-field>
              </div>
              <strong>{{ 'shifts.second_period' | translate }}</strong>
              <div style="display:flex;gap:16px;flex-wrap:wrap">
                <mat-form-field appearance="outline">
                  <mat-label>{{ 'shifts.start_time' | translate }}</mat-label>
                  <input matInput type="time" formControlName="secondStartTime" />
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>{{ 'shifts.end_time' | translate }}</mat-label>
                  <input matInput type="time" formControlName="secondEndTime" />
                </mat-form-field>
              </div>
            </div>
          }

          <div style="display:flex;gap:16px;flex-wrap:wrap">
            <mat-form-field appearance="outline">
              <mat-label>{{ 'shifts.latitude' | translate }}</mat-label>
              <input matInput type="number" step="any" formControlName="latitude" />
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>{{ 'shifts.longitude' | translate }}</mat-label>
              <input matInput type="number" step="any" formControlName="longitude" />
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>{{ 'shifts.radius' | translate }}</mat-label>
              <input matInput type="number" formControlName="radiusMeters" />
            </mat-form-field>
          </div>

          <div>
            <mat-label style="display:block;margin-bottom:8px">{{ 'shifts.days_label' | translate }}</mat-label>
            <div style="display:flex;gap:8px;flex-wrap:wrap" formArrayName="days">
              @for (day of daysArray.controls; track $index) {
                <mat-checkbox [formControlName]="$index">
                  {{ 'shifts.days.' + dayKeys[$index] | translate }}
                </mat-checkbox>
              }
            </div>
          </div>

          <div style="display:flex;gap:16px;flex-wrap:wrap">
            <mat-form-field appearance="outline">
              <mat-label>{{ 'shifts.grace_minutes' | translate }}</mat-label>
              <input matInput type="number" formControlName="gracePeriodMinutes" />
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>{{ 'shifts.window_start_minutes' | translate }}</mat-label>
              <input matInput type="number" formControlName="windowStartMinutes" />
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>{{ 'shifts.window_end_minutes' | translate }}</mat-label>
              <input matInput type="number" formControlName="windowEndMinutes" />
            </mat-form-field>
          </div>

          <div style="display:flex;gap:8px;justify-content:flex-end">
            <button mat-button type="button" routerLink="/shifts">{{ 'common.cancel' | translate }}</button>
            <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">
              {{ 'common.save' | translate }}
            </button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
})
export class ShiftFormComponent implements OnInit {
  dayKeys = DAY_KEYS;
  isEdit = false;
  private shiftId: string | null = null;

  form = this.fb.group({
    name: ['', Validators.required],
    startTime: ['08:00', Validators.required],
    endTime: ['16:00', Validators.required],
    isSplitShift: [false],
    firstStartTime: ['08:00'],
    firstEndTime: ['12:00'],
    secondStartTime: ['13:00'],
    secondEndTime: ['16:00'],
    latitude: [0, Validators.required],
    longitude: [0, Validators.required],
    radiusMeters: [100, [Validators.required, Validators.min(1)]],
    days: this.fb.array(DAY_KEYS.map(() => true)),
    gracePeriodMinutes: [15, [Validators.required, Validators.min(0)]],
    windowStartMinutes: [60, [Validators.required, Validators.min(0)]],
    windowEndMinutes: [60, [Validators.required, Validators.min(0)]],
  });

  get daysArray(): FormArray {
    return this.form.get('days') as FormArray;
  }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private store: Store,
  ) {}

  ngOnInit(): void {
    this.shiftId = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.shiftId;

    if (this.shiftId) {
      this.store.dispatch(loadShifts());
      this.store
        .select((s: any) => selectShiftEntities(s.shifts))
        .pipe(
          filter((entities) => !!this.shiftId && !!entities[this.shiftId]),
          take(1),
        )
        .subscribe((entities) => this.patchForm(entities[this.shiftId!] as Shift));
    }
  }

  private patchForm(shift: Shift): void {
    this.form.patchValue({
      name: shift.name,
      startTime: this.toTimeInput(shift.startTime),
      endTime: this.toTimeInput(shift.endTime),
      isSplitShift: shift.isSplitShift,
      firstStartTime: this.toTimeInput(shift.firstStartTime) || '08:00',
      firstEndTime: this.toTimeInput(shift.firstEndTime) || '12:00',
      secondStartTime: this.toTimeInput(shift.secondStartTime) || '13:00',
      secondEndTime: this.toTimeInput(shift.secondEndTime) || '16:00',
      latitude: shift.latitude,
      longitude: shift.longitude,
      radiusMeters: shift.radiusMeters,
      gracePeriodMinutes: shift.gracePeriodMinutes,
      windowStartMinutes: shift.windowStartMinutes,
      windowEndMinutes: shift.windowEndMinutes,
    });

    try {
      const days: number[] = JSON.parse(shift.daysOfWeekJson || '[]');
      DAY_KEYS.forEach((_, i) => this.daysArray.at(i).setValue(days.includes(i)));
    } catch {
      // leave default (all days selected) if the stored JSON is unexpectedly malformed
    }
  }

  private toTimeInput(value?: string | null): string {
    return value ? value.substring(0, 5) : '';
  }

  private toApiTime(value: string): string {
    return value ? `${value}:00` : '00:00:00';
  }

  submit(): void {
    if (this.form.invalid) return;
    const v = this.form.value;

    const selectedDays = this.daysArray.controls
      .map((c, i) => (c.value ? i : -1))
      .filter((i) => i >= 0);

    const dto: ShiftUpsertDto = {
      name: v.name!,
      startTime: this.toApiTime(v.startTime!),
      endTime: this.toApiTime(v.endTime!),
      latitude: v.latitude!,
      longitude: v.longitude!,
      radiusMeters: v.radiusMeters!,
      daysOfWeekJson: JSON.stringify(selectedDays),
      gracePeriodMinutes: v.gracePeriodMinutes!,
      windowStartMinutes: v.windowStartMinutes!,
      windowEndMinutes: v.windowEndMinutes!,
      isSplitShift: !!v.isSplitShift,
      firstStartTime: v.isSplitShift ? this.toApiTime(v.firstStartTime!) : null,
      firstEndTime: v.isSplitShift ? this.toApiTime(v.firstEndTime!) : null,
      secondStartTime: v.isSplitShift ? this.toApiTime(v.secondStartTime!) : null,
      secondEndTime: v.isSplitShift ? this.toApiTime(v.secondEndTime!) : null,
    };

    if (this.isEdit && this.shiftId) {
      this.store.dispatch(updateShift({ id: this.shiftId, dto }));
    } else {
      this.store.dispatch(createShift({ dto }));
    }
    this.router.navigate(['/shifts']);
  }
}
