using Domain;
using Test;

namespace HumanResources.Domain.Tests
{
    public class EmployeeFactoryTests
    {
        private Employee.Factory _factory = null!;
        //About the = null!; -> We tell the compiler that this non-nullable reference type variable
        //will be initialized later, and it is ok to be null here. This removes the compiler warning.
        private string _lastName = null!;
        private string _firstName = null!;
        private DateTime _startDate;
        private int _sequence;
        private EmployeeType _employeeType;

        [SetUp]
        public void BeforeEachTest()
        {
            _factory = new Employee.Factory();

            _lastName = Random.Shared.NextString();
            _firstName = Random.Shared.NextString();
            _startDate = Random.Shared.NextDateTimeInFuture();
            _sequence = Random.Shared.Next(1, 1000);
            _employeeType = EmployeeType.Developer; // Example type
        }

        [Test]
        public void CreateNew_ValidInput_ShouldInitializeFieldsCorrectly()
        {
            //Act
            IEmployee employee = _factory.CreateNew(_lastName, _firstName, _startDate, _sequence, _employeeType);

            //Assert
            Assert.That(employee.Number, Is.EqualTo(new EmployeeNumber(_startDate, _sequence)));
            Assert.That(employee.FirstName, Is.EqualTo(_firstName));
            Assert.That(employee.LastName, Is.EqualTo(_lastName));
            Assert.That(employee.StartDate, Is.EqualTo(_startDate));
            Assert.That(employee.EndDate, Is.Null);
            Assert.That(employee.Type, Is.EqualTo(_employeeType));
        }

        [Test]
        public void CreateNew_StartDateMoreThanAYearAgo_ShouldThrowContractException()
        {
            DateTime invalidStartDate = DateTime.Now.AddYears(-1);
            Assert.That(() => _factory.CreateNew(_lastName, _firstName, invalidStartDate, _sequence, _employeeType), Throws.InstanceOf<ContractException>());
        }

        [TestCase("")]
        [TestCase("a")]
        public void CreateNew_FirstNameTooShort_ShouldThrowContractException(string invalidFirstName)
        {
            Assert.That(() => _factory.CreateNew(_lastName, invalidFirstName, _startDate, _sequence, _employeeType), Throws.InstanceOf<ContractException>());
        }

        [TestCase("")]
        [TestCase("a")]
        public void CreateNew_LastNameTooShort_ShouldThrowContractException(string invalidLastName)
        {
            Assert.That(() => _factory.CreateNew(invalidLastName, _firstName, _startDate, _sequence, _employeeType), Throws.InstanceOf<ContractException>());
        }
    }
}