using Moq;
using NUnit.Framework;
using System;
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
    public class AddApprenticeViewModelTests
    {
        private Mock<ITrainingService> _trainingServiceMock;
        private Mock<IToastService> _toastServiceMock;
        private Mock<INavigationService> _navigationServiceMock;
        private AddApprenticeViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _trainingServiceMock = new Mock<ITrainingService>();
            _toastServiceMock = new Mock<IToastService>();
            _navigationServiceMock = new Mock<INavigationService>();

            _viewModel = new AddApprenticeViewModel(_trainingServiceMock.Object, _toastServiceMock.Object, _navigationServiceMock.Object)
            {
                FirstName = string.Empty,
                LastName = string.Empty,
                Company = string.Empty
            };
        }

        [Test]
        public void Constructor_InitializesAddApprenticeCommand()
        {
            // Assert
            Assert.IsNotNull(_viewModel.AddApprenticeCommand);
        }

        [Test]
        public void Constructor_RegistersForTrainingSelectedMessage()
        {
            // Arrange
            var training = new TrainingDetail { Code = "Training-1" };

            // Act
            WeakReferenceMessenger.Default.Send(new TrainingSelectedMessage(training));

            // Assert
            Assert.That(_viewModel.Training, Is.EqualTo(training));
        }

        [Test]
        public async Task AddApprenticeCommand_ExecutesAddApprenticeAsync()
        {
            // Arrange
            _viewModel.FirstName = "John";
            _viewModel.LastName = "Doe";
            _viewModel.Company = "Company 1";
            _viewModel.Training = new TrainingDetail { Code = "Training-1" };

            // Act
            if (_viewModel.AddApprenticeCommand != null)
            {
                await Task.Run(() => _viewModel.AddApprenticeCommand.Execute(null));
            }

            // Assert
            _trainingServiceMock.Verify(s => s.RegisterExternalApprenticeAsync("Training-1", It.Is<ApprenticeCreateModel>(a =>
                a.FirstName == "John" &&
                a.LastName == "Doe" &&
                a.Company == "Company 1"
            )), Times.Once);
            _toastServiceMock.Verify(t => t.DisplayToastAsync("Externe leerling succesvol toegevoegd!"), Times.Once);
            _navigationServiceMock.Verify(n => n.NavigateAsync("TrainingPage"), Times.Once);
        }

        [Test]
        public async Task AddApprenticeCommand_WhenServiceThrowsException_DisplaysToast()
        {
            // Arrange
            var exceptionMessage = "Error adding apprentice";
            _viewModel.FirstName = "John";
            _viewModel.LastName = "Doe";
            _viewModel.Company = "Company 1";
            _viewModel.Training = new TrainingDetail { Code = "Training-1" };
            _trainingServiceMock.Setup(s => s.RegisterExternalApprenticeAsync(It.IsAny<string>(), It.IsAny<ApprenticeCreateModel>())).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            if (_viewModel.AddApprenticeCommand != null)
            {
                await Task.Run(() => _viewModel.AddApprenticeCommand.Execute(null));
            }

            // Assert
            _toastServiceMock.Verify(t => t.DisplayToastAsync(exceptionMessage), Times.Once);
        }
    }
}