import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Store } from '@ngrx/store';
import { MatTabsModule } from '@angular/material/tabs';
import { PageEvent } from '@angular/material/paginator';
import { TranslateModule } from '@ngx-translate/core';
import { AuditFilter, AuditSource, loadAuditLogs } from '../../store/audit.actions';
import { selectAuditSource } from '../../store/audit.reducer';
import { AuditPanelComponent } from './audit-panel.component';

const DEFAULT_SIZE = 20;

@Component({
  selector: 'app-audit-log',
  standalone: true,
  imports: [MatTabsModule, TranslateModule, AuditPanelComponent],
  template: `
    <h2>{{ 'audit.title' | translate }}</h2>

    <mat-tab-group (selectedTabChange)="onTabChange($event.index)">
      <mat-tab [label]="'audit.sources.identity' | translate">
        <app-audit-panel
          [filterForm]="identityFilter"
          [state$]="identityState$"
          (search)="search('identity', identityFilter)"
          (pageEvent)="pageChange('identity', identityFilter, $event)"
        ></app-audit-panel>
      </mat-tab>
      <mat-tab [label]="'audit.sources.employees' | translate">
        <app-audit-panel
          [filterForm]="employeesFilter"
          [state$]="employeesState$"
          (search)="search('employees', employeesFilter)"
          (pageEvent)="pageChange('employees', employeesFilter, $event)"
        ></app-audit-panel>
      </mat-tab>
      <mat-tab [label]="'audit.sources.leaves' | translate">
        <app-audit-panel
          [filterForm]="leavesFilter"
          [state$]="leavesState$"
          (search)="search('leaves', leavesFilter)"
          (pageEvent)="pageChange('leaves', leavesFilter, $event)"
        ></app-audit-panel>
      </mat-tab>
    </mat-tab-group>
  `,
})
export class AuditLogComponent implements OnInit {
  identityFilter = this.fb.group({ action: [''], from: [''], to: [''] });
  employeesFilter = this.fb.group({ action: [''], from: [''], to: [''] });
  leavesFilter = this.fb.group({ action: [''], from: [''], to: [''] });

  identityState$ = this.store.select((s: any) => selectAuditSource(s.audit, 'identity'));
  employeesState$ = this.store.select((s: any) => selectAuditSource(s.audit, 'employees'));
  leavesState$ = this.store.select((s: any) => selectAuditSource(s.audit, 'leaves'));

  private loadedTabs = new Set<AuditSource>();
  private tabOrder: AuditSource[] = ['identity', 'employees', 'leaves'];

  constructor(private fb: FormBuilder, private store: Store) {}

  ngOnInit(): void {
    this.loadTab('identity', this.identityFilter, 1);
  }

  onTabChange(index: number): void {
    const source = this.tabOrder[index];
    const filterForm = source === 'identity' ? this.identityFilter
      : source === 'employees' ? this.employeesFilter
      : this.leavesFilter;
    this.loadTab(source, filterForm, 1);
  }

  search(source: AuditSource, filterForm: FormGroup): void {
    this.dispatchLoad(source, filterForm, 1, DEFAULT_SIZE);
  }

  pageChange(source: AuditSource, filterForm: FormGroup, event: PageEvent): void {
    this.dispatchLoad(source, filterForm, event.pageIndex + 1, event.pageSize);
  }

  private loadTab(source: AuditSource, filterForm: FormGroup, page: number): void {
    if (this.loadedTabs.has(source)) return;
    this.loadedTabs.add(source);
    this.dispatchLoad(source, filterForm, page, DEFAULT_SIZE);
  }

  private dispatchLoad(source: AuditSource, filterForm: FormGroup, page: number, size: number): void {
    const v = filterForm.value as { action?: string; from?: string; to?: string };
    const filter: AuditFilter = {
      page,
      size,
      action: v.action || undefined,
      from: v.from || undefined,
      to: v.to || undefined,
    };
    this.store.dispatch(loadAuditLogs({ source, filter }));
  }
}
