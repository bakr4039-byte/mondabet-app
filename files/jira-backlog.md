# Jira Backlog – Mondabet Attendance Platform
> Format: EPIC > Story > Task (sub-task)
> Story Points scale: 1=trivial, 2=small, 3=medium, 5=large, 8=xlarge, 13=complex

---

## EPIC-01: Platform Infrastructure & DevOps
**Goal**: Establish all infrastructure, CI/CD, and shared services before feature development.

| ID | Type | Title | SP | Priority |
|----|------|-------|----|----------|
| INFRA-01 | Story | Set up monorepo structure (services + mobile + portals) | 3 | P0 |
| INFRA-02 | Story | Docker Compose for local dev (SQL, Redis, RabbitMQ, MinIO, Keycloak) | 5 | P0 |
| INFRA-03 | Story | Keycloak configuration: realms, clients, RS256 JWKS | 5 | P0 |
| INFRA-04 | Story | YARP API Gateway: routing rules, JWT validation middleware | 5 | P0 |
| INFRA-05 | Story | YARP: rate limiting policies per route group | 3 | P0 |
| INFRA-06 | Story | YARP: CORS, request size limits, header sanitization | 2 | P0 |
| INFRA-07 | Story | Kubernetes Helm charts for all services | 8 | P1 |
| INFRA-08 | Story | HPA configuration per service (cpu=70%) | 3 | P1 |
| INFRA-09 | Story | GitHub Actions CI pipeline (lint, test, build, push to Harbor) | 5 | P1 |
| INFRA-10 | Story | ArgoCD GitOps setup (staging + production) | 5 | P1 |
| INFRA-11 | Story | OpenTelemetry instrumentation (traces, metrics, logs) | 5 | P1 |
| INFRA-12 | Story | Grafana + Loki + Tempo dashboards | 3 | P2 |
| INFRA-13 | Story | MinIO bucket setup + pre-signed URL service | 3 | P1 |
| INFRA-14 | Story | External Secrets Operator + Azure KeyVault integration | 3 | P1 |
| INFRA-15 | Story | Shared .NET library: Result<T>, BaseEntity, TenantMiddleware | 3 | P0 |

---

## EPIC-02: Identity & Authentication
**Goal**: Secure multi-method authentication for all user types.

| ID | Type | Title | SP | Priority |
|----|------|-------|----|----------|
| AUTH-01 | Story | Iqama/username + password login endpoint | 5 | P0 |
| AUTH-02 | Story | SMS OTP MFA via Unifonic integration | 5 | P0 |
| AUTH-03 | Story | Biometric: device key registration + challenge/verify flow | 8 | P0 |
| AUTH-04 | Story | Nafath OIDC adapter: initiate + polling verify | 8 | P0 |
| AUTH-05 | Story | JWT refresh token rotation with Redis jti revocation | 5 | P0 |
| AUTH-06 | Story | Keycloak RBAC: SuperAdmin, CompanyAdmin, Employee roles | 3 | P0 |
| AUTH-07 | Story | Per-tenant user isolation (TenantId in JWT, middleware enforcement) | 5 | P0 |
| AUTH-08 | Story | Logout + token revocation | 2 | P0 |
| AUTH-09 | Task | Unit tests: all auth handlers (80% coverage) | 3 | P0 |
| AUTH-10 | Task | Integration tests: auth endpoints | 3 | P1 |

---

## EPIC-03: Tenant & Subscription Management
**Goal**: SuperAdmin can onboard companies and manage subscriptions.

| ID | Type | Title | SP | Priority |
|----|------|-------|----|----------|
| TENANT-01 | Story | Tenant CRUD API (tenant-svc) | 5 | P0 |
| TENANT-02 | Story | Package CRUD API | 3 | P0 |
| TENANT-03 | Story | Per-tenant DB schema provisioning on onboarding | 8 | P0 |
| TENANT-04 | Story | Subscription expiry check middleware (block expired tenants) | 3 | P1 |
| TENANT-05 | Story | Tenant stats API (user count, last activity, subscription) | 3 | P1 |
| TENANT-06 | Story | Logo + color upload to MinIO, pre-signed URL retrieval | 3 | P1 |
| TENANT-07 | Story | Saudi National Address API integration (typeahead) | 5 | P1 |
| TENANT-08 | Story | Google Maps picker integration | 3 | P1 |
| TENANT-09 | Task | Unit + integration tests for tenant-svc | 3 | P1 |

---

## EPIC-04: Employee Management
**Goal**: CompanyAdmin can manage the employee directory.

| ID | Type | Title | SP | Priority |
|----|------|-------|----|----------|
| EMP-01 | Story | Employee CRUD API (employee-svc) | 5 | P0 |
| EMP-02 | Story | Department CRUD with hierarchy | 3 | P1 |
| EMP-03 | Story | Excel import: parse, validate, bulk insert | 8 | P0 |
| EMP-04 | Story | Excel import error report (per-row validation feedback) | 3 | P1 |
| EMP-05 | Story | HR system webhook integration (configurable per tenant) | 8 | P2 |
| EMP-06 | Story | Iqama AES-256 encryption at rest | 3 | P0 |
| EMP-07 | Story | Employee soft-delete | 2 | P1 |
| EMP-08 | Story | Role & permission assignment to company users | 5 | P1 |
| EMP-09 | Task | Unit + integration tests for employee-svc | 3 | P1 |

---

## EPIC-05: Attendance & GPS Check-In
**Goal**: GPS-verified attendance tracking per shift.

| ID | Type | Title | SP | Priority |
|----|------|-------|----|----------|
| ATT-01 | Story | Shift CRUD API | 3 | P0 |
| ATT-02 | Story | Assign shift to department or individual employee | 3 | P0 |
| ATT-03 | Story | Check-in API: receive GPS coords, validate geofence, persist | 5 | P0 |
| ATT-04 | Story | Check-out API | 3 | P0 |
| ATT-05 | Story | Attendance summary API (by employee, date range) | 3 | P1 |
| ATT-06 | Story | Attendance list API with filters (admin) | 3 | P1 |
| ATT-07 | Story | Geofence haversine logic unit tests | 2 | P0 |
| ATT-08 | Story | Prevent duplicate check-in same shift/day | 2 | P0 |
| ATT-09 | Task | Integration tests: check-in flow | 3 | P1 |

---

## EPIC-06: Leave Management & Approval Workflow
**Goal**: Employee can request leaves; dynamic workflow processes approvals.

| ID | Type | Title | SP | Priority |
|----|------|-------|----|----------|
| LEAVE-01 | Story | Excuse request API (with file upload) | 5 | P0 |
| LEAVE-02 | Story | Vacation request API | 5 | P0 |
| LEAVE-03 | Story | Permission (time-off) request API | 5 | P0 |
| LEAVE-04 | Story | Leave list API (employee + admin views) | 3 | P0 |
| LEAVE-05 | Story | Workflow definition CRUD (workflow-svc) | 5 | P1 |
| LEAVE-06 | Story | Workflow engine: event-driven via MassTransit | 13 | P0 |
| LEAVE-07 | Story | Approval/rejection endpoint with comment | 3 | P0 |
| LEAVE-08 | Story | Workflow escalation (timeout → next approver) | 8 | P1 |
| LEAVE-09 | Story | Push + SMS notification on workflow state change | 5 | P1 |
| LEAVE-10 | Story | File download (excuse attachments) via MinIO pre-signed URL | 3 | P1 |
| LEAVE-11 | Task | Unit tests: workflow engine state machine | 5 | P1 |
| LEAVE-12 | Task | Integration tests: leave request → approval flow | 5 | P1 |

---

## EPIC-07: Clarifications
**Goal**: Admin can request written clarification from employees.

| ID | Type | Title | SP | Priority |
|----|------|-------|----|----------|
| CLAR-01 | Story | Create clarification API (admin) | 3 | P1 |
| CLAR-02 | Story | Employee response API (text + file upload) | 3 | P1 |
| CLAR-03 | Story | Clarification list API (admin) | 2 | P1 |
| CLAR-04 | Story | Push notification to employee on new clarification | 2 | P1 |

---

## EPIC-08: Notifications & Messaging
**Goal**: Bulk and targeted messaging capabilities.

| ID | Type | Title | SP | Priority |
|----|------|-------|----|----------|
| NOTIF-01 | Story | Unifonic SMS adapter (notification-svc) | 5 | P0 |
| NOTIF-02 | Story | FCM push adapter | 5 | P0 |
| NOTIF-03 | Story | Bulk SMS/push to all or selected employees | 5 | P1 |
| NOTIF-04 | Story | Notification inbox in mobile app | 3 | P1 |
| NOTIF-05 | Story | Email adapter (SMTP / SendGrid) | 3 | P2 |

---

## EPIC-09: Reports
**Goal**: Downloadable PDF and Excel reports for all stakeholders.

| ID | Type | Title | SP | Priority |
|----|------|-------|----|----------|
| RPT-01 | Story | Attendance report per employee (PDF + Excel) | 8 | P1 |
| RPT-02 | Story | Attendance report all employees (PDF + Excel) | 8 | P1 |
| RPT-03 | Story | Company-level report for SuperAdmin | 5 | P1 |
| RPT-04 | Story | Employee mobile: personal attendance chart (daily/monthly/yearly) | 5 | P1 |
| RPT-05 | Story | Report download in mobile (open in viewer) | 3 | P1 |
| RPT-06 | Task | PDF generation with QuestPDF (.NET) | 3 | P1 |
| RPT-07 | Task | Excel generation with ClosedXML (.NET) | 3 | P1 |

---

## EPIC-10: Flutter Mobile App
**Goal**: Complete employee mobile experience.

| ID | Type | Title | SP | Priority |
|----|------|-------|----|----------|
| MOB-01 | Story | Project scaffold: clean arch, BLoC, go_router, DI | 3 | P0 |
| MOB-02 | Story | White-label theming (fetch tenant config, apply ThemeData) | 5 | P0 |
| MOB-03 | Story | Login screen (Iqama/username + password) | 3 | P0 |
| MOB-04 | Story | MFA OTP screen with resend timer | 3 | P0 |
| MOB-05 | Story | Biometric login (local_auth + server challenge) | 5 | P0 |
| MOB-06 | Story | Nafath login flow | 8 | P1 |
| MOB-07 | Story | Home screen: shift info, check-in button, GPS status | 5 | P0 |
| MOB-08 | Story | GPS geofence validation + Google Maps display | 5 | P0 |
| MOB-09 | Story | Vacation request form + submit | 3 | P0 |
| MOB-10 | Story | Permission (time-off) form + submit | 3 | P0 |
| MOB-11 | Story | Excuse form + file upload | 3 | P0 |
| MOB-12 | Story | My leaves list + status stepper | 3 | P1 |
| MOB-13 | Story | Attendance report screen (chart + download) | 5 | P1 |
| MOB-14 | Story | Notification inbox + FCM deep-link | 3 | P1 |
| MOB-15 | Story | Clarification response screen (text + file) | 3 | P1 |
| MOB-16 | Story | AR/EN/UR localization (ARB files) | 3 | P0 |
| MOB-17 | Story | Offline error handling + retry logic | 3 | P2 |
| MOB-18 | Task | BLoC unit tests (auth, attendance, leave) | 5 | P1 |
| MOB-19 | Task | Widget tests (login, check-in screens) | 3 | P2 |

---

## EPIC-11: SuperAdmin Angular Portal
**Goal**: White-label operator management portal.

| ID | Type | Title | SP | Priority |
|----|------|-------|----|----------|
| WLP-01 | Story | Angular project scaffold (standalone, NgRx, Material, ngx-translate) | 3 | P0 |
| WLP-02 | Story | Auth guard + JWT interceptor + 401 handler | 3 | P0 |
| WLP-03 | Story | Tenant list page (DataTable, search, filter) | 3 | P0 |
| WLP-04 | Story | Create/Edit tenant form (all fields) | 8 | P0 |
| WLP-05 | Story | Logo upload + color pickers | 3 | P1 |
| WLP-06 | Story | Google Maps location picker in form | 5 | P1 |
| WLP-07 | Story | Saudi National Address typeahead | 5 | P1 |
| WLP-08 | Story | Package management CRUD | 3 | P1 |
| WLP-09 | Story | Company reports page (table + bar chart + export) | 5 | P1 |
| WLP-10 | Story | RTL/LTR toggle (AR/EN/UR) | 3 | P0 |
| WLP-11 | Task | Jest unit tests (reducers, effects, forms) | 3 | P1 |
| WLP-12 | Task | Cypress E2E: login → create tenant flow | 3 | P2 |

---

## EPIC-12: Customer Company Angular Portal
**Goal**: Tenant admin self-service portal.

| ID | Type | Title | SP | Priority |
|----|------|-------|----|----------|
| CCP-01 | Story | Angular project scaffold (reuse core library from WLP) | 2 | P0 |
| CCP-02 | Story | Employee list + CRUD forms | 5 | P0 |
| CCP-03 | Story | Excel bulk import UI (drag-drop, progress, error table) | 5 | P0 |
| CCP-04 | Story | Shift management UI | 3 | P0 |
| CCP-05 | Story | Location configuration UI (Maps + National Address + GPS coords) | 5 | P1 |
| CCP-06 | Story | Department management with hierarchy | 3 | P1 |
| CCP-07 | Story | Workflow builder (drag-drop step configuration) | 8 | P1 |
| CCP-08 | Story | Leave approval queue UI (tabs: pending/approved/rejected) | 5 | P0 |
| CCP-09 | Story | Approve/reject dialog with comment | 2 | P0 |
| CCP-10 | Story | Attendance reports UI (filter + chart + export) | 5 | P1 |
| CCP-11 | Story | Bulk message compose + send UI | 3 | P1 |
| CCP-12 | Story | Clarification management (create + track responses) | 3 | P1 |
| CCP-13 | Story | User & role management | 5 | P1 |
| CCP-14 | Story | White-label theming applied via TenantConfig API | 3 | P0 |
| CCP-15 | Story | RTL/LTR support (AR/EN/UR) | 3 | P0 |
| CCP-16 | Task | Jest unit tests | 3 | P1 |
| CCP-17 | Task | Cypress E2E: submit leave → approve flow | 5 | P2 |

---

## EPIC-13: Security Hardening
| ID | Type | Title | SP | Priority |
|----|------|-------|----|----------|
| SEC-01 | Story | OWASP A01: RBAC enforcement + tenant isolation tests | 5 | P0 |
| SEC-02 | Story | OWASP A02: verify AES-256 Iqama encryption in all environments | 3 | P0 |
| SEC-03 | Story | OWASP A07: refresh token rotation + jti revocation list | 5 | P0 |
| SEC-04 | Story | Trivy image scanning in CI pipeline | 2 | P1 |
| SEC-05 | Story | Penetration test: YARP rate limiting validation | 3 | P1 |
| SEC-06 | Story | Audit log table: record sensitive actions per tenant | 3 | P1 |

---

## Sprint Suggestion (2-week sprints)
| Sprint | Epics | Goal |
|--------|-------|------|
| S1 | INFRA, AUTH | Dev env up, auth working end-to-end |
| S2 | TENANT, EMP | Onboarding flow complete |
| S3 | ATT, LEAVE (basic) | Check-in + basic leave requests |
| S4 | LEAVE (workflow), NOTIF | Full approval workflow + notifications |
| S5 | MOB (auth, check-in) | Flutter app MVP |
| S6 | MOB (leave, reports) | Flutter app complete |
| S7 | WLP | SuperAdmin portal |
| S8 | CCP | Customer portal |
| S9 | RPT, CLAR | Reports + clarifications |
| S10 | SEC, testing, E2E | Security hardening + QA |
