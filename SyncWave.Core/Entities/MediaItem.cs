using SyncWave.Core.Enums;

namespace SyncWave.Core.Entities;

public class MediaItem
{
     
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string SourceUrl { get; set; } = string.Empty;

    public MediaType Type { get; set; }

    public TimeSpan Duration { get; set; }

    public DateTime AddedAt { get; init; } = DateTime.UtcNow;


}
