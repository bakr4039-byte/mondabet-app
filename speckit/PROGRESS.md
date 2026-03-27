# Mondabet Development Progress Tracker

> Update this file after each checkpoint is completed.
> Status: NOT_STARTED | IN_PROGRESS | DONE

## Sprint 1: Infrastructure + Auth

| # | Checkpoint | Spec Source | Status | Notes |
|---|-----------|-------------|--------|-------|
| 1 | Monorepo Structure | `arch/system-overview.md`, `arch/infra.md` | DONE | .gitignore, .editorconfig, global.json, sln, Directory.Build.props |
| 2 | Docker Compose | `arch/infra.md` | DONE | SQL Server, Redis, RabbitMQ, MinIO, Keycloak |
| 3 | Shared .NET Library | `arch/system-overview.md` | DONE | Result<T>, BaseEntity, TenantMiddleware, BaseDbContext |
| 4 | Identity Domain + Application | `arch/backend-contracts.md`, `arch/security.md` | DONE | User, RefreshToken, BiometricDevice entities; Login/MFA/Biometric/Nafath/Refresh/Logout CQRS |
| 5 | Identity Infrastructure | `arch/backend-contracts.md`, `arch/db-schema.md` | DONE | EF Core IdentityDbContext, Repositories, JWT/OTP/Biometric/Keycloak/Nafath services |
| 6 | Identity API + Dockerfile | `arch/backend-contracts.md`, `arch/infra.md` | DONE | Minimal API AuthEndpoints, Program.cs, appsettings.json, Dockerfile |
| 7 | Keycloak Configuration | `arch/security.md` | DONE | infra/keycloak/mondabet-realm.json – realm, 5 clients, 3 roles, RS256 |
| 8 | YARP Gateway | `arch/system-overview.md`, `arch/security.md` | DONE | Program.cs, appsettings with all 8 service routes, rate-limit policies, CORS, JWT validation, Dockerfile |
| 9 | Flutter Mobile Auth | `mobile/flutter-spec.md`, `mobile/l10n.md` | DONE | pubspec, DI, go_router, AuthBloc (5 events), Login/MFA/Splash screens, AR+EN+UR translations, BLoC tests |
| 10 | Tests + E2E Smoke | `testing/test-strategy.md` | DONE | 7 unit tests passing – LoginCommandHandler (4), RefreshTokenCommandHandler (3) |

## Sprint 2: Tenant + Employee

| # | Checkpoint | Spec Source | Status | Notes |
|---|-----------|-------------|--------|-------|
| 1 | Tenant CRUD API | `arch/backend-contracts.md` | DONE | Domain, Application (CQRS), Infrastructure, API endpoints |
| 2 | Package CRUD API | `arch/backend-contracts.md` | DONE | Package entity, CreatePackage command, ListPackages query, endpoints |
| 3 | Per-tenant schema provisioning | `arch/db-schema.md` | DONE | EmployeeDbContext uses BaseDbContext per-tenant schema via TenantMiddleware |
| 4 | Employee CRUD API | `arch/backend-contracts.md` | DONE | Full CRUD + GetPaged with search, soft-delete |
| 5 | Excel import | `portals/customer-portal.md` | DONE | ClosedXML, IExcelImportService, ImportEmployeesCommand, /import endpoint |
| 6 | SuperAdmin Portal scaffold | `portals/white-label-portal.md` | NOT_STARTED | Angular 18 |
| 7 | Customer Portal scaffold | `portals/customer-portal.md` | NOT_STARTED | Angular 18 |

## Sprint 3: Attendance Service

| # | Checkpoint | Spec Source | Status | Notes |
|---|-----------|-------------|--------|-------|
| 1 | Shift CRUD API | `arch/backend-contracts.md` | DONE | Shift entity, CreateShift/UpdateShift commands, ListShifts query |
| 2 | Check-in API + geofence validation | `arch/backend-contracts.md`, `CLAUDE.md §8` | DONE | CheckInCommand + haversine GeofenceService |
| 3 | Check-out API | `arch/backend-contracts.md` | DONE | CheckOutCommand, open check-in lookup |
| 4 | Attendance list + summary API | `arch/backend-contracts.md` | DONE | ListCheckIns (paged), GetAttendanceSummary |
| 5 | Duplicate check-in guard | `backlog/jira-backlog.md` ATT-08 | DONE | HasCheckInTodayAsync |
| 6 | Geofence unit tests (ATT-07 P0) | `testing/test-strategy.md` | DONE | 6 tests: IsWithinGeofence + HaversineDistance |

## Sprint 4: Leave Management Service

| # | Checkpoint | Spec Source | Status | Notes |
|---|-----------|-------------|--------|-------|
| 1 | LeaveRequest domain entity | `arch/db-schema.md` | DONE | LeaveType/Status enums, Approve/Reject with domain events |
| 2 | Leave attachment entity | `arch/db-schema.md` | DONE | LeaveAttachment (MinIO file ref) |
| 3 | Submit leave (Vacation/Permission/Excuse) | `arch/backend-contracts.md` | DONE | SubmitLeaveCommand, single unified endpoint |
| 4 | Approve/Reject leave | `arch/backend-contracts.md` | DONE | Guard: only Pending leaves can be actioned |
| 5 | Leave list + get | `arch/backend-contracts.md` | DONE | Paged, filtered by type/status/employeeId |
| 6 | Domain events | `arch/system-overview.md` | DONE | LeaveApprovedEvent, LeaveRejectedEvent (ready for MassTransit) |

## Sprint 5: Notification Service

| # | Checkpoint | Spec Source | Status | Notes |
|---|-----------|-------------|--------|-------|
| 1 | Unifonic SMS adapter (NOTIF-01) | `backlog/jira-backlog.md` | DONE | UnifoncSmsService with HttpClient, bulk via comma-sep recipients |
| 2 | FCM push adapter (NOTIF-02) | `backlog/jira-backlog.md` | DONE | FcmPushService v1 API, bulk via Task.WhenAll |
| 3 | Bulk SMS/push endpoints (NOTIF-03) | `backlog/jira-backlog.md` | DONE | /notifications/sms, /push, /sms/bulk, /push/bulk |

## Sprint 6: Report Service

| # | Checkpoint | Spec Source | Status | Notes |
|---|-----------|-------------|--------|-------|
| 1 | Attendance PDF report (RPT-01/RPT-06) | `backlog/jira-backlog.md` | DONE | QuestPDF landscape table with header/footer/pagination |
| 2 | Attendance Excel report (RPT-02/RPT-07) | `backlog/jira-backlog.md` | DONE | ClosedXML workbook with auto-column sizing |
| 3 | Company summary PDF + Excel (RPT-03) | `backlog/jira-backlog.md` | DONE | SuperAdmin only |
| 4 | Report API endpoints | `arch/backend-contracts.md` | DONE | /reports/attendance?format=pdf\|xlsx, /reports/companies |

## Sprint 7: Workflow + Clarification

| # | Checkpoint | Spec Source | Status | Notes |
|---|-----------|-------------|--------|-------|
| 1 | WorkflowDefinition entity + CRUD | `arch/db-schema.md`, `arch/backend-contracts.md` | DONE | WorkflowAppliesTo enum, StepsJson, CreateWorkflow/UpdateWorkflow/ListWorkflows |
| 2 | WorkflowInstance entity + Advance/Reject | `arch/system-overview.md §9` | DONE | Advance() increments step or fires WorkflowCompletedEvent; Reject() fires WorkflowRejectedEvent |
| 3 | StartWorkflow command | `arch/backend-contracts.md` | DONE | Counts steps from StepsJson, creates instance |
| 4 | AdvanceWorkflow command | `arch/backend-contracts.md` | DONE | Guard: only Active instances; optional Reject dto |
| 5 | MassTransit LeaveRequestedConsumer | `arch/system-overview.md §9` | DONE | Receives LeaveRequestedMessage, starts workflow instance |
| 6 | Clarification domain entity | `arch/db-schema.md` | DONE | ClarificationStatus enum, Respond() method |
| 7 | Clarification Application (CQRS) | `arch/backend-contracts.md` | DONE | Create/Respond commands + ListClarifications query |
| 8 | Clarification Infrastructure | `arch/db-schema.md` | DONE | ClarificationDbContext, EF configs, repository, DI |
| 9 | Clarification API + Dockerfile | `arch/backend-contracts.md` | DONE | POST /, GET /employee/{id}, PUT /{id}/respond |

## Sprint 8: Flutter Mobile Completion

| # | Checkpoint | Spec Source | Status | Notes |
|---|-----------|-------------|--------|-------|
| 1 | White-label theming (TenantConfig + ThemeService) | `mobile/flutter-spec.md` | DONE | Hive persistence; ThemeData from hex colors |
| 2 | Shift + AttendanceRecord entities | `arch/db-schema.md` | DONE | Haversine geofence client-side in AttendanceBloc |
| 3 | Check-in screen (Google Maps + geofence) | `mobile/flutter-spec.md` | DONE | Live position stream; button disabled when outside radius |
| 4 | Leave screens (Vacation / Permission / Excuse) | `mobile/flutter-spec.md` | DONE | FilePicker for excuse attachment; TabController list |
| 5 | Reports screen (BarChart + download) | `mobile/flutter-spec.md` | DONE | fl_chart BarChart; Dio download to temp dir |
| 6 | FCM v1 push notifications | `backlog/jira-backlog.md` NOTIF-02 | DONE | Firebase + flutter_local_notifications foreground handler |
| 7 | Clarification response screen | `arch/backend-contracts.md` | DONE | Shows question; text field + submit if pending |
| 8 | Home screen NavigationBar + routing | `mobile/flutter-spec.md` | DONE | 4 tabs; go_router routes; MultiBlocProvider |
| 9 | Full DI + i18n (AR/EN/UR extended) | `mobile/l10n.md` | DONE | GetIt; all new keys added to 3 locales |
| 10 | BLoC tests (Attendance + Leave) | `testing/test-strategy.md` | DONE | 7 blocTest cases |

## Sprint 9: SuperAdmin Angular Portal

| # | Checkpoint | Spec Source | Status | Notes |
|---|-----------|-------------|--------|-------|
| 1 | Package setup (Angular 18, NgRx, Jest) | `portals/white-label-portal.md` | DONE | package.json; app.config.ts with provideStore |
| 2 | Auth interceptor + guard | `arch/security.md` | DONE | HttpInterceptorFn; 401 → /login redirect |
| 3 | Shell layout (sidenav + lang switcher) | `portals/white-label-portal.md` | DONE | RTL/LTR dir toggle; logout |
| 4 | Tenant NgRx slice (reducer + effects) | `arch/backend-contracts.md` | DONE | Entity adapter; load/create/update effects |
| 5 | Tenant list + form components | `portals/white-label-portal.md` | DONE | Search filter; create/edit form with package selector |
| 6 | Package NgRx slice | `arch/backend-contracts.md` | DONE | loadPackages effect |
| 7 | Reports page (charts + table) | `portals/white-label-portal.md` | DONE | ngx-charts bar; PDF/Excel export via window.open |
| 8 | Tenant reducer unit tests | `testing/test-strategy.md` | DONE | 6 Jest tests |

## Sprint 10: Customer Portal

| # | Checkpoint | Spec Source | Status | Notes |
|---|-----------|-------------|--------|-------|
| 1 | App config + routes + interceptor | `portals/customer-portal.md` | DONE | provideStore; lazy routes for all features |
| 2 | White-label CSS theming | `portals/customer-portal.md` | DONE | GET /tenants/config → --primary/--secondary CSS vars |
| 3 | Employee NgRx slice (actions/reducer/effects) | `arch/backend-contracts.md` | DONE | Entity adapter; load/create/delete |
| 4 | Employee list (search + Excel import) | `portals/customer-portal.md` | DONE | FormData POST to /employees/import; snackbar |
| 5 | Employee form (Iqama validation) | `portals/customer-portal.md` | DONE | Pattern /^\d{10}$/ |
| 6 | Leave NgRx slice (actions/reducer/effects) | `arch/backend-contracts.md` | DONE | approve/reject effects |
| 7 | Leave queue (tabs: Pending/Approved/Rejected) | `portals/customer-portal.md` | DONE | Approve/Reject with comment prompt |
| 8 | Workflow builder (CDK DragDrop step reorder) | `portals/customer-portal.md` | DONE | PUT /workflows/definition |
| 9 | Attendance reports (date filter + export) | `portals/customer-portal.md` | DONE | PDF/Excel via window.open |
| 10 | Bulk messages + clarification requests | `portals/customer-portal.md` | DONE | Multi-select; POST /notifications/bulk + /clarifications/bulk |
| 11 | i18n files (AR/EN/UR) | `mobile/l10n.md` | DONE | Full flat-key JSON for all portal features |
| 12 | Leave reducer unit tests | `testing/test-strategy.md` | DONE | 6 Jest tests |

## Sprint 11: Infrastructure

| # | Checkpoint | Spec Source | Status | Notes |
|---|-----------|-------------|--------|-------|
| 1 | MinIO pre-signed URL service (INFRA-13) | `arch/infra.md` | DONE | IStorageService + MinioStorageService; presigned PUT/GET/Delete |
| 2 | Helm chart (mondabet-api) | `arch/infra.md` INFRA-07/08 | DONE | Deployment + Service + Ingress + HPA templates |
| 3 | GitHub Actions CI pipeline (INFRA-09) | `arch/infra.md` | DONE | .NET build/test, Docker push (matrix), Helm lint, Angular test/build, Flutter test/build |
| 4 | OpenTelemetry instrumentation (INFRA-11) | `arch/infra.md` | DONE | AddMondabetTracing extension; OTLP exporter; health endpoint filtered |

## Sprint 12: Security Hardening

| # | Checkpoint | Spec Source | Status | Notes |
|---|-----------|-------------|--------|-------|
| 1 | RBAC enforcement tests (SEC-01) | `arch/security.md` | DONE | RbacPolicyTests – role isolation + tenant claim validation |
| 2 | AES-256 Iqama encryption (SEC-02) | `arch/security.md` | DONE | AesEncryptionService (random IV); round-trip + tamper tests |
| 3 | Audit log table (SEC-06) | `arch/security.md` | DONE | AuditLog entity, IAuditLogger, DbAuditLogger, AuditDbContext in 'audit' schema |

## Speckit Files Coverage

| Spec File | Used In Sprint | Status |
|-----------|---------------|--------|
| `CLAUDE.md` | All | Reference doc (no code output) |
| `arch/system-overview.md` | S1 (CP1,3,8) | DONE |
| `arch/backend-contracts.md` | S1 (CP4-6), S2+ | DONE |
| `arch/db-schema.md` | S1 (CP5), S2+ | DONE |
| `arch/security.md` | S1 (CP4,7,8), S12 | DONE |
| `arch/infra.md` | S1 (CP1,2,6), S11 | DONE |
| `mobile/flutter-spec.md` | S1 (CP9), S8 | DONE |
| `mobile/l10n.md` | S1 (CP9), S8, S10 | DONE |
| `portals/white-label-portal.md` | S9 | DONE |
| `portals/customer-portal.md` | S10 | DONE |
| `backlog/jira-backlog.md` | All | Reference doc |
| `testing/test-strategy.md` | S1 (CP10), S8, S9, S10, S12 | DONE |
