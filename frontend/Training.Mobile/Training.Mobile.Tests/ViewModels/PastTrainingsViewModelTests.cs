using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Training.Mobile.Messages;
using Training.Mobile.Models;
using Training.Mobile.Services;
using Training.Mobile.Services.Backend;
using Training.Mobile.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace Training.Mobile.Tests.ViewModels
{
    [TestFixture]
    public class PastTrainingsViewModelTests
    {
        private Mock<ITrainingService> _trainingServiceMock;
        private Mock<IToastService> _toastServiceMock;
        private Mock<INavigationService> _navigationServiceMock;
        private PastTrainingsViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _trainingServiceMock = new Mock<ITrainingService>();
            _toastServiceMock = new Mock<IToastService>();
            _navigationServiceMock = new Mock<INavigationService>();

            _viewModel = new PastTrainingsViewModel(_trainingServiceMock.Object, _toastServiceMock.Object, _navigationServiceMock.Object);
        }

        [Test]
        public void Constructor_InitializesCommandsCorrectly()
        {
            // Assert
            Assert.IsNotNull(_viewModel.LoadTrainingsCommand);
            Assert.IsNotNull(_viewModel.TrainingTapped);
        }

        [Test]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Assert
            Assert.IsNotNull(_viewModel.Trainings);
        }

        [Test]
        public async Task LoadTrainingsCommand_ExecutesExecuteLoadTrainingsCommand()
        {
            // Arrange
            var trainings = new List<TrainingDetail>
            {
                new TrainingDetail { Code = "Training-1", Name = "Training 1" },
                new TrainingDetail { Code = "Training-2", Name = "Training 2" }
            };
            _trainingServiceMock.Setup(s => s.GetPastTrainingsAsync()).ReturnsAsync(trainings);

            // Act
            await Task.Run(() => _viewModel.LoadTrainingsCommand.Execute(null));

            // Assert
            Assert.That(_viewModel.Trainings.Count, Is.EqualTo(2));
            Assert.That(_viewModel.Trainings[0].Code, Is.EqualTo("Training-1"));
            Assert.That(_viewModel.Trainings[1].Code, Is.EqualTo("Training-2"));
        }

        [Test]
        public async Task LoadTrainingsCommand_WhenServiceThrowsException_DisplaysToast()
        {
            // Arrange
            var exceptionMessage = "Error loading past trainings";
            _trainingServiceMock.Setup(s => s.GetPastTrainingsAsync()).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            await Task.Run(() => _viewModel.LoadTrainingsCommand.Execute(null));

            // Assert
            _toastServiceMock.Verify(t => t.DisplayToastAsync(exceptionMessage), Times.Once);
        }

        [Test]
        public void OnAppearing_WhenCalled_SetsIsBusyToTrue()
        {
            // Act
            _viewModel.OnAppearing();

            // Assert
            Assert.IsTrue(_viewModel.IsBusy);
        }

        [Test]
        public async Task TrainingTapped_ExecutesOnTrainingSelected()
        {
            // Arrange
            var training = new TrainingDetail { Code = "Training-1", Name = "Training 1" };
            bool messageReceived = false;

            // Register for the message before calling the method.
            WeakReferenceMessenger.Default.Register<TrainingSelectedMessage>(this, (r, m) =>
            {
                messageReceived = m.Value == training;
            });

            // Act
            await Task.Run(() => _viewModel.TrainingTapped.Execute(training));

            // Assert
            _navigationServiceMock.Verify(s => s.NavigateRelativeAsync("TrainingDetailPage"), Times.Once);
            Assert.IsTrue(messageReceived, "The expected message was not received.");
        }
    }
}