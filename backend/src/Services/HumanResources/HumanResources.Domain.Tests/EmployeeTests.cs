using Domain;
using HumanResources.Domain.Tests.Builders;

namespace HumanResources.Domain.Tests
{
    public class EmployeeTests
    {
        [Test]
        public void Dismiss_WithoutNotice_ShouldSetEndDateOnToday()
        {
            //Arrange
            Employee employee = new EmployeeBuilder().WithEndDate(null).Build();

            //Act
            employee.Dismiss(withNotice: false);

            //Assert
            Assert.That(employee.EndDate, Is.EqualTo(DateTime.Now).Within(10).Seconds);
        }

        [Test]
        public void Dismiss_WithoutNotice_EmployeeAlreadyHasEndDate_ShouldSetEndDateOnToday()
        {
            //Arrange
            Employee employee = new EmployeeBuilder().WithEndDate(DateTime.Now.AddDays(5)).Build();

            //Act
            employee.Dismiss(withNotice: false);

            //Assert
            Assert.That(employee.EndDate, Is.EqualTo(DateTime.Now).Within(10).Seconds);
        }

        [Test]
        public void Dismiss_WithNotice_EmployeeAlreadyHasEndDate_ShouldThrowContractException()
        {
            //Arrange
            Employee employee = new EmployeeBuilder().WithEndDate(DateTime.Now.AddDays(5)).Build();

            //Act + Assert
            Assert.That(() => employee.Dismiss(withNotice: true), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void Dismiss_WithNotice_LessThan3MonthsInService_ShouldSetEndDateInOneWeek()
        {
            //Arrange

            DateTime lessThan3MonthsAgo = DateTime.Now.AddDays(-28);
            Employee employee = new EmployeeBuilder()
                .WithStartDate(lessThan3MonthsAgo)
                .WithEndDate(null)
                .Build();

            //Act
            employee.Dismiss(withNotice: true);

            //Assert
            DateTime over1Week = DateTime.Now.AddDays(7);
            Assert.That(employee.EndDate, Is.EqualTo(over1Week).Within(10).Seconds);
        }

        [Test]
        public void Dismiss_WithNotice_LessThan12MonthsInService_ShouldSetEndDateIn2Weeks()
        {
            //Arrange
            DateTime lessThan12MonthsAgo = DateTime.Now.AddMonths(-10);
            Employee employee = new EmployeeBuilder()
                .WithStartDate(lessThan12MonthsAgo)
                .WithEndDate(null)
                .Build();

            //Act
            employee.Dismiss(withNotice: true);

            //Assert
            DateTime over2Weeks = DateTime.Now.AddDays(14);
            Assert.That(employee.EndDate, Is.EqualTo(over2Weeks).Within(10).Seconds);
        }

        [Test]
        public void Dismiss_WithNotice_MoreThan12MonthsInService_ShouldSetEndDateIn4Weeks()
        {
            //Arrange
            DateTime moreThan12MonthsAgo = DateTime.Now.AddYears(-1);
            Employee employee = new EmployeeBuilder()
                .WithStartDate(moreThan12MonthsAgo)
                .WithEndDate(null)
                .Build();

            //Act
            employee.Dismiss(withNotice: true);

            //Assert
            DateTime over4Weeks = DateTime.Now.AddDays(28);
            Assert.That(employee.EndDate, Is.EqualTo(over4Weeks).Within(10).Seconds);
        }
        [Test]
        public void FinishTraining_ShouldIncreaseTrainingHours()
        {
            //Arrange
            Employee employee = new EmployeeBuilder().WithTrainingHours(0).Build();
            int trainingHours = 5;

            //Act
            employee.FinishTraining(trainingHours);

            //Assert
            Assert.That(employee.TrainingHours, Is.EqualTo(trainingHours));
        }

        [Test]
        public void FinishTraining_InvalidHours_ShouldThrowContractException()
        {
            //Arrange
            Employee employee = new EmployeeBuilder().WithTrainingHours(0).Build();

            //Act + Assert
            Assert.That(() => employee.FinishTraining(-1), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void AppointAsCoach_ShouldSetIsCoachToTrue()
        {
            //Arrange
            Employee employee = new EmployeeBuilder().WithIsCoach(false).Build();

            //Act
            employee.AppointAsCoach();

            //Assert
            Assert.That(employee.IsCoach, Is.True);
        }

        [Test]
        public void AppointAsCoach_AlreadyCoach_ShouldThrowContractException()
        {
            //Arrange
            Employee employee = new EmployeeBuilder().WithIsCoach(true).Build();

            //Act + Assert
            Assert.That(() => employee.AppointAsCoach(), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void EmployeesWithSameNumber_ShouldBeEqual()
        {
            //Arrange
            EmployeeNumber employeeNumber = new EmployeeNumber(DateTime.Now, 1);
            Employee employee1 = new EmployeeBuilder().WithNumber(employeeNumber).Build();
            Employee employee2 = new EmployeeBuilder().WithNumber(employeeNumber).Build();

            //Act + Assert
            Assert.That(employee1, Is.EqualTo(employee2));
        }
    }
}