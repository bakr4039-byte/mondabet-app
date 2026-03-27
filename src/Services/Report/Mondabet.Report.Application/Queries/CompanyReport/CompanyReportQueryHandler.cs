using MediatR;
using Mondabet.Report.Application.DTOs;
using Mondabet.Report.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Report.Application.Queries.CompanyReport;

public class CompanyReportQueryHandler
    : IRequestHandler<CompanyReportQuery, Result<ReportFileDto>>
{
    private readonly ITenantDataService _data;
    private readonly IReportGenerator _generator;

    public CompanyReportQueryHandler(ITenantDataService data, IReportGenerator generator)
    {
        _data = data;
        _generator = generator;
    }

    public async Task<Result<ReportFileDto>> Handle(
        CompanyReportQuery request, CancellationToken ct)
    {
        var rows = await _data.GetTenantSummaryRowsAsync(ct);
        var r = request.Request;

        var (content, contentType, extension) = r.Format switch
        {
            ReportFormat.Pdf => (
                _generator.GenerateCompanyPdf(rows),
                "application/pdf",
                "pdf"),
            _ => (
                _generator.GenerateCompanyExcel(rows),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "xlsx"),
        };

        return new ReportFileDto(content, contentType,
            $"companies_{DateTime.UtcNow:yyyyMMdd}.{extension}");
    }
}
