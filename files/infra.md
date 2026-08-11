# Infrastructure Specification – Mondabet

## Docker Compose (dev)
```yaml
services:
  sqlserver:    mcr.microsoft.com/mssql/server:2022-latest
  redis:        redis:7-alpine
  rabbitmq:     rabbitmq:3.13-management
  minio:        minio/minio:RELEASE.2024-01-01
  keycloak:     quay.io/keycloak/keycloak:24
  gateway:      mondabet/gateway:latest  # YARP .NET 10
  identity-svc: mondabet/identity-svc:latest
  tenant-svc:   mondabet/tenant-svc:latest
  employee-svc: mondabet/employee-svc:latest
  attendance-svc: mondabet/attendance-svc:latest
  leave-svc:    mondabet/leave-svc:latest
  workflow-svc: mondabet/workflow-svc:latest
  notification-svc: mondabet/notification-svc:latest
  report-svc:   mondabet/report-svc:latest
```

## Kubernetes (production)
```
Namespace: mondabet-prod

Each microservice Deployment:
  replicas: 2 (min)
  resources:
    requests: cpu=250m memory=256Mi
    limits:   cpu=1000m memory=512Mi
  livenessProbe:  GET /health/live  initialDelay=10s period=15s
  readinessProbe: GET /health/ready initialDelay=5s  period=10s

HPA (per svc):
  minReplicas: 2
  maxReplicas: 20
  metrics: cpu avgUtilization=70

PodDisruptionBudget: minAvailable=1

ConfigMap: app settings (non-secret)
Secrets: connection strings, API keys (External Secrets Operator → Azure KeyVault)

Ingress (NGINX):
  {tenantCode}.mondabet.sa → gateway svc
  admin.mondabet.sa → gateway svc (SuperAdmin routes)
  TLS: cert-manager + Let's Encrypt
```

## Service Dockerfile pattern
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "{ServiceName}.dll"]
```

## CI/CD Pipeline (GitHub Actions)
```
PR → lint + unit tests → build Docker image → push to Harbor
Merge main → integration tests → Helm upgrade (staging)
Tag vX.Y.Z → Helm upgrade (production) via ArgoCD
```
