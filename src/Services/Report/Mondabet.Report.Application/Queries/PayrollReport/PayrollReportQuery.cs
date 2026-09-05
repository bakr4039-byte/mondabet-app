using MediatR;
using Mondabet.Report.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Report.Application.Queries.PayrollReport;

public record PayrollReportQuery(PayrollReportRequest Request) : IRequest<Result<ReportFileDto>>;
