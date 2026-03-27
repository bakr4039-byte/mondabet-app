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
        path: 'tenants',
        loadComponent: () =>
          import('./features/tenants/pages/tenant-list/tenant-list.component').then(
            (m) => m.TenantListComponent
          ),
      },
      {
        path: 'tenants/new',
        loadComponent: () =>
          import('./features/tenants/pages/tenant-form/tenant-form.component').then(
            (m) => m.TenantFormComponent
          ),
      },
      {
        path: 'tenants/:id/edit',
        loadComponent: () =>
          import('./features/tenants/pages/tenant-form/tenant-form.component').then(
            (m) => m.TenantFormComponent
          ),
      },
      {
        path: 'packages',
        loadComponent: () =>
          import('./features/packages/pages/package-list/package-list.component').then(
            (m) => m.PackageListComponent
          ),
      },
      {
        path: 'reports',
        loadComponent: () =>
          import('./features/reports/pages/company-reports/company-reports.component').then(
            (m) => m.CompanyReportsComponent
          ),
      },
      { path: '', redirectTo: 'tenants', pathMatch: 'full' },
    ],
  },
  { path: '**', redirectTo: 'tenants' },
];
