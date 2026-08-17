namespace SyncWave.Core.Tests;

using SyncWave.Core.Entities;

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

}
