using HumanResources.AppLogic.Events;
using HumanResources.AppLogic.Tests.Builders;
using HumanResources.Domain;
using IntegrationEvents.Employee;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace HumanResources.AppLogic.Tests
{
    public class EmployeeFinishedTrainingEventConsumerTests
    {
        private Mock<IEmployeeRepository> _employeeRepositoryMock = null!;
        private EmployeeFinishedTrainingEventConsumer _consumer = null!;

        [SetUp]
        public void Setup()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            var logger = new Mock<ILogger<EmployeeFinishedTrainingEventConsumer>>();

            _consumer = new EmployeeFinishedTrainingEventConsumer(_employeeRepositoryMock.Object, logger.Object);
        }

        [Test]
        public void Consume_EmployeeDoesNotExist_ShouldLogAndDoNothing()
        {
            // Arrange
            _employeeRepositoryMock.Setup(repo => repo.GetByNumberAsync(It.IsAny<EmployeeNumber>())).ReturnsAsync((IEmployee?)null);
            var @event = new EmployeeFinishedTrainingIntegrationEventBuilder()
                .Build();

            // Act
            _consumer.Consume(GetContextForEvent(@event)).Wait();

            // Assert
            _employeeRepositoryMock.Verify(repo => repo.GetByNumberAsync(@event.EmployeeNumber), Times.Once);
            _employeeRepositoryMock.Verify(repo => repo.CommitTrackedChangesAsync(), Times.Never);
        }

        [Test]
        public void Consume_EmployeeExists_ShouldFinishTrainingAndCommitChanges()
        {
            // Arrange
            var employeeMock = new Mock<IEmployee>();
            _employeeRepositoryMock.Setup(repo => repo.GetByNumberAsync(It.IsAny<EmployeeNumber>())).ReturnsAsync(employeeMock.Object);
            var @event = new EmployeeFinishedTrainingIntegrationEventBuilder()
                .Build();

            // Act
            _consumer.Consume(GetContextForEvent(@event)).Wait();

            // Assert
            _employeeRepositoryMock.Verify(repo => repo.GetByNumberAsync(@event.EmployeeNumber), Times.Once);
            employeeMock.Verify(e => e.FinishTraining(@event.TrainingHours), Times.Once);
            _employeeRepositoryMock.Verify(repo => repo.CommitTrackedChangesAsync(), Times.Once);
        }

        private ConsumeContext<EmployeeFinishedTrainingIntegrationEvent> GetContextForEvent(EmployeeFinishedTrainingIntegrationEvent @event)
        {
            var consumeContextMock = new Mock<ConsumeContext<EmployeeFinishedTrainingIntegrationEvent>>();
            consumeContextMock.SetupGet(c => c.Message).Returns(@event);
            return consumeContextMock.Object;
        }
    }
}