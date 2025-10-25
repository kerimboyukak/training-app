using Domain;
using MassTransit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Domain;

namespace Training.AppLogic.Tests
{
    public class CoachServiceTests
    {
        private Mock<ITrainingRepository> _trainingRepositoryMock = null!;
        private Mock<IRoomRepository> _roomRepositoryMock = null!;
        private CoachService _service = null!;


        [SetUp]
        public void Setup()
        {
            _trainingRepositoryMock = new Mock<ITrainingRepository>();
            _roomRepositoryMock = new Mock<IRoomRepository>();

            _service = new CoachService(_trainingRepositoryMock.Object, _roomRepositoryMock.Object);
        }


        [Test]
        public async Task CreateTraining_Should_CreateTraining_And_SaveChanges()
        {
            // Arrange
            string name = "Training";
            string description = "Description";
            int maximumCapacity = 10;
            Code roomCode = new Code("Room-001", 1);
            string coachId = "Coach-001";
            TimeWindow timeWindow = new TimeWindow(DateTime.Now.AddDays(1), DateTime.Now.AddDays(2));
            int sequence = 1;

            var room = Room.CreateNew("Room-001");
            var existingTrainings = new List<Domain.Training>();
            var training = Domain.Training.CreateNew(name, description, maximumCapacity, roomCode, coachId, timeWindow, sequence);


            _trainingRepositoryMock.Setup(repo => repo.GetNumberOfTrainingsByName(name))
             .ReturnsAsync(sequence - 1);
            _roomRepositoryMock.Setup(repo => repo.GetByIdAsync(roomCode))
                .ReturnsAsync(room);
            _trainingRepositoryMock.Setup(repo => repo.GetTrainingsByRoomCode(roomCode))
                .ReturnsAsync(existingTrainings);
            _trainingRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Domain.Training>()))
                .ReturnsAsync(training);

            // Act
            var result = await _service.CreateTraining(name, description, maximumCapacity, roomCode, coachId, timeWindow);

            // Assert
            _trainingRepositoryMock.Verify(repo => repo.GetNumberOfTrainingsByName(name), Times.Once);
            _roomRepositoryMock.Verify(repo => repo.GetByIdAsync(roomCode), Times.Once);
            _trainingRepositoryMock.Verify(repo => repo.GetTrainingsByRoomCode(roomCode), Times.Once);
            _trainingRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Domain.Training>()), Times.Once);
            Assert.That(result.Name, Is.EqualTo(name));
            Assert.That(result.Description, Is.EqualTo(description));
            Assert.That(result.MaximumCapacity, Is.EqualTo(maximumCapacity));
            Assert.That(result.RoomCode, Is.EqualTo(roomCode));
            Assert.That(result.CoachId, Is.EqualTo(coachId));
            Assert.That(result.TimeWindow, Is.EqualTo(timeWindow));
        }

        [Test]
        public void CreateTraining_Should_ThrowException_When_RoomDoesNotExist()
        {
            // Arrange
            string name = "Training";
            string description = "Description";
            int maximumCapacity = 10;
            Code roomCode = new Code("Room-001", 1);
            string coachId = "Coach-001";
            TimeWindow timeWindow = new TimeWindow(DateTime.Now.AddDays(1), DateTime.Now.AddDays(2));

            _roomRepositoryMock.Setup(repo => repo.GetByIdAsync(roomCode))
                .ReturnsAsync((Room?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ContractException>(() => _service.CreateTraining(name, description, maximumCapacity, roomCode, coachId, timeWindow));
            Assert.That(ex.Message, Is.EqualTo("The room with the given code does not exist."));
        }

        [Test]
        public void CreateTraining_Should_ThrowException_When_RoomNotAvailable()
        {
            // Arrange
            string name = "Training";
            string description = "Description";
            int maximumCapacity = 10;
            Code roomCode = new Code("Room", 1);
            string coachId = "Coach-001";
            TimeWindow timeWindow = new TimeWindow(DateTime.Now.AddDays(1), DateTime.Now.AddDays(2));

            var room = Room.CreateNew("Room");
            var existingTrainings = new List<Domain.Training>
            {
                Domain.Training.CreateNew(name, description, maximumCapacity, roomCode, coachId, new TimeWindow(DateTime.Now.AddDays(1), DateTime.Now.AddDays(2)), 1)
            };

            _roomRepositoryMock.Setup(repo => repo.GetByIdAsync(roomCode))
                .ReturnsAsync(room);
            _trainingRepositoryMock.Setup(repo => repo.GetTrainingsByRoomCode(roomCode))
                .ReturnsAsync(existingTrainings);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ContractException>(() => _service.CreateTraining(name, description, maximumCapacity, roomCode, coachId, timeWindow));
            Assert.That(ex.Message, Is.EqualTo("The room is not available for the given time window."));
        }

    }
}
