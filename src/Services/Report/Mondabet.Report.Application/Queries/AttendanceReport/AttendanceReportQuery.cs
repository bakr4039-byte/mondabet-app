using MediatR;
using Mondabet.Report.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Report.Application.Queries.AttendanceReport;

public record AttendanceReportQuery(AttendanceReportRequest Request) : IRequest<Result<ReportFileDto>>;
