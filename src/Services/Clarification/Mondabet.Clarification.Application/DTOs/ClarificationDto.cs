using Mondabet.Clarification.Domain.Entities;

namespace Mondabet.Clarification.Application.DTOs;

public record ClarificationDto(
    Guid Id,
    Guid EmployeeId,
    DateOnly FromDate,
    DateOnly ToDate,
    string Question,
    string? ResponseText,
    ClarificationStatus Status,
    DateTime CreatedAt);

public record CreateClarificationDto(
    Guid EmployeeId,
    DateOnly FromDate,
    DateOnly ToDate,
    string Question);

public record RespondClarificationDto(string ResponseText);
