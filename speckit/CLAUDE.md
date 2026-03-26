# SpecKit – Mondabet Attendance Platform
> Feed this file to Claude Code at session start. It is the single source of truth.
> All other files in /speckit/ are referenced here. Do not re-read them unless explicitly asked.

## 1. Project Identity
| Key | Value |
|-----|-------|
| Product | Mondabet – GPS Attendance Platform |
| Type | White-label SaaS |
| Portals | Mobile (Flutter) · White-Label Admin (Angular) · Customer Company Portal (Angular) |
| Backend | .NET 10 · YARP API Gateway · SQL Server · Docker · Kubernetes |
| Languages | Arabic · English · Urdu |
| Auth | Iqama/Username + Password + SMS MFA + Biometric + Nafath |

## 2. File Map (read on demand only)
```
speckit/
├── CLAUDE.md              ← THIS FILE (always loaded)
├── arch/
│   ├── system-overview.md      ← C4 context, containers, onion layers
│   ├── backend-contracts.md    ← All API endpoint signatures
│   ├── db-schema.md            ← Full ERD + table definitions
│   ├── security.md             ← OWASP, auth flows, YARP policies
│   └── infra.md                ← Docker Compose, K8s manifests spec
├── mobile/
│   ├── flutter-spec.md         ← Feature list, BLoC structure, screens
│   └── l10n.md                 ← i18n keys (AR/EN/UR)
├── portals/
│   ├── white-label-portal.md   ← SuperAdmin Angular features
│   └── customer-portal.md      ← Company Admin Angular features
├── backlog/
│   └── jira-backlog.md         ← Full epic/story/task breakdown
└── testing/
    └── test-strategy.md        ← Unit, integration, e2e strategy
```

## 3. Non-Negotiable Constraints (apply to every file generated)
- **Architecture**: Onion (Domain → Application → Infrastructure → Presentation). No leaking.
- **SOLID**: Every class has one reason to change. Inject all dependencies.
- **Patterns**: Repository + Unit of Work · CQRS via MediatR · Result<T> monad (no exceptions for flow)
- **Security**: OWASP Top 10 · JWT RS256 · AES-256 at rest · TLS 1.3 · Rate limiting in YARP
- **Performance**: EF Core compiled queries · Redis cache layer · Async/await everywhere
- **White-label**: Tenant resolved from subdomain → TenantId injected into every query
- **K8s**: HPA on CPU 70% · PodDisruptionBudget min 1 · Liveness + Readiness probes
- **Tests**: xUnit + FluentAssertions + Moq for .NET; flutter_test + mocktail for Flutter; Cypress for Angular

## 4. Domain Glossary
| Term | Meaning |
|------|---------|
| Tenant | A customer company subscribed on the platform |
| SuperAdmin | White-label owner operators |
| CompanyAdmin | Tenant-level administrator |
| Employee | End user of the mobile app |
| Shift | Configured attendance window (start/end time + location) |
| CheckIn | GPS-verified attendance record |
| Excuse | Employee-uploaded absence justification |
| Permission | Time-bounded leave request with approval workflow |
| Vacation | Multi-day leave request with approval workflow |
| Clarification | Admin-initiated inquiry; employee responds via app |
| URC | Not used in this project (EJAR term – ignore) |

## 5. Quick Reference – Tech Stack per Layer
```
Presentation  │ Flutter 3.x (mobile) · Angular 18 (portals)
API Gateway   │ YARP 2.x on .NET 10 – routing, rate-limit, auth middleware
Microservices │ .NET 10 Minimal API per bounded context
Messaging     │ MassTransit + RabbitMQ (async workflows)
ORM           │ EF Core 9 – code-first, compiled queries
Database      │ SQL Server 2022 (per-tenant schema isolation)
Cache         │ Redis 7
Storage       │ MinIO (S3-compatible) – excuse files, logos
Auth          │ Keycloak 24 (OIDC/OAuth2) + custom Nafath adapter
SMS           │ Unifonic (Saudi) – MFA OTP
Maps          │ Google Maps API + Saudi National Address API
Container     │ Docker 26 · Kubernetes 1.30 · Helm 3
CI/CD         │ GitHub Actions → Harbor registry → ArgoCD
Monitoring    │ OpenTelemetry → Grafana + Loki + Tempo
```

## 6. Bounded Contexts (Microservices)
| Service | Responsibility |
|---------|---------------|
| identity-svc | Auth, JWT, MFA, Nafath, biometric token |
| tenant-svc | Company onboarding, packages, subscription |
| employee-svc | Employee CRUD, HR import, Excel upload |
| attendance-svc | CheckIn/Out, GPS validation, shift management |
| leave-svc | Vacation, Permission, Excuse workflows |
| notification-svc | Bulk SMS, push, email |
| report-svc | PDF/Excel generation, aggregation |
| workflow-svc | Dynamic approval chains |
| gateway | YARP – single ingress |

## 7. Coding Conventions (must follow without being told)
- File naming: `PascalCase` for classes, `kebab-case` for files in Angular/Flutter
- Every command/query handler returns `Result<T>` – never throw for business errors
- No raw SQL except in read-model projections; use compiled EF queries
- All controllers are thin: validate → dispatch MediatR → return mapped DTO
- Flutter: BLoC per feature; no logic in widgets; use go_router
- Angular: standalone components; NgRx for portal state; reactive forms only
- Localization: all user-facing strings via ARB (Flutter) / ngx-translate (Angular)

## 8. GPS Attendance Rule
```
isWithinGeofence = haversineDistance(employeeCoords, shiftLocation) <= shift.RadiusMeters
CheckIn allowed only when: isWithinGeofence AND now BETWEEN shift.StartTime AND shift.EndTime
```

## 9. Approval Workflow Engine (Dynamic)
- Workflow steps stored as JSON in `workflow_svc.WorkflowDefinition`
- Each step has: `ApproverRoleId | ApproverId`, `Order`, `EscalationHours`
- Engine is event-driven via MassTransit: `LeaveRequested` → workflow-svc → `ApprovalRequired` → notification-svc
- CompanyAdmin configures steps in Customer Portal drag-and-drop UI
