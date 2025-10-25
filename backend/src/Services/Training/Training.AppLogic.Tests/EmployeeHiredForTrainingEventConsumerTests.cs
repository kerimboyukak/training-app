using IntegrationEvents.Employee;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Training.AppLogic.Events;
using Training.AppLogic.Tests.Builders;
using Training.Domain;

namespace Training.AppLogic.Tests
{
    public class EmployeeHiredForTrainingEventConsumerTests
    {
        private Mock<IApprenticeRepository> _apprenticeRepositoryMock = null!;
        private EmployeeHiredForTrainingEventConsumer _consumer = null!;

        [SetUp]
        public void Setup()
        {
            _apprenticeRepositoryMock = new Mock<IApprenticeRepository>();
            var logger = new Mock<ILogger<EmployeeHiredForTrainingEventConsumer>>();

            _consumer = new EmployeeHiredForTrainingEventConsumer(_apprenticeRepositoryMock.Object, logger.Object);
        }

        [Test]
        public void Consume_ApprenticeWithSameIdAlreadyExists_ShouldDoNothing()
        {
            // Arrange
            var existingApprentice = Apprentice.CreateNew("123", "John", "Doe", "Company");
            _apprenticeRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(existingApprentice);
            var @event = new EmployeeHiredForTrainingIntegrationEventBuilder().Build();

            // Act
            _consumer.Consume(GetContextForEvent(@event)).Wait();

            // Assert
            _apprenticeRepositoryMock.Verify(repo => repo.GetByIdAsync(@event.Number), Times.Once);
            _apprenticeRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Apprentice>()), Times.Never);
        }

        [Test]
        public void Consume_ApprenticeDoesNotExistYet_ShouldAddApprentice()
        {
            // Arrange
            _apprenticeRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Apprentice?)null);
            var @event = new EmployeeHiredForTrainingIntegrationEventBuilder().Build();

            // Act
            _consumer.Consume(GetContextForEvent(@event)).Wait();

            // Assert
            _apprenticeRepositoryMock.Verify(repo => repo.GetByIdAsync(@event.Number), Times.Once);
            _apprenticeRepositoryMock.Verify(
                repo => repo.AddAsync(It.Is<Apprentice>(a =>
                    a.Id == @event.Number &&
                    a.FirstName == @event.FirstName &&
                    a.LastName == @event.LastName &&
                    a.Company == @event.Company)), Times.Once);
        }

        private ConsumeContext<EmployeeHiredForTrainingIntegrationEvent> GetContextForEvent(EmployeeHiredForTrainingIntegrationEvent @event)
        {
            var consumeContextMock = new Mock<ConsumeContext<EmployeeHiredForTrainingIntegrationEvent>>();
            consumeContextMock.SetupGet(c => c.Message).Returns(@event);
            return consumeContextMock.Object;
        }
    }
}