using SyncWave.Core.Enums;
using System.ComponentModel.Design;

namespace SyncWave.Core.Entities;

// <summary>
// Описує кімнату для спільного відтворення медіа.
// </summary>

public class Room
{

    public Guid Id { get; init; } = Guid.NewGuid(); // Унікальний ідентифікатор кімнати
    public required string RoomCode { get; init; } // Код кімнати, який використовується для приєднання
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow; // Дата та час створення кімнати
    public MediaItem? CurrentMedia { get; set; } // Поточний медіа-елемент, який відтворюється в кімнаті
    public TimeSpan CurrentPosition { get; set; } = TimeSpan.Zero; // Поточна позиція відтворення медіа-елемента
    public PlaybackState State { get; set; } = PlaybackState.Stopped; // Поточний стан відтворення (наприклад, відтворюється, призупинено, зупинено)

    private readonly List<Participant> _participants = new(); // Список учасників кімнати
    public IReadOnlyList<Participant> Participants => _participants.AsReadOnly(); // Публічний доступ до списку учасників у вигляді IReadOnlyList

    private readonly List<MediaItem> _queue = new(); // Список медіа-елементів у черзі
    public IReadOnlyList<MediaItem> Queue => _queue.AsReadOnly(); // Публічний доступ до черги медіа-елементів у вигляді IReadOnlyList

    // Метод для додавання учасника до кімнати
    public void AddParticipant(Participant participant)
    {
        // якщо кімната порожня, перший учасник стає хостом
        if (_participants.Count == 0)
            participant.IsHost = true;

        // додаємо учасника до списку учасників
        _participants.Add(participant);

    }

    // Метод для видалення учасника з кімнати за його ConnectionId
    public void RemoveParticipant(string connectionId)
    {
        // шукаємо учасника за його ConnectionId
        var participant = _participants.FirstOrDefault(p => p.ConnectionId == connectionId);

        // якщо учасник не знайдений, виходимо з методу
        if (participant is null)
            return;

        // якщо учасник є хостом і в кімнаті є ще учасники, передаємо роль хоста іншому учаснику
        if (participant.IsHost && _participants.Count > 1)
        {
            var newHost = _participants.FirstOrDefault(p => p != participant);
            if (newHost != null)
            {
                newHost.IsHost = true;
            }
        }

        // видаляємо учасника з кімнати
        _participants.Remove(participant);

        // якщо після видалення учасника в кімнаті не залишилося учасників, зупиняємо відтворення та скидаємо позицію
        if (_participants.Count == 0)
        {
            State = PlaybackState.Stopped;
            CurrentPosition = TimeSpan.Zero;
        }
    }

    // Метод для додавання медіа-елемента до черги відтворення
    public void EnqueueMedia(MediaItem media)
    {
        // якщо поточний медіа-елемент відсутній, встановлюємо його як поточний та починаємо відтворення
        if (CurrentMedia is null)
        {
            CurrentMedia = media;
            CurrentPosition = TimeSpan.Zero;
            State = PlaybackState.Playing;
        }
        else // якщо поточний медіа-елемент вже відтворюється, додаємо новий медіа-елемент до черги
        {
            _queue.Add(media);
        }
    }

    // Метод для відтворення наступного медіа-елемента з черги
    public void PlayNext()
    { 
        if (_queue.Count > 0) // якщо в черзі є медіа-елементи, відтворюємо наступний
        {
            var nextMedia = _queue[0];
            _queue.RemoveAt(0);
            CurrentMedia = nextMedia;
            CurrentPosition = TimeSpan.Zero;
            State = PlaybackState.Playing;
        }
        else // якщо черга порожня, зупиняємо відтворення та скидаємо поточний медіа-елемент і позицію
        {
            CurrentMedia = null;
            CurrentPosition = TimeSpan.Zero;
            State = PlaybackState.Stopped;
        }
    }

    // Метод для призупинення відтворення
    public void Pause() 
    {
        if (State == PlaybackState.Playing) // якщо відтворення триває, призупиняємо його
        {
            State = PlaybackState.Paused;
        }
    }

    // Метод для відновлення відтворення
    public void Resume()
    {
        // якщо поточний медіа-елемент існує і відтворення призупинено, відновлюємо його
        if (CurrentMedia != null && State == PlaybackState.Paused)
        {
            State = PlaybackState.Playing;
        }
    }

    // Метод зміни поточної позиції відтворення медіа-елемента
    public void SeekTo(TimeSpan position)
    {
        // перевіряємо, чи існує поточний медіа-елемент і чи вказана позиція знаходиться в межах тривалості медіа-елемента
        if (CurrentMedia != null && position >= TimeSpan.Zero && position <= CurrentMedia.Duration)
        {
            CurrentPosition = position;
        }
    }

}
