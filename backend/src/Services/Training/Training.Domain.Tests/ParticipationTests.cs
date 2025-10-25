using Domain;
using NUnit.Framework;
using System;

namespace Training.Domain.Tests
{
    public class ParticipationTests
    {
        private string _trainingCode = null!;
        private string _apprenticeId = null!;

        [SetUp]
        public void Setup()
        {
            _trainingCode = "Training-1";
            _apprenticeId = Guid.NewGuid().ToString();
        }

        [Test]
        public void CreateNew_ValidInput_ShouldInitializeFieldsCorrectly()
        {
            // Act
            Participation participation = Participation.CreateNew(_trainingCode, _apprenticeId);

            // Assert
            Assert.That(participation.TrainingCode.ToString(), Is.EqualTo(_trainingCode));
            Assert.That(participation.ApprenticeId, Is.EqualTo(_apprenticeId));
            Assert.That(participation.IsFinished, Is.False);
        }

        [Test]
        public void Finish_NotFinished_ShouldMarkAsFinished()
        {
            // Arrange
            Participation participation = Participation.CreateNew(_trainingCode, _apprenticeId);

            // Act
            participation.Finish();

            // Assert
            Assert.That(participation.IsFinished, Is.True);
        }

        [Test]
        public void Finish_AlreadyFinished_ShouldThrowContractException()
        {
            // Arrange
            Participation participation = Participation.CreateNew(_trainingCode, _apprenticeId);
            participation.Finish();

            // Assert
            Assert.That(() => participation.Finish(), Throws.InstanceOf<ContractException>());
        }
        [Test]
        public void Participations_WithSameTrainingCodeAndApprenticeId_ShouldBeEqual()
        {
            // Arrange
            var participation1 = Participation.CreateNew(_trainingCode, _apprenticeId);
            var participation2 = Participation.CreateNew(_trainingCode, _apprenticeId);

            // Act & Assert
            Assert.That(participation1, Is.EqualTo(participation2));
        }

        [Test]
        public void Participations_WithDifferentTrainingCode_ShouldNotBeEqual()
        {
            // Arrange
            var participation1 = Participation.CreateNew(_trainingCode, _apprenticeId);
            var differentTrainingCode = "Training-2";
            var participation2 = Participation.CreateNew(differentTrainingCode, _apprenticeId);

            // Act & Assert
            Assert.That(participation1, Is.Not.EqualTo(participation2));
        }

        [Test]
        public void Participations_WithDifferentApprenticeId_ShouldNotBeEqual()
        {
            // Arrange
            var participation1 = Participation.CreateNew(_trainingCode, _apprenticeId);
            var differentApprenticeId = Guid.NewGuid().ToString();
            var participation2 = Participation.CreateNew(_trainingCode, differentApprenticeId);

            // Act & Assert
            Assert.That(participation1, Is.Not.EqualTo(participation2));
        }

        [Test]
        public void Participation_EmptyConstructor_ShouldInitializeFieldsCorrectly()
        {
            // Act
            var participation = (Participation)Activator.CreateInstance(typeof(Participation), true)!;

            // Assert
            Assert.That(participation.TrainingCode, Is.Null);
            Assert.That(participation.ApprenticeId, Is.Null);
            Assert.That(participation.IsFinished, Is.False);
            Assert.That(participation.Apprentice, Is.Null);
            Assert.That(participation.Training, Is.Null);
        }

        [Test]
        public void CreateNew_WithApprenticeAndTraining_ShouldInitializeFieldsCorrectly()
        {
            // Arrange
            var apprentice = Apprentice.CreateNew(_apprenticeId, "John", "Doe", "Company");
            var training = Training.CreateNew("Training", "Description", 10, new Code("Room", 1), "Coach", new TimeWindow(DateTime.Now, DateTime.Now.AddHours(1)), 1);

            // Act
            var participation = (Participation)Activator.CreateInstance(typeof(Participation), true)!;
            participation.GetType().GetProperty("Apprentice")!.SetValue(participation, apprentice);
            participation.GetType().GetProperty("Training")!.SetValue(participation, training);

            // Assert
            Assert.That(participation.Apprentice, Is.EqualTo(apprentice));
            Assert.That(participation.Training, Is.EqualTo(training));
        }
    }
}