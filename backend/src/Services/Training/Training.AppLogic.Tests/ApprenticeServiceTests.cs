using Domain;
using IntegrationEvents.Employee;
using MassTransit;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Training.Domain;
using Training.Domain.Tests.Builders;

namespace Training.AppLogic.Tests
{
    public class ApprenticeServiceTests
    {
        private Mock<ITrainingRepository> _trainingRepositoryMock = null!;
        private Mock<IApprenticeRepository> _apprenticeRepositoryMock = null!;
        private Mock<IPublishEndpoint> _eventBusMock = null!;
        private ApprenticeService _service = null!;

        [SetUp]
        public void Setup()
        {
            _trainingRepositoryMock = new Mock<ITrainingRepository>();
            _apprenticeRepositoryMock = new Mock<IApprenticeRepository>();
            _eventBusMock = new Mock<IPublishEndpoint>();

            _service = new ApprenticeService(_trainingRepositoryMock.Object, _apprenticeRepositoryMock.Object, _eventBusMock.Object);
        }

        [Test]
        public async Task RegisterApprentice_Should_RegisterApprentice_And_SaveChanges()
        {
            // Arrange
            string trainingCode = "Training-001";
            string apprenticeId = Guid.NewGuid().ToString();

            var training = new TrainingBuilder().Build();
            var apprentice = Apprentice.CreateNew(apprenticeId, "John", "Doe", "Company");
            Code code = new Code(trainingCode);

            _trainingRepositoryMock.Setup(repo => repo.GetByCodeAsync(It.Is<Code>(c => c.Equals(code))))
                .ReturnsAsync(training);
            _apprenticeRepositoryMock.Setup(repo => repo.GetByIdAsync(apprenticeId))
                .ReturnsAsync(apprentice);

            // Act
            var result = await _service.RegisterApprentice(trainingCode, apprenticeId);

            // Assert
            _trainingRepositoryMock.Verify(repo => repo.GetByCodeAsync(It.Is<Code>(c => c.Equals(code))), Times.Once);
            _apprenticeRepositoryMock.Verify(repo => repo.GetByIdAsync(apprenticeId), Times.Once);
            Assert.That(training.Participations, Has.Exactly(1).Matches<Participation>(p => p.ApprenticeId == apprenticeId));
            _trainingRepositoryMock.Verify(repo => repo.CommitTrackedChangesAsync(), Times.Once);
            Assert.That(result, Is.SameAs(apprentice));
        }


        [Test]
        public void RegisterApprentice_Should_ThrowException_When_TrainingNotFound()
        {
            // Arrange
            string trainingCode = "Training-001";
            string apprenticeId = Guid.NewGuid().ToString();

            _trainingRepositoryMock.Setup(repo => repo.GetByCodeAsync(It.IsAny<Code>()))
                .ReturnsAsync((Domain.Training?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ContractException>(() => _service.RegisterApprentice(trainingCode, apprenticeId));
            Assert.That(ex.Message, Is.EqualTo($"Cannot find a training with code '{trainingCode}'"));
        }

        [Test]
        public void RegisterApprentice_Should_ThrowException_When_ApprenticeNotFound()
        {
            // Arrange
            string trainingCode = "Training-001";
            string apprenticeId = Guid.NewGuid().ToString();

            var training = new TrainingBuilder().Build();

            _trainingRepositoryMock.Setup(repo => repo.GetByCodeAsync(It.IsAny<Code>()))
                .ReturnsAsync(training);
            _apprenticeRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Apprentice?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ContractException>(() => _service.RegisterApprentice(trainingCode, apprenticeId));
            Assert.That(ex.Message, Is.EqualTo($"Cannot find an apprentice with id '{apprenticeId}'"));
        }

        [Test]
        public async Task RegisterExternalApprentice_Should_RegisterNewApprentice_And_SaveChanges()
        {
            // Arrange
            string trainingCode = "Training-001";
            string firstName = "John";
            string lastName = "Doe";
            string company = "Company";

            var training = new TrainingBuilder().Build();
            var apprenticeId = Guid.NewGuid().ToString().Substring(0, 11);
            var apprentice = Apprentice.CreateNew(apprenticeId, firstName, lastName, company);

            _trainingRepositoryMock.Setup(repo => repo.GetByCodeAsync(It.IsAny<Code>()))
                .ReturnsAsync(training);
            _apprenticeRepositoryMock.Setup(repo => repo.GetByNameAndCompanyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Apprentice?)null);
            _apprenticeRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Apprentice>()))
                .Returns(Task.CompletedTask);
            _apprenticeRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(apprentice);

            // Act
            var result = await _service.RegisterExternalApprentice(trainingCode, firstName, lastName, company);

            // Assert
            _apprenticeRepositoryMock.Verify(repo => repo.GetByNameAndCompanyAsync(firstName, lastName, company), Times.Once);
            _apprenticeRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Apprentice>()), Times.Once);
            _trainingRepositoryMock.Verify(repo => repo.CommitTrackedChangesAsync(), Times.Once);
            Assert.That(result.FirstName, Is.EqualTo(firstName));
            Assert.That(result.LastName, Is.EqualTo(lastName));
            Assert.That(result.Company, Is.EqualTo(company));
        }

        [Test]
        public async Task RegisterExternalApprentice_Should_RegisterExistingApprentice_If_AlreadyExists()
        {
            // Arrange
            string trainingCode = "Training-001";
            string firstName = "John";
            string lastName = "Doe";
            string company = "Company";

            var existingApprentice = Apprentice.CreateNew("123", firstName, lastName, company);
            var training = new TrainingBuilder().Build();

            _apprenticeRepositoryMock.Setup(repo => repo.GetByNameAndCompanyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(existingApprentice);
            _apprenticeRepositoryMock.Setup(repo => repo.GetByIdAsync(existingApprentice.Id))
                .ReturnsAsync(existingApprentice);
            _trainingRepositoryMock.Setup(repo => repo.GetByCodeAsync(It.IsAny<Code>()))
                .ReturnsAsync(training);

            // Act
            var result = await _service.RegisterExternalApprentice(trainingCode, firstName, lastName, company);

            // Assert
            _apprenticeRepositoryMock.Verify(repo => repo.GetByNameAndCompanyAsync(firstName, lastName, company), Times.Once);
            _apprenticeRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Apprentice>()), Times.Never);
            _trainingRepositoryMock.Verify(repo => repo.CommitTrackedChangesAsync(), Times.Once);
            Assert.That(result, Is.SameAs(existingApprentice));
        }

        [Test]
        public async Task FinishParticipation_Should_FinishParticipation_And_SaveChanges()
        {
            // Arrange
            string trainingCode = "Training-001";
            string apprenticeId = Guid.NewGuid().ToString();

            var training = new TrainingBuilder().Build();
            var apprentice = Apprentice.CreateNew(apprenticeId, "John", "Doe", "Company");
            var participation = Participation.CreateNew(trainingCode, apprenticeId);

            training.Register(apprentice);

            _trainingRepositoryMock.Setup(repo => repo.GetByCodeAsync(It.IsAny<Code>()))
                .ReturnsAsync(training);
            _apprenticeRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(apprentice);

            // Act
            var result = await _service.FinishParticipation(trainingCode, apprenticeId);

            // Assert
            _trainingRepositoryMock.Verify(repo => repo.GetByCodeAsync(It.IsAny<Code>()), Times.Once);
            _apprenticeRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.That(training.Participations, Has.Exactly(1).Matches<Participation>(p => p.ApprenticeId == apprenticeId && p.IsFinished));
            apprentice.AddXp(It.IsAny<int>());
            _eventBusMock.Verify(bus => bus.Publish(It.IsAny<EmployeeFinishedTrainingIntegrationEvent>(), default), Times.Once);
            _apprenticeRepositoryMock.Verify(repo => repo.CommitTrackedChangesAsync(), Times.Once);
            Assert.That(result, Is.SameAs(training.Participations.First(p => p.ApprenticeId == apprenticeId)));
        }

        [Test]
        public void FinishParticipation_Should_ThrowException_When_TrainingNotFound()
        {
            // Arrange
            string trainingCode = "Training-001";
            string apprenticeId = Guid.NewGuid().ToString();

            _trainingRepositoryMock.Setup(repo => repo.GetByCodeAsync(It.IsAny<Code>()))
                .ReturnsAsync((Domain.Training?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ContractException>(() => _service.FinishParticipation(trainingCode, apprenticeId));
            Assert.That(ex.Message, Is.EqualTo("The training with the given code does not exist."));
        }

        [Test]
        public void FinishParticipation_Should_ThrowException_When_ApprenticeNotFound()
        {
            // Arrange
            string apprenticeId = Guid.NewGuid().ToString();

            var training = new TrainingBuilder().Build();

            _trainingRepositoryMock.Setup(repo => repo.GetByCodeAsync(It.IsAny<Code>()))
                .ReturnsAsync(training);
            _apprenticeRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Apprentice?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ContractException>(() => _service.FinishParticipation(training.TrainingCode, apprenticeId));
            Assert.That(ex.Message, Is.EqualTo("The apprentice with the given id does not exist."));
        }
    }
}