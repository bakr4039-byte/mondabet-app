# Test Strategy – Mondabet

## .NET Microservices
- **Unit**: xUnit + FluentAssertions + Moq
  - Test every Command/Query Handler in isolation (mock IRepository)
  - Test every Domain entity business rule
  - Naming: `MethodName_Scenario_ExpectedResult`
- **Integration**: WebApplicationFactory + Testcontainers (SQL Server, Redis)
  - One integration test per API endpoint
  - Test auth middleware, tenant isolation, rate limiting
- **Coverage target**: 80% on Application layer, 60% on Infrastructure

## Flutter
- **Unit**: flutter_test + mocktail
  - Test every BLoC: emit sequence for each event
  - Test GPS geofence helper function (haversine)
  - Test token refresh interceptor logic
- **Widget**: testWidgets for critical screens (LoginScreen, CheckInScreen)
- **Integration**: flutter_driver or integration_test package for E2E on emulator

## Angular
- **Unit**: Jest (replace Karma) + Testing Library
  - Test every NgRx reducer, effect, selector
  - Test form validators
- **E2E**: Cypress
  - Critical paths: login → check-in, submit vacation → approve in portal
  - Run in CI against staging environment

## Test Data Pattern (.NET)
```csharp
// Use Builder pattern for test entities
var employee = EmployeeBuilder.Default()
    .WithTenantId(tenantId)
    .WithShift(shiftId)
    .Build();
```

## CI Test Gates
- PR: unit tests must pass (< 2 min)
- Merge to main: unit + integration tests (< 10 min)
- Nightly: full E2E suite on staging
