using MediatR;
using Mondabet.Report.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Report.Application.Queries.PunctualityAnalytics;

public record PunctualityAnalyticsQuery(DateTime From, DateTime To)
    : IRequest<Result<IReadOnlyList<EmployeePunctualityDto>>>;
