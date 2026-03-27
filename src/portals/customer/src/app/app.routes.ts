import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./core/auth/login/login.component').then((m) => m.LoginComponent),
  },
  {
    path: '',
    loadComponent: () =>
      import('./core/layout/shell/shell.component').then((m) => m.ShellComponent),
    canActivate: [authGuard],
    children: [
      {
        path: 'employees',
        loadComponent: () =>
          import('./features/employees/pages/employee-list/employee-list.component').then(
            (m) => m.EmployeeListComponent
          ),
      },
      {
        path: 'employees/new',
        loadComponent: () =>
          import('./features/employees/pages/employee-form/employee-form.component').then(
            (m) => m.EmployeeFormComponent
          ),
      },
      {
        path: 'leaves',
        loadComponent: () =>
          import('./features/leaves/pages/leave-queue/leave-queue.component').then(
            (m) => m.LeaveQueueComponent
          ),
      },
      {
        path: 'workflow',
        loadComponent: () =>
          import('./features/workflow/pages/workflow-builder/workflow-builder.component').then(
            (m) => m.WorkflowBuilderComponent
          ),
      },
      {
        path: 'reports',
        loadComponent: () =>
          import('./features/reports/pages/attendance-reports/attendance-reports.component').then(
            (m) => m.AttendanceReportsComponent
          ),
      },
      {
        path: 'messages',
        loadComponent: () =>
          import('./features/messages/pages/bulk-messages/bulk-messages.component').then(
            (m) => m.BulkMessagesComponent
          ),
      },
      { path: '', redirectTo: 'employees', pathMatch: 'full' },
    ],
  },
  { path: '**', redirectTo: 'employees' },
];
