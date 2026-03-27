using Mondabet.Shared.Domain;

namespace Mondabet.Clarification.Domain.Entities;

public enum ClarificationStatus : byte { Pending = 1, Responded = 2 }

public class Clarification : BaseEntity
{
    public Guid EmployeeId { get; private set; }
    public DateOnly FromDate { get; private set; }
    public DateOnly ToDate { get; private set; }
    public string Question { get; private set; } = default!;
    public string? ResponseText { get; private set; }
    public ClarificationStatus Status { get; private set; }

    private Clarification() { }

    public static Clarification Create(
        Guid employeeId, DateOnly fromDate, DateOnly toDate, string question) => new()
    {
        Id = Guid.NewGuid(),
        EmployeeId = employeeId,
        FromDate = fromDate,
        ToDate = toDate,
        Question = question,
        Status = ClarificationStatus.Pending,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    public void Respond(string responseText)
    {
        ResponseText = responseText;
        Status = ClarificationStatus.Responded;
        UpdatedAt = DateTime.UtcNow;
    }
}
