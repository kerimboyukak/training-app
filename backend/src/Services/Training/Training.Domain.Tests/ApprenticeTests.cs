using Domain;
using Test;

namespace Training.Domain.Tests
{
    public class ApprenticeTests
    {
        private string _id = null!;
        private string _firstName = null!;
        private string _lastName = null!;
        private string _company = null!;

        [SetUp]
        public void BeforeEachTest()
        {
            _id = Random.Shared.NextString();
            _firstName = Random.Shared.NextString();
            _lastName = Random.Shared.NextString();
            _company = Random.Shared.NextString();
        }

        [Test]
        public void CreateNew_ValidInput_ShouldInitializeFieldsCorrectly()
        {
            // Act
            Apprentice apprentice = Apprentice.CreateNew(_id, _firstName, _lastName, _company);

            // Assert
            Assert.That(apprentice.Id, Is.EqualTo(_id));
            Assert.That(apprentice.FirstName, Is.EqualTo(_firstName));
            Assert.That(apprentice.LastName, Is.EqualTo(_lastName));
            Assert.That(apprentice.Company, Is.EqualTo(_company));
            Assert.That(apprentice.Xp, Is.EqualTo(0));
        }

        [Test]
        public void CreateNew_EmptyId_ShouldThrowContractException()
        {
            Assert.That(() => Apprentice.CreateNew("", _firstName, _lastName, _company), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void CreateNew_EmptyFirstName_ShouldThrowContractException()
        {
            Assert.That(() => Apprentice.CreateNew(_id, "", _lastName, _company), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void CreateNew_EmptyLastName_ShouldThrowContractException()
        {
            Assert.That(() => Apprentice.CreateNew(_id, _firstName, "", _company), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void CreateNew_EmptyCompany_ShouldThrowContractException()
        {
            Assert.That(() => Apprentice.CreateNew(_id, _firstName, _lastName, ""), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void AddXp_ValidXp_ShouldIncreaseXp()
        {
            // Arrange
            Apprentice apprentice = Apprentice.CreateNew(_id, _firstName, _lastName, _company);
            int initialXp = apprentice.Xp;
            int xpToAdd = 10;

            // Act
            apprentice.AddXp(xpToAdd);

            // Assert
            Assert.That(apprentice.Xp, Is.EqualTo(initialXp + xpToAdd));
        }

        [Test]
        public void AddXp_NegativeXp_ShouldThrowContractException()
        {
            // Arrange
            Apprentice apprentice = Apprentice.CreateNew(_id, _firstName, _lastName, _company);

            // Act + Assert
            Assert.That(() => apprentice.AddXp(-1), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void Apprentices_WithSameId_ShouldBeEqual()
        {
            // Arrange
            var apprentice1 = Apprentice.CreateNew(_id, _firstName, _lastName, _company);
            var apprentice2 = Apprentice.CreateNew(_id, "DifferentFirstName", "DifferentLastName", "DifferentCompany");

            // Act & Assert
            Assert.That(apprentice1, Is.EqualTo(apprentice2));
        }

        [Test]
        public void Apprentices_WithDifferentId_ShouldNotBeEqual()
        {
            // Arrange
            var apprentice1 = Apprentice.CreateNew(_id, _firstName, _lastName, _company);
            var differentId = Random.Shared.NextString();
            var apprentice2 = Apprentice.CreateNew(differentId, _firstName, _lastName, _company);

            // Act & Assert
            Assert.That(apprentice1, Is.Not.EqualTo(apprentice2));
        }

    }
}