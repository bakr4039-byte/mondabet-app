# Backend API Contracts – Mondabet
> All routes prefixed with /api/v1. Auth header: Authorization: Bearer {jwt}
> Tenant resolved from JWT claim `tid`. All responses: { data, error, traceId }

## identity-svc
| Method | Route | Body / Params | Returns |
|--------|-------|---------------|---------|
| POST | /auth/login | {identifier, password, tenantCode} | {accessToken, refreshToken, mfaRequired} |
| POST | /auth/mfa/verify | {sessionToken, otp} | {accessToken, refreshToken} |
| POST | /auth/biometric/challenge | {deviceId} | {challenge} |
| POST | /auth/biometric/verify | {deviceId, signedChallenge} | {accessToken, refreshToken} |
| POST | /auth/nafath/initiate | {iqamaNumber} | {transactionId} |
| POST | /auth/nafath/verify | {transactionId} | {accessToken, refreshToken} |
| POST | /auth/refresh | {refreshToken} | {accessToken, refreshToken} |
| POST | /auth/logout | — | 204 |

## tenant-svc (SuperAdmin only)
| Method | Route | Body | Returns |
|--------|-------|------|---------|
| POST | /tenants | TenantCreateDto | TenantDto |
| GET | /tenants | ?page&size&search | PagedResult<TenantDto> |
| GET | /tenants/{id} | — | TenantDto |
| PUT | /tenants/{id} | TenantUpdateDto | TenantDto |
| PUT | /tenants/{id}/subscription | {packageId, endDate} | SubscriptionDto |
| GET | /tenants/{id}/stats | — | TenantStatsDto |
| GET | /packages | — | List<PackageDto> |
| POST | /packages | PackageDto | PackageDto |

## employee-svc
| Method | Route | Body | Returns |
|--------|-------|------|---------|
| POST | /employees | EmployeeCreateDto | EmployeeDto |
| GET | /employees | ?page&size&search | PagedResult<EmployeeDto> |
| GET | /employees/{id} | — | EmployeeDto |
| PUT | /employees/{id} | EmployeeUpdateDto | EmployeeDto |
| DELETE | /employees/{id} | — | 204 |
| POST | /employees/import | multipart/excel | ImportResultDto |
| POST | /employees/{id}/send-message | {message} | 204 |
| POST | /employees/bulk-message | {employeeIds[], message} | 204 |
| GET | /employees/{id}/clarifications | — | List<ClarificationDto> |
| POST | /employees/{id}/clarifications/{clarId}/respond | {text, files[]} | 204 |

## attendance-svc
| Method | Route | Body | Returns |
|--------|-------|------|---------|
| POST | /checkins | {latitude, longitude, deviceId} | CheckInDto |
| GET | /checkins | ?employeeId&from&to | PagedResult<CheckInDto> |
| GET | /shifts | — | List<ShiftDto> |
| POST | /shifts | ShiftCreateDto | ShiftDto |
| PUT | /shifts/{id} | ShiftUpdateDto | ShiftDto |
| GET | /attendance/employee/{id} | ?from&to | AttendanceSummaryDto |

## leave-svc
| Method | Route | Body | Returns |
|--------|-------|------|---------|
| POST | /excuses | {date, reason, files[]} | ExcuseDto |
| POST | /vacations | {startDate, endDate, reason} | VacationDto |
| POST | /permissions | {date, startTime, endTime, reason} | PermissionDto |
| GET | /leaves | ?type&status&page&size | PagedResult<LeaveDto> |
| GET | /leaves/{id} | — | LeaveDto |
| PUT | /leaves/{id}/approve | {comment} | LeaveDto |
| PUT | /leaves/{id}/reject | {reason} | LeaveDto |

## workflow-svc (CompanyAdmin)
| Method | Route | Body | Returns |
|--------|-------|------|---------|
| GET | /workflows | — | List<WorkflowDefinitionDto> |
| POST | /workflows | WorkflowDefinitionDto | WorkflowDefinitionDto |
| PUT | /workflows/{id} | WorkflowDefinitionDto | WorkflowDefinitionDto |
| GET | /workflows/{id}/instances | — | List<WorkflowInstanceDto> |

## report-svc
| Method | Route | Params | Returns |
|--------|-------|--------|---------|
| GET | /reports/attendance | ?employeeId&from&to&format(pdf|xlsx) | File |
| GET | /reports/attendance/all | ?from&to&format | File |
| GET | /reports/companies | ?format | File (SuperAdmin) |

## notification-svc (internal + admin)
| Method | Route | Body | Returns |
|--------|-------|------|---------|
| POST | /notifications/sms | {to, message, tenantId} | 204 |
| POST | /notifications/push | {employeeIds[], title, body} | 204 |
| POST | /clarifications | {employeeId, fromDate, toDate, question} | ClarificationDto |

## DTO Schemas (key ones)

### TenantCreateDto
```json
{
  "companyName": "string", "logoUrl": "string",
  "primaryColor": "#hex", "secondaryColor": "#hex",
  "slogan": "string", "address": "string",
  "nationalAddress": "string", "latitude": 0.0, "longitude": 0.0,
  "bankAccount": "string?", "zakatNumber": "string?",
  "adminEmail": "string", "adminMobile": "string",
  "subscriptionEndDate": "ISO8601", "packageId": "uuid"
}
```

### ShiftCreateDto
```json
{
  "name": "string", "startTime": "HH:mm", "endTime": "HH:mm",
  "latitude": 0.0, "longitude": 0.0, "radiusMeters": 100,
  "daysOfWeek": [1,2,3,4,5]
}
```

### CheckInDto
```json
{
  "id": "uuid", "employeeId": "uuid", "timestamp": "ISO8601",
  "latitude": 0.0, "longitude": 0.0, "isWithinGeofence": true,
  "shiftId": "uuid", "deviceId": "string"
}
```
