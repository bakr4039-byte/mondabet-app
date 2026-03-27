using MediatR;
using Mondabet.Report.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Report.Application.Queries.CompanyReport;

public record CompanyReportQuery(CompanyReportRequest Request) : IRequest<Result<ReportFileDto>>;
