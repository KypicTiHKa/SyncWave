namespace SyncWave.Core.Tests;

using SyncWave.Core.Entities;
using SyncWave.Core.Enums;

// <summary>
// Тестовий клас для перевірки функціональності класу Room.
// </summary>

public class RoomTests
{
    // Тест для перевірки додавання першого учасника до кімнати та встановлення його як хоста
    [Fact]
    public void AddParticipant_WhenFirstParticipant_ShouldSetAsHost()
    {
        // Arrange
        // Створюємо нову кімнату та учасника
        var room = new Room { RoomCode = "Test-Room" };
        var participant = new Participant { ConnectionId = "Test-Connection" , Username = "Test-User" };

        // Act
        // Додаємо учасника до кімнати
        room.AddParticipant(participant);

        // Assert
        // Перевіряємо, що учасник встановлений як хост та що він єдиний учасник у кімнаті
        Assert.True(participant.IsHost);
        Assert.Single(room.Participants);

    }

    // Тест для перевірки додавання другого учасника до кімнати та перевірки, що він не встановлений як хост
    [Fact]
    public void AddParticipant_WhenSecondParticipant_ShouldNotSetAsHost()
    {
        // Arrange
        // Створюємо нову кімнату та двох учасників
        var room = new Room { RoomCode = "Test-Room" };
        var host = new Participant { ConnectionId = "Host-Connection", Username = "Host-User" };
        var guest = new Participant { ConnectionId = "Guest-Connection", Username = "Guest-User" };
        room.AddParticipant(host); // Додаємо першого учасника до кімнати

        // Act
        // Додаємо другого учасника до кімнати
        room.AddParticipant(guest);
       
        // Assert
        // Перевіряємо, що перший учасник встановлений як хост, а другий - ні, та що обидва учасники присутні у кімнаті
        Assert.True(host.IsHost);
        Assert.False(guest.IsHost);
        Assert.Equal(2, room.Participants.Count);
    }

    // Тест для перевірки видалення хоста з кімнати та передачі ролі хоста наступному учаснику
    [Fact]
    public void RemoveParticipant_WhenHostLeaves_ShouldTransferHostToNextParticipant()
    {
        // Arrange
        // Створюємо нову кімнату та двох учасників
        var room = new Room { RoomCode = "Test-Room" };
        var host = new Participant { ConnectionId = "Host-Connection", Username = "Host-User" };
        var guest = new Participant { ConnectionId = "Guest-Connection", Username = "Guest-User" };
        room.AddParticipant(host); // Додаємо першого учасника до кімнати
        room.AddParticipant(guest); // Додаємо другого учасника до кімнати

        // Act
        // Видаляємо хоста з кімнати
        room.RemoveParticipant(host.ConnectionId);

        // Assert
        // Перевіряємо, що другий учасник тепер є хостом та що в кімнаті залишився лише один учасник
        Assert.True(guest.IsHost);
        Assert.Single(room.Participants);
    }

    // Тест для перевірки видалення останнього учасника з кімнати та перевірки, що хост не встановлений
    [Fact]
    public void RemoveParticipant_WhenLastParticipantLeaves_ShouldHaveNoHost()
    {
        // Arrange
        // Створюємо нову кімнату та одного учасника
        var room = new Room { RoomCode = "Test-Room" };
        var participant = new Participant { ConnectionId = "Test-Connection", Username = "Test-User" };
        room.AddParticipant(participant); // Додаємо учасника до кімнати
        // Act
        // Видаляємо учасника з кімнати
        room.RemoveParticipant(participant.ConnectionId);
        // Assert
        // Перевіряємо, що в кімнаті немає учасників та що хост не встановлений
        Assert.Empty(room.Participants);
    }

    // Тест для перевірки додавання медіа-елемента до черги відтворення, коли нічого не відтворюється
    [Fact]
    public void EnqueueMedia_WhenNoMediaPlaying_ShouldStartPlayingImmediately()
    {
        // Arrange
        // Створюємо нову кімнату та медіа-елемент
        var room = new Room { RoomCode = "Test-Room" };
        var media = new MediaItem { Title = "Song 1", SourceUrl = "http://testmedia.com/song1", Duration = TimeSpan.FromMinutes(3) };

        // Act
        // Додаємо медіа-елемент до черги відтворення
        room.EnqueueMedia(media);

        // Assert
        // Перевіряємо, що медіа-елемент встановлений як поточний та що стан відтворення встановлений на "Playing"
        Assert.NotNull(room.CurrentMedia);
        Assert.Equal(media, room.CurrentMedia);
        Assert.Equal(PlaybackState.Playing, room.State);
        Assert.Equal(TimeSpan.Zero, room.CurrentPosition);
        Assert.Empty(room.Queue);
    }

    // Тест для перевірки додавання медіа-елемента до черги відтворення, коли вже відтворюється інший медіа-елемент
    [Fact]
    public void EnqueueMedia_WhenMediaAlreadyPlaying_ShouldAddToQueue()
    {
        // Arrange
        // Створюємо нову кімнату та два медіа-елементи
        var room = new Room { RoomCode = "Test-Room" };
        var media1 = new MediaItem { Title = "Song 1", SourceUrl = "http://testmedia.com/song1", Duration = TimeSpan.FromMinutes(3) };
        var media2 = new MediaItem { Title = "Song 2", SourceUrl = "http://testmedia.com/song2", Duration = TimeSpan.FromMinutes(4) };
        room.EnqueueMedia(media1); // Додаємо перший медіа-елемент до черги відтворення

        // Act
        // Додаємо другий медіа-елемент до черги відтворення
        room.EnqueueMedia(media2);

        // Assert
        // Перевіряємо, що перший медіа-елемент встановлений як поточний, а другий доданий до черги
        Assert.Equal(media1, room.CurrentMedia);
        Assert.Single(room.Queue);
        Assert.Equal(media2, room.Queue.First());
    }

    // Тест для перевірки відтворення наступного медіа-елемента з черги та видалення його з черги
    [Fact]
    public void PlayNext_WhenQueueHasItems_ShouldPlayNextAndRemoveFromQueue()
    {

        // Arrange
        // Створюємо нову кімнату та два медіа-елементи
        var room = new Room { RoomCode = "Test-Room" };
        var media1 = new MediaItem { Title = "Song 1", SourceUrl = "http://testmedia.com/song1", Duration = TimeSpan.FromMinutes(3) };
        var media2 = new MediaItem { Title = "Song 2", SourceUrl = "http://testmedia.com/song2", Duration = TimeSpan.FromMinutes(4) };
        room.EnqueueMedia(media1);
        room.EnqueueMedia(media2);

        // Act
        // Викликаємо метод для відтворення наступного медіа-елемента
        room.PlayNext();

        // Assert
        // Перевіряємо, що другий медіа-елемент встановлений як поточний, черга порожня,
        // стан відтворення встановлений на "Playing" та позиція відтворення скинута
        Assert.Equal(media2, room.CurrentMedia);
        Assert.Empty(room.Queue);
        Assert.Equal(PlaybackState.Playing, room.State);
        Assert.Equal(TimeSpan.Zero, room.CurrentPosition);

    }

    // Тест для перевірки відтворення наступного медіа-елемента, коли черга порожня, та перевірки, що відтворення зупиняється
    [Fact]
    public void PlayNext_WhenQueueIsEmpty_ShouldStopPlaybackAndClearCurrentMedia()
    {
        // Arrange
        // Створюємо нову кімнату та медіа-елемент
        var room = new Room { RoomCode = "Test-Room" };
        var media = new MediaItem { Title = "Song 1", SourceUrl = "http://testmedia.com/song1", Duration = TimeSpan.FromMinutes(3) };
        room.EnqueueMedia(media);

        // Act
        // Викликаємо метод для відтворення наступного медіа-елемента, коли черга порожня
        room.PlayNext();

        // Assert
        // Перевіряємо, що поточний медіа-елемент відсутній, стан відтворення встановлений на "Stopped" та позиція відтворення скинута
        Assert.Null(room.CurrentMedia);
        Assert.Equal(PlaybackState.Stopped, room.State);
        Assert.Equal(TimeSpan.Zero, room.CurrentPosition);
    }

}
