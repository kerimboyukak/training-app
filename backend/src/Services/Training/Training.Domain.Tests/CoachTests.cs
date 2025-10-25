using Domain;
using Test;

namespace Training.Domain.Tests
{
    public class CoachTests
    {
        private string _id = null!;
        private string _firstName = null!;
        private string _lastName = null!;

        [SetUp]
        public void BeforeEachTest()
        {
            _id = Random.Shared.NextString();
            _firstName = Random.Shared.NextString();
            _lastName = Random.Shared.NextString();
        }

        [Test]
        public void CreateNew_ValidInput_ShouldInitializeFieldsCorrectly()
        {
            // Act
            Coach coach = Coach.CreateNew(_id, _firstName, _lastName);

            // Assert
            Assert.That(coach.Id, Is.EqualTo(_id));
            Assert.That(coach.FirstName, Is.EqualTo(_firstName));
            Assert.That(coach.LastName, Is.EqualTo(_lastName));
        }

        [Test]
        public void CreateNew_EmptyId_ShouldThrowContractException()
        {
            Assert.That(() => Coach.CreateNew("", _firstName, _lastName), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void CreateNew_EmptyFirstName_ShouldThrowContractException()
        {
            Assert.That(() => Coach.CreateNew(_id, "", _lastName), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void CreateNew_EmptyLastName_ShouldThrowContractException()
        {
            Assert.That(() => Coach.CreateNew(_id, _firstName, ""), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void Coaches_WithSameId_ShouldBeEqual()
        {
            // Arrange
            var coach1 = Coach.CreateNew(_id, _firstName, _lastName);
            var coach2 = Coach.CreateNew(_id, "DifferentFirstName", "DifferentLastName");

            // Act & Assert
            Assert.That(coach1, Is.EqualTo(coach2));
        }

        [Test]
        public void Coaches_WithDifferentId_ShouldNotBeEqual()
        {
            // Arrange
            var coach1 = Coach.CreateNew(_id, _firstName, _lastName);
            var differentId = Random.Shared.NextString();
            var coach2 = Coach.CreateNew(differentId, _firstName, _lastName);

            // Act & Assert
            Assert.That(coach1, Is.Not.EqualTo(coach2));
        }
    }
}