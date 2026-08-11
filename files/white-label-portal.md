# White-Label SuperAdmin Portal – Angular Spec

## Architecture
```
src/app/
├── core/
│   ├── auth/           # AuthGuard, TokenInterceptor, RoleGuard
│   ├── services/       # ApiService (base), TenantService
│   └── store/          # NgRx root store
├── features/
│   ├── tenants/        # CRUD company management
│   ├── packages/       # Subscription package management
│   ├── reports/        # Company-level analytics
│   └── users/          # SuperAdmin user management
└── shared/
    ├── components/     # DataTable, ConfirmDialog, FileUpload
    └── pipes/          # DateAr, CurrencySar
```

## Module: Tenant Management
Route: /tenants
Features:
- List with search, filter (active/inactive), pagination
- Create form fields (all TenantCreateDto fields):
  - Company Name, Logo upload (MinIO), Primary/Secondary color pickers
  - Slogan, Address, National Address (Saudi API typeahead)
  - Google Maps picker (lat/lng from map click)
  - Bank Account (optional), Zakat number (optional)
  - Admin Email + Mobile, Subscription End Date
  - Package selector (dropdown from GET /packages)
- Edit tenant (same form)
- Toggle active/inactive
- View tenant stats: active users count, last login, subscription status

## Module: Packages
Route: /packages
- CRUD for subscription packages (name, maxUsers, priceMonthly, features JSON editor)

## Module: Reports (SuperAdmin)
Route: /reports
- Table: all tenants → columns: name, users count, active status, subscription dates
- Chart: bar chart users per tenant (ngx-charts)
- Export PDF/Excel buttons → GET /reports/companies?format=pdf|xlsx

## Module: Users (SuperAdmin)
- CRUD SuperAdmin users with role assignment

## White-label Config
- SuperAdmin portal uses fixed branding (Mondabet brand), not tenant colors
- Tenants table shows tenant logo thumbnails from MinIO pre-signed URL

## Tech
- Angular 18 standalone components
- NgRx 18 (store per feature)
- Angular Material 18 UI components
- ngx-translate (AR/EN/UR)
- ngx-charts for analytics
- Reactive Forms with custom validators (Iqama pattern, Saudi mobile)
- Interceptors: JWT attach, 401 → redirect to login, error toast
- RTL/LTR toggle: dir attribute on <html> driven by language selection
