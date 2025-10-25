using Domain;
using Test;

namespace Training.Domain.Tests
{
    public class CodeTests
    {
        [Test]
        public void Constructor_FromNameAndSequence_ShouldInitializeProperly()
        {
            //Arrange
            string name = Random.Shared.NextString().Substring(0, 11);
            int sequence = Random.Shared.Next(1, 1000);

            //Act
            var code = new Code(name, sequence);

            //Assert
            Assert.That(code.Name, Is.EqualTo(name));
            Assert.That(code.Sequence, Is.EqualTo(sequence));
        }

        [Test]
        public void Constructor_FromNameAndSequence_InvalidSequence_ShouldThrowContractException()
        {
            //Arrange
            string name = Random.Shared.NextString().Substring(0, 11);
            int invalidSequence = Random.Shared.Next(1000, int.MaxValue);

            //Act + Assert
            Assert.That(() => new Code(name, invalidSequence), Throws.InstanceOf<ContractException>());
        }


        [TestCase("Name-001", "Name", 1)]
        [TestCase("LongName-123", "LongName", 123)]
        public void Constructor_FromString_ShouldInitializeProperly(string input, string expectedName, int expectedSequence)
        {
            //Act
            var code = new Code(input);

            //Assert
            Assert.That(code.Name, Is.EqualTo(expectedName));
            Assert.That(code.Sequence, Is.EqualTo(expectedSequence));
        }

        [TestCase("Name")]
        [TestCase("-001")]
        [TestCase("Name-0")]
        [TestCase("Name--001")]
        [TestCase("Name-1000")]
        public void Constructor_FromString_InvalidInput_ShouldThrowContractException(string input)
        {
            Assert.That(() => new Code(input), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void Constructor_FromString_EmptyName_ShouldThrowContractException()
        {
            //Arrange
            string input = "-001";

            //Act + Assert
            Assert.That(() => new Code(input), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void Constructor_FromString_NullName_ShouldThrowContractException()
        {
            //Arrange
            string input = string.Empty;

            //Act + Assert
            Assert.That(() => new Code(input), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void Codes_WithSameNameAndSequence_ShouldBeEqual()
        {
            // Arrange
            var code1 = new Code("Name", 1);
            var code2 = new Code("Name", 1);

            // Act & Assert
            Assert.That(code1, Is.EqualTo(code2));
        }

        [Test]
        public void Codes_WithDifferentName_ShouldNotBeEqual()
        {
            // Arrange
            var code1 = new Code("Name1", 1);
            var code2 = new Code("Name2", 1);

            // Act & Assert
            Assert.That(code1, Is.Not.EqualTo(code2));
        }

        [Test]
        public void Codes_WithDifferentSequence_ShouldNotBeEqual()
        {
            // Arrange
            var code1 = new Code("Name", 1);
            var code2 = new Code("Name", 2);

            // Act & Assert
            Assert.That(code1, Is.Not.EqualTo(code2));
        }


        [TestCaseSource(nameof(ToStringCases))]
        public void ToString_ShouldCorrectlyConvert(string name, int sequence, string expected)
        {
            Code code = new Code(name, sequence);
            string result = code.ToString();
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(ToStringCases))]
        public void ImplicitConvertToString_ShouldCorrectlyConvert(string name, int sequence, string expected)
        {
            Code code = new Code(name, sequence);
            string result = code;
            Assert.That(result, Is.EqualTo(expected));
        }

        private static object[] ToStringCases =
        {
            new object[] { "Name", 1, "Name-1" },
            new object[] { "LongName", 123, "LongName-123" }
        };

    }
}