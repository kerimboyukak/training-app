using HumanResources.AppLogic;
using HumanResources.Domain;
using IntegrationEvents.Employee;
using MassTransit;
using Moq;
using Test;

namespace HumanResources.AppLogic.Tests;

public class EmployeeServiceTests
{
    private Mock<IEmployeeFactory> _employeeFactoryMock = null!;
    private Mock<IEmployeeRepository> _employeeRepositoryMock = null!;
    private Mock<IPublishEndpoint> _eventBusMock = null!;
    private EmployeeService _service = null!;

    [SetUp]
    public void Setup()
    {
        _employeeFactoryMock = new Mock<IEmployeeFactory>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _eventBusMock = new Mock<IPublishEndpoint>();

        _service = new EmployeeService(_employeeFactoryMock.Object, _employeeRepositoryMock.Object, _eventBusMock.Object);
    }

    [Test]
    public void HireNewAsync_Should_RetrieveNumberOfStarters_CreateTheEmployee_AndSaveIt()
    {
        //Arrange
        string lastName = Random.Shared.NextString();
        string firstName = Random.Shared.NextString();
        DateTime startDate = DateTime.Now;
        EmployeeType employeeType = EmployeeType.Developer; // Example type

        int numberOfStartersOnStartDate = Random.Shared.Next(1, 1000);
        _employeeRepositoryMock.Setup(repo => repo.GetNumberOfStartersOnAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(numberOfStartersOnStartDate);

        var createdEmployeeMock = new Mock<IEmployee>();
        createdEmployeeMock.SetupGet(e => e.Number).Returns(new EmployeeNumber(startDate, 1));
        createdEmployeeMock.SetupGet(e => e.FirstName).Returns(firstName);
        createdEmployeeMock.SetupGet(e => e.LastName).Returns(lastName);
        createdEmployeeMock.SetupGet(e => e.Type).Returns(employeeType);
        IEmployee createdEmployee = createdEmployeeMock.Object;

        _employeeFactoryMock
            .Setup(factory =>
                factory.CreateNew(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<EmployeeType>()))
            .Returns(createdEmployee);

        //Act
        IEmployee result = _service.HireNewAsync(lastName, firstName, startDate, employeeType).Result;

        //Assert
        _employeeRepositoryMock.Verify(repo => repo.GetNumberOfStartersOnAsync(startDate), Times.Once);
        int expectedSequence = numberOfStartersOnStartDate + 1;
        _employeeFactoryMock.Verify(
            factory => factory.CreateNew(lastName, firstName, startDate, expectedSequence, employeeType), Times.Once);
        _employeeRepositoryMock.Verify(repo => repo.AddAsync(createdEmployee), Times.Once);
        Assert.That(result, Is.SameAs(createdEmployee));

        _eventBusMock.Verify(
            bus => bus.Publish(It.Is<EmployeeHiredIntegrationEvent>(@event =>
                @event.Number == createdEmployee.Number &&
                @event.FirstName == firstName &&
                @event.LastName == lastName), CancellationToken.None), Times.Once); // CancellationToken.None is the default value, Moq cannot handle default values -> it expects a value for each parameter
    }

    [Test]
    public void DismissAsync_Should_RetrieveEmployeeFromRepository_DismissTheEmployee_AndSaveTheChanges()
    {
        //Arrange
        EmployeeNumber employeeNumber = new EmployeeNumber(DateTime.Now, 1);

        Mock<IEmployee> employeeToDismissMock = new Mock<IEmployee>();
        IEmployee employeeToDismiss = employeeToDismissMock.Object;

        _employeeRepositoryMock.Setup(repo => repo.GetByNumberAsync(It.IsAny<EmployeeNumber>()))
            .ReturnsAsync(employeeToDismiss);

        //Act
        _service.DismissAsync(employeeNumber, true).Wait();

        //Assert
        _employeeRepositoryMock.Verify(repo => repo.GetByNumberAsync(employeeNumber), Times.Once);
        employeeToDismissMock.Verify(e => e.Dismiss(true), Times.Once);
        _employeeRepositoryMock.Verify(repo => repo.CommitTrackedChangesAsync(), Times.Once);
    }

    [Test]
    public void AppointEmployeeAsCoach_Should_RetrieveEmployeeFromRepository_AppointAsCoach_AndPublishEvent()
    {
        //Arrange
        EmployeeNumber employeeNumber = new EmployeeNumber(DateTime.Now, 1);

        Mock<IEmployee> employeeToAppointMock = new Mock<IEmployee>();
        employeeToAppointMock.SetupGet(e => e.Number).Returns(employeeNumber);
        employeeToAppointMock.SetupGet(e => e.FirstName).Returns("John");
        employeeToAppointMock.SetupGet(e => e.LastName).Returns("Doe");
        IEmployee employeeToAppoint = employeeToAppointMock.Object;

        _employeeRepositoryMock.Setup(repo => repo.GetByNumberAsync(It.IsAny<EmployeeNumber>()))
            .ReturnsAsync(employeeToAppoint);

        //Act
        _service.AppointEmployeeAsCoach(employeeNumber).Wait();

        //Assert
        _employeeRepositoryMock.Verify(repo => repo.GetByNumberAsync(employeeNumber), Times.Once);
        employeeToAppointMock.Verify(e => e.AppointAsCoach(), Times.Once);
        _employeeRepositoryMock.Verify(repo => repo.CommitTrackedChangesAsync(), Times.Once);

        _eventBusMock.Verify(
            bus => bus.Publish(It.Is<EmployeeAppointedAsCoachIntegrationEvent>(@event =>
                @event.Number == employeeToAppoint.Number &&
                @event.FirstName == employeeToAppoint.FirstName &&
                @event.LastName == employeeToAppoint.LastName), CancellationToken.None), Times.Once);
    }
}