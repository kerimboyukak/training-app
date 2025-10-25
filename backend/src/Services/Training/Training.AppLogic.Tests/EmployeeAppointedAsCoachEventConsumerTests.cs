using IntegrationEvents.Employee;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Training.AppLogic.Events;
using Training.AppLogic.Tests.Builders;
using Training.Domain;

namespace Training.AppLogic.Tests
{
    public class EmployeeAppointedAsCoachEventConsumerTests
    {
        private Mock<ICoachRepository> _coachRepositoryMock = null!;
        private EmployeeAppointedAsCoachEventConsumer _consumer = null!;

        [SetUp]
        public void Setup()
        {
            _coachRepositoryMock = new Mock<ICoachRepository>();
            var logger = new Mock<ILogger<EmployeeAppointedAsCoachEventConsumer>>();

            _consumer = new EmployeeAppointedAsCoachEventConsumer(_coachRepositoryMock.Object, logger.Object);
        }

        [Test]
        public void Consume_CoachWithSameIdAlreadyExists_ShouldDoNothing()
        {
            // Arrange
            var existingCoach = Coach.CreateNew("123", "John", "Doe");
            _coachRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(existingCoach);
            var @event = new EmployeeAppointedAsCoachIntegrationEventBuilder().Build();

            // Act
            _consumer.Consume(GetContextForEvent(@event)).Wait();

            // Assert
            _coachRepositoryMock.Verify(repo => repo.GetByIdAsync(@event.Number), Times.Once);
            _coachRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Coach>()), Times.Never);
        }

        [Test]
        public void Consume_CoachDoesNotExistYet_ShouldAddCoach()
        {
            // Arrange
            _coachRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Coach?)null);
            var @event = new EmployeeAppointedAsCoachIntegrationEventBuilder().Build();

            // Act
            _consumer.Consume(GetContextForEvent(@event)).Wait();

            // Assert
            _coachRepositoryMock.Verify(repo => repo.GetByIdAsync(@event.Number), Times.Once);
            _coachRepositoryMock.Verify(
                repo => repo.AddAsync(It.Is<Coach>(c =>
                    c.Id == @event.Number &&
                    c.FirstName == @event.FirstName &&
                    c.LastName == @event.LastName)), Times.Once);
        }

        private ConsumeContext<EmployeeAppointedAsCoachIntegrationEvent> GetContextForEvent(EmployeeAppointedAsCoachIntegrationEvent @event)
        {
            var consumeContextMock = new Mock<ConsumeContext<EmployeeAppointedAsCoachIntegrationEvent>>();
            consumeContextMock.SetupGet(c => c.Message).Returns(@event);
            return consumeContextMock.Object;
        }
    }
}