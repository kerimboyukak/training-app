using Domain;
using Test;

namespace DevOps.Domain.Tests
{
    public class DeveloperTests
    {
        private string _id = null!;
        //About the = null!; -> We tell the compiler that this non-nullable reference type variable
        //will be initialized later, and it is ok to be null here. This removes the compiler warning.
        private string _firstName = null!;
        private string _lastName = null!;
        private Percentage _rating = null!;

        [SetUp]
        public void BeforeEachTest()
        {
            _id = Random.Shared.NextString();
            _firstName = Random.Shared.NextString();
            _lastName = Random.Shared.NextString();
            _rating = Random.Shared.NextDouble();
        }

        [Test]
        public void CreateNew_ValidInput_ShouldInitializeFieldsCorrectly()
        {
            //Act
            Developer developer = Developer.CreateNew(_id, _firstName, _lastName, _rating);

            //Assert
            Assert.That(developer.Id, Is.EqualTo(_id));
            Assert.That(developer.FirstName, Is.EqualTo(_firstName));
            Assert.That(developer.LastName, Is.EqualTo(_lastName));
            Assert.That(developer.Rating, Is.EqualTo(_rating));
        }

        [Test]
        public void CreateNew_EmptyId_ShouldThrowContractException()
        {
            Assert.That(() => Developer.CreateNew("", _firstName, _lastName, _rating), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void CreateNew_EmptyFirstName_ShouldThrowContractException()
        {
            Assert.That(() => Developer.CreateNew(_id, "", _lastName, _rating), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void CreateNew_EmptyLastName_ShouldThrowContractException()
        {
            Assert.That(() => Developer.CreateNew(_id, _firstName, "", _rating), Throws.InstanceOf<ContractException>());
        }
    }
}