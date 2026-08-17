namespace SyncWave.Core.Tests;

using SyncWave.Core.Entities;
using SyncWave.Core.Enums;

public class RoomTests
{
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


}
