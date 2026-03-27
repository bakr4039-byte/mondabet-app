using Mondabet.Shared.Domain;

namespace Mondabet.Leave.Domain.Entities;

public class LeaveAttachment : BaseEntity
{
    public Guid LeaveRequestId { get; private set; }
    public string FileUrl { get; private set; } = default!;
    public string FileName { get; private set; } = default!;

    private LeaveAttachment() { }

    public static LeaveAttachment Create(Guid leaveRequestId, string fileUrl, string fileName) => new()
    {
        Id = Guid.NewGuid(),
        LeaveRequestId = leaveRequestId,
        FileUrl = fileUrl,
        FileName = fileName,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };
}
