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
    public class TrainingDetailViewModelTests
    {
        private Mock<ITrainingService> _trainingServiceMock;
        private Mock<IToastService> _toastServiceMock;
        private TrainingDetailViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _trainingServiceMock = new Mock<ITrainingService>();
            _toastServiceMock = new Mock<IToastService>();

            _viewModel = new TrainingDetailViewModel(_trainingServiceMock.Object, _toastServiceMock.Object);
        }

        [Test]
        public void Constructor_InitializesCompleteParticipationCommand()
        {
            // Assert
            Assert.IsNotNull(_viewModel.CompleteParticipationCommand);
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
        public async Task CompleteParticipationCommand_ExecutesExecuteCompleteParticipationCommand()
        {
            // Arrange
            var participation = new Participation
            {
                Apprentice = new Apprentice { Id = "Apprentice-1", FirstName = "John" }
            };
            _viewModel.Training = new TrainingDetail { Code = "Training-1" };

            // Act
            await Task.Run(() => _viewModel.CompleteParticipationCommand.Execute(participation));

            // Assert
            _trainingServiceMock.Verify(s => s.CompleteParticipation("Training-1", "Apprentice-1"), Times.Once);
            _toastServiceMock.Verify(t => t.DisplayToastAsync("Training afgerond voor John"), Times.Once);
            _trainingServiceMock.Verify(s => s.GetTrainingDetailsAsync("Training-1"), Times.Once);
        }

        [Test]
        public async Task CompleteParticipationCommand_WhenServiceThrowsException_DisplaysToast()
        {
            // Arrange
            var exceptionMessage = "Error completing participation";
            var participation = new Participation
            {
                Apprentice = new Apprentice { Id = "Apprentice-1", FirstName = "John" }
            };
            _viewModel.Training = new TrainingDetail { Code = "Training-1" };
            _trainingServiceMock.Setup(s => s.CompleteParticipation(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            await Task.Run(() => _viewModel.CompleteParticipationCommand.Execute(participation));

            // Assert
            _toastServiceMock.Verify(t => t.DisplayToastAsync(exceptionMessage), Times.Once);
        }

        [Test]
        public async Task CompleteParticipationCommand_WhenParticipationIsNull_DoesNothing()
        {
            // Act
            await Task.Run(() => _viewModel.CompleteParticipationCommand.Execute(null));

            // Assert
            _trainingServiceMock.Verify(s => s.CompleteParticipation(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _toastServiceMock.Verify(t => t.DisplayToastAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task CompleteParticipationCommand_WhenTrainingIsNull_DoesNothing()
        {
            // Arrange
            var participation = new Participation
            {
                Apprentice = new Apprentice { Id = "Apprentice-1", FirstName = "John" }
            };

            // Act
            await Task.Run(() => _viewModel.CompleteParticipationCommand.Execute(participation));

            // Assert
            _trainingServiceMock.Verify(s => s.CompleteParticipation(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _toastServiceMock.Verify(t => t.DisplayToastAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task CompleteParticipationCommand_WhenApprenticeIsNull_DoesNothing()
        {
            // Arrange
            var participation = new Participation();
            _viewModel.Training = new TrainingDetail { Code = "Training-1" };

            // Act
            await Task.Run(() => _viewModel.CompleteParticipationCommand.Execute(participation));

            // Assert
            _trainingServiceMock.Verify(s => s.CompleteParticipation(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _toastServiceMock.Verify(t => t.DisplayToastAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ReloadTrainingDetailsAsync_WhenCalled_UpdatesTraining()
        {
            // Arrange
            var updatedTraining = new TrainingDetail { Code = "Training-1", Name = "Updated Training" };
            _viewModel.Training = new TrainingDetail { Code = "Training-1" };
            _trainingServiceMock.Setup(s => s.GetTrainingDetailsAsync("Training-1")).ReturnsAsync(updatedTraining);

            // Act
            await Task.Run(() => _viewModel.CompleteParticipationCommand.Execute(new Participation { Apprentice = new Apprentice { Id = "Apprentice-1" } }));

            // Assert
            Assert.That(_viewModel.Training, Is.EqualTo(updatedTraining));
        }

        [Test]
        public async Task ReloadTrainingDetailsAsync_WhenServiceThrowsException_DisplaysToast()
        {
            // Arrange
            var exceptionMessage = "Error reloading training details";
            _viewModel.Training = new TrainingDetail { Code = "Training-1" };
            _trainingServiceMock.Setup(s => s.GetTrainingDetailsAsync(It.IsAny<string>())).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            await Task.Run(() => _viewModel.CompleteParticipationCommand.Execute(new Participation { Apprentice = new Apprentice { Id = "Apprentice-1" } }));

            // Assert
            _toastServiceMock.Verify(t => t.DisplayToastAsync(exceptionMessage), Times.Once);
        }

        [Test]
        public async Task ReloadTrainingDetailsAsync_WhenTrainingIsNull_DoesNothing()
        {
            // Act
            await _viewModel.ReloadTrainingDetailsAsync();

            // Assert
            _trainingServiceMock.Verify(s => s.GetTrainingDetailsAsync(It.IsAny<string>()), Times.Never);
            _toastServiceMock.Verify(t => t.DisplayToastAsync(It.IsAny<string>()), Times.Never);
        }
    }
}