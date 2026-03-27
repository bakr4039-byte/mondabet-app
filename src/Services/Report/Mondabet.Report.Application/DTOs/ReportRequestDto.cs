namespace Mondabet.Report.Application.DTOs;

public enum ReportFormat { Pdf, Xlsx }

public record AttendanceReportRequest(
    Guid? EmployeeId,
    DateTime From,
    DateTime To,
    ReportFormat Format);

public record CompanyReportRequest(ReportFormat Format);

public record ReportFileDto(byte[] Content, string ContentType, string FileName);
