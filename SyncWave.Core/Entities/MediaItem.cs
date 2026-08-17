using SyncWave.Core.Enums;

namespace SyncWave.Core.Entities;

// <summary>
// Описує медіа-елемент, який може бути відтворений у кімнаті.
// </summary>

public class MediaItem
{
     
    public Guid Id { get; init; } = Guid.NewGuid(); // Унікальний ідентифікатор медіа-елемента
    public string Title { get; set; } = string.Empty; // Назва медіа-елемента
    public string SourceUrl { get; set; } = string.Empty; // URL джерела медіа-елемента
    public MediaType Type { get; set; } // Тип медіа-елемента (наприклад, аудіо, відео)
    public TimeSpan Duration { get; set; } // Тривалість медіа-елемента
    public DateTime AddedAt { get; init; } = DateTime.UtcNow; // Дата та час додавання медіа-елемента

}
