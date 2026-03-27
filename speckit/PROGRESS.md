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

## Sprint 5-10: See `backlog/jira-backlog.md` for full breakdown

## Speckit Files Coverage

| Spec File | Used In Sprint | Status |
|-----------|---------------|--------|
| `CLAUDE.md` | All | Reference doc (no code output) |
| `arch/system-overview.md` | S1 (CP1,3,8) | DONE |
| `arch/backend-contracts.md` | S1 (CP4-6), S2+ | DONE (S1) |
| `arch/db-schema.md` | S1 (CP5), S2+ | DONE (S1) |
| `arch/security.md` | S1 (CP4,7,8) | DONE (S1) |
| `arch/infra.md` | S1 (CP1,2,6) | DONE |
| `mobile/flutter-spec.md` | S1 (CP9), S5-6 | DONE (S1) |
| `mobile/l10n.md` | S1 (CP9) | DONE (S1) |
| `portals/white-label-portal.md` | S7 | NOT_STARTED |
| `portals/customer-portal.md` | S8 | NOT_STARTED |
| `backlog/jira-backlog.md` | All | Reference doc |
| `testing/test-strategy.md` | S1 (CP10), S10 | DONE (S1) |
