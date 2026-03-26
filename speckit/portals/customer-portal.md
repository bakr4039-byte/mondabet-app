# Customer Company Portal – Angular Spec

## Architecture (same pattern as white-label portal, different features)
```
src/app/
├── core/ (shared with white-label portal as nx library)
├── features/
│   ├── employees/      # CRUD, Excel import, HR integration
│   ├── attendance/     # Shift config, location setup
│   ├── leaves/         # Approval queue, workflow config
│   ├── reports/        # Attendance reports per user/all
│   ├── messages/       # Bulk SMS, clarifications
│   ├── workflow/       # Drag-and-drop approval chain builder
│   └── users/          # Company admin user + role management
└── shared/
```

## Module: Employees
Route: /employees
- DataTable: name, iqama, job, department, shift, status
- Add employee form (EmployeeCreateDto fields)
- Bulk import: drag-drop Excel → POST /employees/import → show ImportResultDto (success count, errors list)
- HR integration: configure webhook URL in settings; system pulls from HR periodically
- Send message to individual employee

## Module: Attendance Configuration
Route: /attendance/config
- Shift management: create/edit shifts (name, times, days of week)
- Location picker: Google Maps click OR Saudi National Address typeahead → auto-fill lat/lng
- Set geofence radius (slider 50–500m)
- Assign shifts to departments or individuals

## Module: Leave Management (Approval Queue)
Route: /leaves
- Tabs: Pending | Approved | Rejected
- Each card: employee, leave type, dates, reason, attached files
- Approve / Reject with comment
- View workflow step history

## Module: Workflow Builder
Route: /workflow/builder
- Drag-and-drop step reordering (Angular CDK DragDrop)
- Each step: select approver (role dropdown or specific user) + escalation hours
- Apply to: Vacation | Permission | Both
- Save → PUT /workflows/{id}

## Module: Reports (CompanyAdmin)
Route: /reports
- Filter: employee (typeahead) | date range | all employees
- Summary table + attendance chart
- Export buttons: PDF (GET /reports/attendance?format=pdf) | Excel (format=xlsx)
- "All employees" report: /reports/attendance/all

## Module: Bulk Messages
Route: /messages
- Select all or multi-select employees (checkbox table)
- Type message (max 160 chars SMS / 1000 chars push)
- Send → POST /employees/bulk-message
- Clarification: select employee, date range, question text → POST /clarifications
  - Track response status in table

## Module: User & Role Management
Route: /admin/users
- Create CompanyAdmin users with custom roles
- Role builder: toggle permissions (view_reports, approve_leaves, manage_employees, etc.)

## White-label Theming
- Portal uses tenant primaryColor + secondaryColor from TenantConfig (fetched on init)
- Logo shown in sidebar header
- ThemeService patches CSS variables: --primary, --secondary
