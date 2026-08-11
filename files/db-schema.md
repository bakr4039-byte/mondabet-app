# Database Schema – Mondabet (SQL Server 2022)
> Shared DB · per-tenant schema prefix: tenant_{tenantId}
> All tables include: CreatedAt, UpdatedAt, IsDeleted (soft delete), CreatedBy

## Global Schema (dbo)

### dbo.Tenants
| Column | Type | Notes |
|--------|------|-------|
| Id | uniqueidentifier PK | |
| Code | nvarchar(50) UNIQUE | subdomain slug |
| CompanyName | nvarchar(200) | |
| LogoUrl | nvarchar(500) | MinIO path |
| PrimaryColor | nchar(7) | #RRGGBB |
| SecondaryColor | nchar(7) | |
| Slogan | nvarchar(300) | |
| Address | nvarchar(500) | |
| NationalAddress | nvarchar(200) | |
| Latitude | float | |
| Longitude | float | |
| BankAccount | nvarchar(100) NULL | |
| ZakatNumber | nvarchar(50) NULL | |
| SubscriptionEndDate | datetime2 | |
| PackageId | uniqueidentifier FK | |
| IsActive | bit | |

### dbo.Packages
| Column | Type | Notes |
|--------|------|-------|
| Id | uniqueidentifier PK | |
| Name | nvarchar(100) | |
| MaxUsers | int | |
| PriceMonthly | decimal(10,2) | |
| Features | nvarchar(max) | JSON |

### dbo.Users (Keycloak shadow – for local queries)
| Column | Type | Notes |
|--------|------|-------|
| Id | uniqueidentifier PK | matches Keycloak sub |
| TenantId | uniqueidentifier FK | NULL = SuperAdmin |
| Email | nvarchar(200) | |
| MobileNumber | nvarchar(20) | |
| RoleId | uniqueidentifier FK | |
| IsActive | bit | |

## Tenant Schema (tenant_{tenantId})

### Employees
| Column | Type | Notes |
|--------|------|-------|
| Id | uniqueidentifier PK | |
| UserId | uniqueidentifier FK→dbo.Users | |
| FullNameAr | nvarchar(200) | |
| FullNameEn | nvarchar(200) | |
| Iqama | nvarchar(10) UNIQUE | |
| DateOfBirth | date | |
| JobTitle | nvarchar(200) | |
| MobileNumber | nvarchar(20) | |
| Email | nvarchar(200) | |
| DepartmentId | uniqueidentifier FK | |
| ShiftId | uniqueidentifier FK | NULL = inherit dept |

### Departments
| Column | Type | Notes |
|--------|------|-------|
| Id | uniqueidentifier PK | |
| Name | nvarchar(200) | |
| ParentId | uniqueidentifier NULL FK self | |
| ManagerId | uniqueidentifier FK→Employees | |

### Shifts
| Column | Type | Notes |
|--------|------|-------|
| Id | uniqueidentifier PK | |
| Name | nvarchar(200) | |
| StartTime | time | |
| EndTime | time | |
| Latitude | float | |
| Longitude | float | |
| RadiusMeters | int | default 100 |
| DaysOfWeek | nvarchar(20) | JSON [1-7] |

### Attendance
| Column | Type | Notes |
|--------|------|-------|
| Id | uniqueidentifier PK | |
| EmployeeId | uniqueidentifier FK | |
| ShiftId | uniqueidentifier FK | |
| CheckInTime | datetime2 | |
| CheckOutTime | datetime2 NULL | |
| CheckInLat | float | |
| CheckInLng | float | |
| IsWithinGeofence | bit | |
| DeviceId | nvarchar(100) | |

### LeaveRequests
| Column | Type | Notes |
|--------|------|-------|
| Id | uniqueidentifier PK | |
| EmployeeId | uniqueidentifier FK | |
| LeaveType | tinyint | 1=Vacation 2=Permission 3=Excuse |
| StartDate | date | |
| EndDate | date NULL | vacations only |
| StartTime | time NULL | permissions only |
| EndTime | time NULL | permissions only |
| Reason | nvarchar(1000) | |
| Status | tinyint | 1=Pending 2=Approved 3=Rejected |
| WorkflowInstanceId | uniqueidentifier FK NULL | |

### LeaveAttachments
| Column | Type | Notes |
|--------|------|-------|
| Id | uniqueidentifier PK | |
| LeaveRequestId | uniqueidentifier FK | |
| FileUrl | nvarchar(500) | MinIO |
| FileName | nvarchar(200) | |

### WorkflowDefinitions
| Column | Type | Notes |
|--------|------|-------|
| Id | uniqueidentifier PK | |
| Name | nvarchar(200) | |
| AppliesTo | tinyint | 1=Vacation 2=Permission 3=Both |
| StepsJson | nvarchar(max) | [{order, approverRoleId, escalationHours}] |

### WorkflowInstances
| Column | Type | Notes |
|--------|------|-------|
| Id | uniqueidentifier PK | |
| DefinitionId | uniqueidentifier FK | |
| EntityId | uniqueidentifier | LeaveRequestId |
| CurrentStep | int | |
| Status | tinyint | 1=InProgress 2=Completed 3=Rejected |

### Clarifications
| Column | Type | Notes |
|--------|------|-------|
| Id | uniqueidentifier PK | |
| EmployeeId | uniqueidentifier FK | |
| FromDate | date | |
| ToDate | date | |
| Question | nvarchar(2000) | |
| ResponseText | nvarchar(2000) NULL | |
| Status | tinyint | 1=Pending 2=Responded |

### ClarificationAttachments
Same structure as LeaveAttachments referencing ClarificationId.

### Roles
| Column | Type |
|--------|------|
| Id | uniqueidentifier PK |
| Name | nvarchar(100) |
| Permissions | nvarchar(max) JSON |

## Indexes (critical)
```sql
CREATE INDEX IX_Attendance_Employee_Date ON Attendance(EmployeeId, CheckInTime);
CREATE INDEX IX_LeaveRequests_Employee_Status ON LeaveRequests(EmployeeId, Status);
CREATE INDEX IX_Employees_Iqama ON Employees(Iqama);
CREATE INDEX IX_Tenants_Code ON dbo.Tenants(Code);
```
