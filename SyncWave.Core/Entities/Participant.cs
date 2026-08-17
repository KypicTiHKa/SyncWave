namespace SyncWave.Core.Entities;

// <summary>
// Описує учасника кімнати.
// </summary>

public class Participant
{

    public Guid Id { get; init; } = Guid.NewGuid(); // Унікальний ідентифікатор учасника
    public required string Username { get; init; } // Ім'я користувача учасника
    public required string ConnectionId { get; set; } // Ідентифікатор підключення учасника (наприклад, для SignalR)
    public bool IsHost { get; set; } = false; // Вказує, чи є учасник хостом кімнати
    public DateTime JoinedAt { get; init; } = DateTime.UtcNow; // Дата та час приєднання учасника до кімнати

}
