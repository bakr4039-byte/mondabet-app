# System Architecture – Mondabet

## C4 Context
```
[Employee Mobile App] ──► [API Gateway / YARP]
[Company Admin Portal] ──► [API Gateway / YARP]
[SuperAdmin Portal] ──► [API Gateway / YARP]
                              │
        ┌─────────────────────┼─────────────────────┐
        ▼                     ▼                     ▼
  identity-svc          tenant-svc           employee-svc
  attendance-svc         leave-svc          workflow-svc
  notification-svc      report-svc
        │
  [SQL Server] [Redis] [MinIO] [RabbitMQ] [Keycloak]
```

## Onion Layers (per microservice)
```
src/
└── {ServiceName}/
    ├── Domain/           # Entities, Value Objects, Domain Events, IRepositories
    ├── Application/      # Commands, Queries, Handlers, DTOs, IServices
    ├── Infrastructure/   # EF DbContext, Repositories, ExternalService adapters
    └── Api/              # Controllers/Minimal API, Middleware, DI registration
```
- **Domain** has zero external dependencies.
- **Application** depends on Domain only.
- **Infrastructure** implements Application interfaces.
- **Api** wires DI; depends on Application + Infrastructure.

## Database – Multi-tenant Strategy
- Shared database, separate schema per tenant: `tenant_{tenantId}.Employees`
- TenantId resolved in YARP from JWT claim `tid` → forwarded as header `X-Tenant-Id`
- EF `DbContext` uses `HasDefaultSchema(tenantId)` applied via middleware

## Kubernetes Topology
```yaml
Namespace: mondabet
Deployments: gateway, identity-svc, tenant-svc, employee-svc,
             attendance-svc, leave-svc, workflow-svc,
             notification-svc, report-svc
StatefulSets: sqlserver, redis, rabbitmq, minio, keycloak
HPA: minReplicas=2 maxReplicas=20 cpuThreshold=70%
Ingress: NGINX + cert-manager (Let's Encrypt)
```

## Inter-service Communication
| Pattern | Usage |
|---------|-------|
| REST (YARP) | Client → Gateway → Service |
| gRPC | Service-to-service sync calls (identity ↔ attendance) |
| MassTransit/RabbitMQ | Async events (leave workflow, notifications) |
