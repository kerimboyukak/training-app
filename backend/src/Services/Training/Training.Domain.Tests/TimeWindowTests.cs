using Domain;
using NUnit.Framework;
using System;

namespace Training.Domain.Tests
{
    public class TimeWindowTests
    {
        private DateTime _start;
        private DateTime _end;

        [SetUp]
        public void Setup()
        {
            _start = DateTime.Now;
            _end = _start.AddHours(1);
        }

        [Test]
        public void Constructor_ValidInput_ShouldInitializeFieldsCorrectly()
        {
            // Act
            TimeWindow timeWindow = new TimeWindow(_start, _end);

            // Assert
            Assert.That(timeWindow.Start, Is.EqualTo(_start));
            Assert.That(timeWindow.End, Is.EqualTo(_end));
        }

        [Test]
        public void Constructor_EndBeforeStart_ShouldThrowContractException()
        {
            // Arrange
            DateTime invalidEnd = _start.AddHours(-1);

            // Assert
            Assert.That(() => new TimeWindow(_start, invalidEnd), Throws.InstanceOf<ContractException>());
        }

        [Test]
        public void Overlaps_OverlappingTimeWindow_ShouldReturnTrue()
        {
            // Arrange
            TimeWindow timeWindow1 = new TimeWindow(_start, _end);
            TimeWindow timeWindow2 = new TimeWindow(_start.AddMinutes(30), _end.AddMinutes(30));

            // Act
            bool result = timeWindow1.Overlaps(timeWindow2);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Overlaps_NonOverlappingTimeWindow_ShouldReturnFalse()
        {
            // Arrange
            TimeWindow timeWindow1 = new TimeWindow(_start, _end);
            TimeWindow timeWindow2 = new TimeWindow(_end.AddMinutes(1), _end.AddHours(2));

            // Act
            bool result = timeWindow1.Overlaps(timeWindow2);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ToString_ShouldReturnCorrectString()
        {
            // Arrange
            TimeWindow timeWindow = new TimeWindow(_start, _end);
            string expected = $"{_start:dd/MM/yyyy HH:mm} - {_end:dd/MM/yyyy HH:mm}";

            // Act
            string result = timeWindow.ToString();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ImplicitConvertToString_ShouldReturnCorrectString()
        {
            // Arrange
            TimeWindow timeWindow = new TimeWindow(_start, _end);
            string expected = $"{_start:dd/MM/yyyy HH:mm} - {_end:dd/MM/yyyy HH:mm}";

            // Act
            string result = timeWindow;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TimeWindows_WithSameStartAndEnd_ShouldBeEqual()
        {
            // Arrange
            var timeWindow1 = new TimeWindow(_start, _end);
            var timeWindow2 = new TimeWindow(_start, _end);

            // Act & Assert
            Assert.That(timeWindow1, Is.EqualTo(timeWindow2));
        }

        [Test]
        public void TimeWindows_WithDifferentEnd_ShouldNotBeEqual()
        {
            // Arrange
            var timeWindow1 = new TimeWindow(_start, _end);
            var differentEnd = _end.AddHours(1);
            var timeWindow2 = new TimeWindow(_start, differentEnd);

            // Act & Assert
            Assert.That(timeWindow1, Is.Not.EqualTo(timeWindow2));
        }
    }
}