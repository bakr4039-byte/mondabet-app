using Mondabet.Shared.Domain;

namespace Mondabet.Clarification.Domain.Entities;

public class ClarificationAttachment : BaseEntity
{
    public Guid ClarificationId { get; private set; }
    public string FileUrl { get; private set; } = default!;
    public string FileName { get; private set; } = default!;

    private ClarificationAttachment() { }

    public static ClarificationAttachment Create(
        Guid clarificationId, string fileUrl, string fileName) => new()
    {
        Id = Guid.NewGuid(),
        ClarificationId = clarificationId,
        FileUrl = fileUrl,
        FileName = fileName,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };
}
