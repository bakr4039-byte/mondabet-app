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
| 9 | Flutter Mobile Auth | `mobile/flutter-spec.md`, `mobile/l10n.md` | NOT_STARTED | Scaffold, BLoC, Login/MFA/Biometric screens |
| 10 | Tests + E2E Smoke | `testing/test-strategy.md` | DONE | 7 unit tests passing – LoginCommandHandler (4), RefreshTokenCommandHandler (3) |

## Sprint 2: Tenant + Employee (Future)

| # | Checkpoint | Spec Source | Status |
|---|-----------|-------------|--------|
| - | Tenant CRUD API | `arch/backend-contracts.md` | NOT_STARTED |
| - | Package CRUD API | `arch/backend-contracts.md` | NOT_STARTED |
| - | Per-tenant schema provisioning | `arch/db-schema.md` | NOT_STARTED |
| - | Employee CRUD API | `arch/backend-contracts.md` | NOT_STARTED |
| - | Excel import | `portals/customer-portal.md` | NOT_STARTED |
| - | SuperAdmin Portal scaffold | `portals/white-label-portal.md` | NOT_STARTED |
| - | Customer Portal scaffold | `portals/customer-portal.md` | NOT_STARTED |

## Sprint 3-10: See `backlog/jira-backlog.md` for full breakdown

## Speckit Files Coverage

| Spec File | Used In Sprint | Status |
|-----------|---------------|--------|
| `CLAUDE.md` | All | Reference doc (no code output) |
| `arch/system-overview.md` | S1 (CP1,3,8) | DONE |
| `arch/backend-contracts.md` | S1 (CP4-6), S2+ | DONE (S1) |
| `arch/db-schema.md` | S1 (CP5), S2+ | DONE (S1) |
| `arch/security.md` | S1 (CP4,7,8) | DONE (S1) |
| `arch/infra.md` | S1 (CP1,2,6) | DONE |
| `mobile/flutter-spec.md` | S1 (CP9), S5-6 | NOT_STARTED |
| `mobile/l10n.md` | S1 (CP9) | NOT_STARTED |
| `portals/white-label-portal.md` | S7 | NOT_STARTED |
| `portals/customer-portal.md` | S8 | NOT_STARTED |
| `backlog/jira-backlog.md` | All | Reference doc |
| `testing/test-strategy.md` | S1 (CP10), S10 | DONE (S1) |
