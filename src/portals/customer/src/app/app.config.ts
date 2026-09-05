import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideHttpClient, withInterceptors, HttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { routes } from './app.routes';
import { authInterceptor } from './core/auth/auth.interceptor';
import { employeeReducer } from './features/employees/store/employee.reducer';
import { EmployeeEffects } from './features/employees/store/employee.effects';
import { leaveReducer } from './features/leaves/store/leave.reducer';
import { LeaveEffects } from './features/leaves/store/leave.effects';
import { shiftReducer } from './features/shifts/store/shift.reducer';
import { ShiftEffects } from './features/shifts/store/shift.effects';
import { auditReducer } from './features/audit/store/audit.reducer';
import { AuditEffects } from './features/audit/store/audit.effects';

export function httpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAnimations(),
    provideStore({
      employees: employeeReducer,
      leaves: leaveReducer,
      shifts: shiftReducer,
      audit: auditReducer,
    }),
    provideEffects([EmployeeEffects, LeaveEffects, ShiftEffects, AuditEffects]),
    importProvidersFrom(
      TranslateModule.forRoot({
        loader: { provide: TranslateLoader, useFactory: httpLoaderFactory, deps: [HttpClient] },
        defaultLanguage: 'ar',
      }),
    ),
  ],
};
