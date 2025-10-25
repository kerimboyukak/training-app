using Domain;
using Test;

namespace Training.Domain.Tests
{
    public class TrainingTests
    {
        private string _name = null!;
        private string _description = null!;
        private int _maximumCapacity;
        private Code _roomCode = null!;
        private string _coachId = null!;
        private TimeWindow _timeWindow = null!;
        private int _sequence;

        [SetUp]
        public void BeforeEachTest()
        {
            _name = "Training";
            _description = Random.Shared.NextString();
            _maximumCapacity = Random.Shared.Next(2, 100);
            _roomCode = new Code(Random.Shared.NextString().Substring(0, 11), Random.Shared.Next(1, 100));
            _coachId = Random.Shared.NextString();
            _timeWindow = new TimeWindow(DateTime.Now.AddDays(1), DateTime.Now.AddDays(2));
            _sequence = 1;
        }

        [Test]
        public void CreateNew_ValidInput_ShouldInitializeFieldsCorrectly()
        {
            // Act
            Training training = Training.CreateNew(_name, _description, _maximumCapacity, _roomCode, _coachId, _timeWindow, _sequence);

            // Assert
            Assert.That(training.Name, Is.EqualTo(_name));
            Assert.That(training.Description, Is.EqualTo(_description));
            Assert.That(training.MaximumCapacity, Is.EqualTo(_maximumCapacity));
            Assert.That(training.RoomCode, Is.EqualTo(_roomCode));
            Assert.That(training.CoachId, Is.EqualTo(_coachId));
            Assert.That(training.TimeWindow, Is.EqualTo(_timeWindow));
            Assert.That(training.TrainingCode.ToString(), Is.EqualTo($"{_name}-{_sequence}"));
        }

        [Test]
        public void CreateNew_InvalidMaximumCapacity_ShouldThrowContractException()
        {
            Assert.That(() => Training.CreateNew(_name, _description, 1, _roomCode, _coachId, _timeWindow, _sequence), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void CreateNew_EmptyName_ShouldThrowContractException()
        {
            Assert.That(() => Training.CreateNew("", _description, _maximumCapacity, _roomCode, _coachId, _timeWindow, _sequence), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void CreateNew_EmptyDescription_ShouldThrowContractException()
        {
            Assert.That(() => Training.CreateNew(_name, "", _maximumCapacity, _roomCode, _coachId, _timeWindow, _sequence), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void CreateNew_EmptyCoachId_ShouldThrowContractException()
        {
            Assert.That(() => Training.CreateNew(_name, _description, _maximumCapacity, _roomCode, "", _timeWindow, _sequence), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void Register_ValidApprentice_ShouldAddParticipation()
        {
            // Arrange
            Training training = Training.CreateNew(_name, _description, _maximumCapacity, _roomCode, _coachId, _timeWindow, _sequence);
            Apprentice apprentice = Apprentice.CreateNew(Random.Shared.NextString(), Random.Shared.NextString(), Random.Shared.NextString(), Random.Shared.NextString());

            // Act
            training.Register(apprentice);

            // Assert
            Assert.That(training.Participations.Count, Is.EqualTo(1));
            Assert.That(training.Participations.First().ApprenticeId, Is.EqualTo(apprentice.Id));
        }

        [Test]
        public void Register_ExceedingCapacity_ShouldThrowContractException()
        {
            // Arrange
            int validCapacity = 2;
            Training training = Training.CreateNew(_name, _description, validCapacity, _roomCode, _coachId, _timeWindow, _sequence);
            Apprentice apprentice1 = Apprentice.CreateNew(Random.Shared.NextString(), Random.Shared.NextString(), Random.Shared.NextString(), Random.Shared.NextString());
            Apprentice apprentice2 = Apprentice.CreateNew(Random.Shared.NextString(), Random.Shared.NextString(), Random.Shared.NextString(), Random.Shared.NextString());
            Apprentice apprentice3 = Apprentice.CreateNew(Random.Shared.NextString(), Random.Shared.NextString(), Random.Shared.NextString(), Random.Shared.NextString());

            // Act
            training.Register(apprentice1);
            training.Register(apprentice2);

            // Assert
            Assert.That(() => training.Register(apprentice3), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void Register_DuplicateApprentice_ShouldThrowContractException()
        {
            // Arrange
            Training training = Training.CreateNew(_name, _description, _maximumCapacity, _roomCode, _coachId, _timeWindow, _sequence);
            Apprentice apprentice = Apprentice.CreateNew(Random.Shared.NextString(), Random.Shared.NextString(), Random.Shared.NextString(), Random.Shared.NextString());

            // Act
            training.Register(apprentice);

            // Assert
            Assert.That(() => training.Register(apprentice), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void Register_AfterRegistrationClosed_ShouldThrowContractException()
        {
            // Arrange
            TimeWindow timeWindow = new TimeWindow(DateTime.Now.AddMinutes(-30), DateTime.Now.AddMinutes(-15));
            Training training = Training.CreateNew(_name, _description, _maximumCapacity, _roomCode, _coachId, timeWindow, _sequence);
            Apprentice apprentice = Apprentice.CreateNew(Random.Shared.NextString(), Random.Shared.NextString(), Random.Shared.NextString(), Random.Shared.NextString());

            // Assert
            Assert.That(() => training.Register(apprentice), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void FinishParticipation_ValidApprentice_ShouldMarkParticipationAsFinished()
        {
            // Arrange
            Training training = Training.CreateNew(_name, _description, _maximumCapacity, _roomCode, _coachId, _timeWindow, _sequence);
            Apprentice apprentice = Apprentice.CreateNew(Random.Shared.NextString(), Random.Shared.NextString(), Random.Shared.NextString(), Random.Shared.NextString());
            training.Register(apprentice);

            // Act
            training.FinishParticipation(apprentice.Id);

            // Assert
            Assert.That(training.Participations.First().IsFinished, Is.True);
        }

        [Test]
        public void FinishParticipation_NonRegisteredApprentice_ShouldThrowContractException()
        {
            // Arrange
            Training training = Training.CreateNew(_name, _description, _maximumCapacity, _roomCode, _coachId, _timeWindow, _sequence);

            // Assert
            Assert.That(() => training.FinishParticipation(Random.Shared.NextString()), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void Trainings_WithSameTrainingCode_ShouldBeEqual()
        {
            // Arrange
            var training1 = Training.CreateNew(_name, _description, _maximumCapacity, _roomCode, _coachId, _timeWindow, _sequence);
            var training2 = Training.CreateNew(_name, _description, _maximumCapacity, _roomCode, _coachId, _timeWindow, _sequence);

            // Act & Assert
            Assert.That(training1, Is.EqualTo(training2));
        }

        [Test]
        public void Trainings_WithDifferentTrainingCode_ShouldNotBeEqual()
        {
            // Arrange
            var training1 = Training.CreateNew(_name, _description, _maximumCapacity, _roomCode, _coachId, _timeWindow, _sequence);
            var differentSequence = 2;
            var training2 = Training.CreateNew(_name, _description, _maximumCapacity, _roomCode, _coachId, _timeWindow, differentSequence);

            // Act & Assert
            Assert.That(training1, Is.Not.EqualTo(training2));
        }
        [Test]
        public void Training_EmptyConstructor_ShouldInitializeFieldsCorrectly()
        {
            // Act
            var training = (Training)Activator.CreateInstance(typeof(Training), true)!;

            // Assert
            Assert.That(training.Participations, Is.Not.Null);
            Assert.That(training.Participations, Is.Empty);
        }

        [Test]
        public void CreateNew_WithRoomAndCoach_ShouldInitializeFieldsCorrectly()
        {
            // Arrange
            var room = Room.CreateNew("Room1");
            var coach = Coach.CreateNew("Coach1", "John", "Doe");

            // Act
            var training = Training.CreateNew(_name, _description, _maximumCapacity, _roomCode, _coachId, _timeWindow, _sequence);
            training.GetType().GetProperty("Room")!.SetValue(training, room);
            training.GetType().GetProperty("Coach")!.SetValue(training, coach);

            // Assert
            Assert.That(training.Room, Is.EqualTo(room));
            Assert.That(training.Coach, Is.EqualTo(coach));
        }


    }
}