using Domain;
using Test;

namespace Training.Domain.Tests
{
    public class RoomTests
    {
        private string _name = null!;

        [SetUp]
        public void BeforeEachTest()
        {
            _name = Random.Shared.NextString().Substring(0, 11); // Ensure the name is within the valid length
        }

        [Test]
        public void CreateNew_ValidInput_ShouldInitializeFieldsCorrectly()
        {
            // Act
            Room room = Room.CreateNew(_name);

            // Assert
            Assert.That(room.Name, Is.EqualTo(_name));
            Assert.That(room.RoomCode.Name, Is.EqualTo(_name));
            Assert.That(room.RoomCode.Sequence, Is.EqualTo(1));
        }

        [Test]
        public void CreateNew_EmptyName_ShouldThrowContractException()
        {
            Assert.That(() => Room.CreateNew(""), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void CreateNew_NullName_ShouldThrowContractException()
        {
            Assert.That(() => Room.CreateNew(null!), Throws.InstanceOf<ContractException>());
        }
        [Test]
        public void Rooms_WithSameRoomCode_ShouldBeEqual()
        {
            // Arrange
            var room1 = Room.CreateNew(_name);
            var room2 = Room.CreateNew(_name);

            // Act & Assert
            Assert.That(room1, Is.EqualTo(room2));
        }

        [Test]
        public void Rooms_WithDifferentRoomCode_ShouldNotBeEqual()
        {
            // Arrange
            var room1 = Room.CreateNew(_name);
            var differentName = Random.Shared.NextString().Substring(0, 11);
            var room2 = Room.CreateNew(differentName);

            // Act & Assert
            Assert.That(room1, Is.Not.EqualTo(room2));
        }
    }
}