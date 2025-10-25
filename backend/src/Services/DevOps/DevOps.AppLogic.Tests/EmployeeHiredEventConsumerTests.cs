using DevOps.AppLogic.Events;
using DevOps.AppLogic.Tests.Builders;
using DevOps.Domain;
using DevOps.Domain.Tests.Builders;
using IntegrationEvents.Employee;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevOps.AppLogic.Tests;

public class EmployeeHiredEventConsumerTests
{
    private Mock<IDeveloperRepository> _developerRepositoryMock = null!;
    private EmployeeHiredEventConsumer _consumer = null!;

    [SetUp]
    public void Setup()
    {
        _developerRepositoryMock = new Mock<IDeveloperRepository>();
        var logger = new Mock<ILogger<EmployeeHiredEventConsumer>>();

        _consumer = new EmployeeHiredEventConsumer(_developerRepositoryMock.Object, logger.Object);
    }

    [Test]
    public void Consume_DeveloperWithSameIdAlreadyExists_ShouldDoNothing()
    {
        //Arrange
        var existingDeveloper = new DeveloperBuilder().Build();
        _developerRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(existingDeveloper);
        EmployeeHiredIntegrationEvent @event = new EmployeeHiredIntegrationEventBuilder().Build();

        //Act
        _consumer.Consume(GetContextForEvent(@event)).Wait();

        //Assert
        _developerRepositoryMock.Verify(repo => repo.GetByIdAsync(@event.Number), Times.Once);
        _developerRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Developer>()), Times.Never);
    }

    [Test]
    public void Consume_DeveloperDoesNotExistYet_ShouldAddDeveloper()
    {
        //Arrange
        _developerRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);
        EmployeeHiredIntegrationEvent @event = new EmployeeHiredIntegrationEventBuilder().Build();

        //Act
        _consumer.Consume(GetContextForEvent(@event)).Wait();

        //Assert
        _developerRepositoryMock.Verify(repo => repo.GetByIdAsync(@event.Number), Times.Once);
        _developerRepositoryMock.Verify(
            repo => repo.AddAsync(It.Is<Developer>(d =>
                d.Id == @event.Number &&
                d.FirstName == @event.FirstName &&
                d.LastName == @event.LastName &&
                d.Rating == 0.0)), Times.Once);
    }

    private ConsumeContext<EmployeeHiredIntegrationEvent> GetContextForEvent(EmployeeHiredIntegrationEvent @event)
    {
        var consumeContextMock = new Mock<ConsumeContext<EmployeeHiredIntegrationEvent>>();
        consumeContextMock.SetupGet(c => c.Message).Returns(@event);
        return consumeContextMock.Object;
    }
}