import { Component, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { startWith } from 'rxjs';
import { loadShifts, Shift } from '../../store/shift.actions';
import { selectAllShifts } from '../../store/shift.reducer';

const DAY_KEYS = ['sun', 'mon', 'tue', 'wed', 'thu', 'fri', 'sat'];

@Component({
  selector: 'app-shift-list',
  standalone: true,
  imports: [
    AsyncPipe, RouterLink,
    MatTableModule, MatButtonModule, MatIconModule, MatChipsModule,
    MatProgressBarModule, TranslateModule,
  ],
  template: `
    <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:16px">
      <h2>{{ 'shifts.title' | translate }}</h2>
      <a mat-raised-button color="primary" routerLink="/shifts/new">
        <mat-icon>add</mat-icon> {{ 'shifts.add' | translate }}
      </a>
    </div>

    @if (loading$ | async) {
      <mat-progress-bar mode="indeterminate" style="margin-bottom:8px"></mat-progress-bar>
    }

    <mat-table [dataSource]="(shifts$ | async) ?? []">
      <ng-container matColumnDef="name">
        <mat-header-cell *matHeaderCellDef>{{ 'shifts.name' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let s">
          {{ s.name }}
          @if (s.isSplitShift) {
            <mat-chip style="margin-inline-start:8px" color="accent">{{ 'shifts.split' | translate }}</mat-chip>
          }
        </mat-cell>
      </ng-container>

      <ng-container matColumnDef="time">
        <mat-header-cell *matHeaderCellDef>{{ 'shifts.time_range' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let s">
          @if (s.isSplitShift && s.firstStartTime && s.secondEndTime) {
            <div>{{ shortTime(s.firstStartTime) }} - {{ shortTime(s.firstEndTime) }}</div>
            <div>{{ shortTime(s.secondStartTime) }} - {{ shortTime(s.secondEndTime) }}</div>
          } @else {
            {{ shortTime(s.startTime) }} - {{ shortTime(s.endTime) }}
          }
        </mat-cell>
      </ng-container>

      <ng-container matColumnDef="days">
        <mat-header-cell *matHeaderCellDef>{{ 'shifts.days_label' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let s">{{ daysLabel(s.daysOfWeekJson) }}</mat-cell>
      </ng-container>

      <ng-container matColumnDef="radius">
        <mat-header-cell *matHeaderCellDef>{{ 'shifts.radius' | translate }}</mat-header-cell>
        <mat-cell *matCellDef="let s">{{ s.radiusMeters }} {{ 'shifts.meters' | translate }}</mat-cell>
      </ng-container>

      <ng-container matColumnDef="actions">
        <mat-header-cell *matHeaderCellDef></mat-header-cell>
        <mat-cell *matCellDef="let s">
          <a mat-icon-button color="primary" [routerLink]="['/shifts', s.id, 'edit']">
            <mat-icon>edit</mat-icon>
          </a>
        </mat-cell>
      </ng-container>

      <mat-header-row *matHeaderRowDef="columns"></mat-header-row>
      <mat-row *matRowDef="let row; columns: columns;"></mat-row>
    </mat-table>

    @if ((shifts$ | async)?.length === 0 && !(loading$ | async)) {
      <p style="text-align:center;color:#888;margin-top:24px">{{ 'common.no_data' | translate }}</p>
    }
  `,
})
export class ShiftListComponent implements OnInit {
  columns = ['name', 'time', 'days', 'radius', 'actions'];

  shifts$ = this.store.select((s: any) => selectAllShifts(s.shifts));
  loading$ = this.store.select((s: any) => s.shifts.loading);

  // Rebuilt whenever translations (re)load or the user switches language via the shell's
  // language menu, so day labels don't go stale like a one-time translate.instant() snapshot
  // would.
  private dayLabels: string[] = DAY_KEYS.map((k) => 'shifts.days.' + k);

  constructor(private store: Store, private translate: TranslateService) {
    this.translate.onLangChange
      .pipe(startWith(null))
      .subscribe(() => {
        this.dayLabels = DAY_KEYS.map((k) => this.translate.instant('shifts.days.' + k));
      });
  }

  ngOnInit(): void {
    this.store.dispatch(loadShifts());
  }

  shortTime(value?: string | null): string {
    return value ? value.substring(0, 5) : '';
  }

  daysLabel(daysJson: string): string {
    try {
      const days: number[] = JSON.parse(daysJson);
      if (!days || days.length === 0 || days.length === 7) {
        return this.translate.instant('shifts.every_day');
      }
      return days
        .slice()
        .sort((a, b) => a - b)
        .map((d) => this.dayLabels[d])
        .join(', ');
    } catch {
      return this.translate.instant('shifts.every_day');
    }
  }
}
