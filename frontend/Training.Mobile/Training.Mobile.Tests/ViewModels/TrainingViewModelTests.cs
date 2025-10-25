using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Training.Mobile.Models;
using Training.Mobile.Services;
using Training.Mobile.Services.Backend;
using Training.Mobile.ViewModels;
using Training.Mobile.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;

namespace Training.Mobile.Tests.ViewModels
{
    [TestFixture]
    public class TrainingViewModelTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<INavigationService> _navigationServiceMock;
        private Mock<ITrainingService> _trainingServiceMock;
        private Mock<IToastService> _toastServiceMock;
        private TrainingViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);


            _userServiceMock = new Mock<IUserService>();
            _navigationServiceMock = new Mock<INavigationService>();
            _trainingServiceMock = new Mock<ITrainingService>();
            _toastServiceMock = new Mock<IToastService>();

            _viewModel = new TrainingViewModel(_userServiceMock.Object, _navigationServiceMock.Object, _trainingServiceMock.Object, _toastServiceMock.Object);
        }

        [Test]
        public void Constructor_InitializesDependencies()
        {
            // Assert
            Assert.IsNotNull(_viewModel);
            Assert.IsNotNull(_viewModel.Trainings);
            Assert.IsNotNull(_viewModel.LoadTrainingsCommand);
            Assert.IsNotNull(_viewModel.RegisterCommand);
            Assert.IsNotNull(_viewModel.NavigateToAddTrainingCommand);
            Assert.IsNotNull(_viewModel.NavigateToPastTrainingsCommand);
            Assert.IsNotNull(_viewModel.NavigateToAddApprenticeCommand);
        }

        [Test]
        public void Constructor_SetsUserServicePropertyChangedEventHandler()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();

            // Act
            var viewModel = new TrainingViewModel(userServiceMock.Object, _navigationServiceMock.Object, _trainingServiceMock.Object, _toastServiceMock.Object);

            // Assert
            userServiceMock.VerifyAdd(u => u.PropertyChanged += It.IsAny<PropertyChangedEventHandler>(), Times.Once);
        }

        [Test]
        public void OnAppearing_WhenCalled_IsBusyIsTrue()
        {
            // Act
            _viewModel.OnAppearing();

            // Assert
            Assert.IsTrue(_viewModel.IsBusy);
        }

        [Test]
        public async Task ExecuteLoadTrainingsCommand_WhenCalled_LoadsTrainings()
        {
            // Arrange
            var trainings = new List<TrainingDetail>
            {
                new TrainingDetail { Code = "Training-1", MaximumCapacity = 2, Participations = new List<Participation>() },
                new TrainingDetail { Code = "Training-2", MaximumCapacity = 1, Participations = new List<Participation> { new Participation() } }
            };
            _trainingServiceMock.Setup(s => s.GetFutureTrainingsAsync()).ReturnsAsync(trainings);

            // Act
            await _viewModel.ExecuteLoadTrainingsCommand();

            // Assert
            Assert.That(_viewModel.Trainings.Count, Is.EqualTo(2));
            Assert.IsFalse(_viewModel.Trainings[0].IsFull);
            Assert.IsTrue(_viewModel.Trainings[1].IsFull);
        }

        [Test]
        public async Task ExecuteLoadTrainingsCommand_WhenServiceThrowsException_DisplaysToast()
        {
            // Arrange
            var exceptionMessage = "Error loading trainings";
            _trainingServiceMock.Setup(s => s.GetFutureTrainingsAsync()).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            await _viewModel.ExecuteLoadTrainingsCommand();

            // Assert
            _toastServiceMock.Verify(t => t.DisplayToastAsync(exceptionMessage), Times.Once);
        }

        [Test]
        public async Task ExecuteRegisterCommand_WhenCalled_RegistersUser()
        {
            // Arrange
            var training = new TrainingDetail { Code = "Training-1" };
            _userServiceMock.Setup(u => u.Id).Returns("User1");

            // Act
            await _viewModel.ExecuteRegisterCommand(training);

            // Assert
            _trainingServiceMock.Verify(s => s.RegisterApprenticeAsync("Training-1", "User1"), Times.Once);
            _toastServiceMock.Verify(t => t.DisplayToastAsync("Succesvol ingeschreven!"), Times.Once);
        }

        [Test]
        public async Task ExecuteRegisterCommand_WhenUserIdIsNull_DoesNothing()
        {
            // Arrange
            var training = new TrainingDetail { Code = "T1" };
            _userServiceMock.Setup(u => u.Id).Returns((string?)null);

            // Act
            await _viewModel.ExecuteRegisterCommand(training);

            // Assert
            _trainingServiceMock.Verify(s => s.RegisterApprenticeAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ExecuteRegisterCommand_WhenExceptionThrown_DisplaysToastMessage()
        {
            // Arrange
            var training = new TrainingDetail { Code = "Training-1" };
            var exceptionMessage = "Error registering";
            _userServiceMock.Setup(u => u.Id).Returns("User1");
            _trainingServiceMock.Setup(s => s.RegisterApprenticeAsync(training.Code, "User1")).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            await _viewModel.ExecuteRegisterCommand(training);

            // Assert
            _toastServiceMock.Verify(t => t.DisplayToastAsync(exceptionMessage), Times.Once);
        }

        [Test]
        public async Task ExecuteRegisterCommand_SetsIsBusyCorrectly()
        {
            // Arrange
            var training = new TrainingDetail { Code = "Training-1" };
            _userServiceMock.Setup(u => u.Id).Returns("User1");

            // Act
            var task = _viewModel.ExecuteRegisterCommand(training);

            // Assert
            await task;

            Assert.IsFalse(_viewModel.IsBusy);
        }

        [Test]
        public async Task ExecuteRegisterCommand_RefreshesTrainings()
        {
            // Arrange
            var training = new TrainingDetail { Code = "Training-1" };
            _userServiceMock.Setup(u => u.Id).Returns("User1");

            // Act
            await _viewModel.ExecuteRegisterCommand(training);

            // Assert
            _trainingServiceMock.Verify(s => s.GetFutureTrainingsAsync(), Times.Once);
        }

        [Test]
        public void CheckIfUserRegistered_WhenUserIsRegistered_ReturnsTrue()
        {
            // Arrange
            var training = new TrainingDetail
            {
                Participations = new List<Participation>
                {
                    new Participation { Apprentice = new Apprentice { Id = "User1" } }
                }
            };
            _userServiceMock.Setup(u => u.Id).Returns("User1");

            // Act
            var result = _viewModel.CheckIfUserRegistered(training);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CheckIfUserRegistered_WhenUserIsNotRegistered_ReturnsFalse()
        {
            // Arrange
            var training = new TrainingDetail
            {
                Participations = new List<Participation>()
            };
            _userServiceMock.Setup(u => u.Id).Returns("User1");

            // Act
            var result = _viewModel.CheckIfUserRegistered(training);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void OnUserServicePropertyChanged_WhenIsCoachChanges_RaisesPropertyChanged()
        {
            // Arrange
            bool propertyChangedRaised = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TrainingViewModel.IsCoach))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            _userServiceMock.Raise(u => u.PropertyChanged += null, new PropertyChangedEventArgs(nameof(IUserService.IsCoach)));

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [Test]
        public async Task NavigateToAddTraining_WhenCalled_NavigatesToAddTrainingPage()
        {
            // Act
            await _viewModel.NavigateToAddTraining();

            // Assert
            _navigationServiceMock.Verify(s => s.NavigateRelativeAsync("AddTrainingPage"), Times.Once);
        }

        [Test]
        public async Task NavigateToPastTrainings_WhenCalled_NavigatesToPastTrainingsPage()
        {
            // Act
            await _viewModel.NavigateToPastTrainings();

            // Assert
            _navigationServiceMock.Verify(s => s.NavigateRelativeAsync("PastTrainingsPage"), Times.Once);
        }

        [Test]
        public void LoadTrainingsCommand_Getter_ReturnsCommand()
        {
            // Assert
            Assert.IsNotNull(_viewModel.LoadTrainingsCommand);
            Assert.IsInstanceOf<Command>(_viewModel.LoadTrainingsCommand);
        }

        [Test]
        public void RegisterCommand_Getter_ReturnsCommand()
        {
            // Assert
            Assert.IsNotNull(_viewModel.RegisterCommand);
            Assert.IsInstanceOf<Command>(_viewModel.RegisterCommand);
        }

        [Test]
        public void NavigateToAddTrainingCommand_Getter_ReturnsCommand()
        {
            // Assert
            Assert.IsNotNull(_viewModel.NavigateToAddTrainingCommand);
            Assert.IsInstanceOf<Command>(_viewModel.NavigateToAddTrainingCommand);
        }

        [Test]
        public void NavigateToPastTrainingsCommand_Getter_ReturnsCommand()
        {
            // Assert
            Assert.IsNotNull(_viewModel.NavigateToPastTrainingsCommand);
            Assert.IsInstanceOf<Command>(_viewModel.NavigateToPastTrainingsCommand);
        }

        [Test]
        public void NavigateToAddApprenticeCommand_Getter_ReturnsCommand()
        {
            // Assert
            Assert.IsNotNull(_viewModel.NavigateToAddApprenticeCommand);
            Assert.IsInstanceOf<Command>(_viewModel.NavigateToAddApprenticeCommand);
        }

        [Test]
        public void IsCoach_Getter_ReturnsCorrectValue()
        {
            // Arrange
            _userServiceMock.Setup(u => u.IsCoach).Returns(true);

            // Act
            var isCoach = _viewModel.IsCoach;

            // Assert
            Assert.IsTrue(isCoach);
        }
 
        [Test]
        public async Task NavigateToAddApprentice_WhenTrainingIsValid_NavigatesAndSendsMessage()
        {
            // Arrange
            var training = new TrainingDetail { Code = "Training-1" };
            bool messageReceived = false;

            // Register for the message before calling the method.
            WeakReferenceMessenger.Default.Register<TrainingSelectedMessage>(this, (r, m) =>
            {
                messageReceived = m.Value == training;
            });

            // Act
            await _viewModel.NavigateToAddApprentice(training);

            // Assert
            _navigationServiceMock.Verify(s => s.NavigateRelativeAsync("AddApprenticePage"), Times.Once);
            Assert.IsTrue(messageReceived, "The expected message was not received.");
        }

        [Test]
        public void Register_WeakReferenceMessenger_SubscriptionTest()
        {
            // Arrange
            var training = new TrainingDetail { Code = "Training-1" };
            bool messageReceived = false;

            // Act
            WeakReferenceMessenger.Default.Register<TrainingSelectedMessage>(this, (r, m) =>
            {
                messageReceived = m.Value == training;
            });
            WeakReferenceMessenger.Default.Send(new TrainingSelectedMessage(training));

            // Assert
            Assert.IsTrue(messageReceived, "Message was not received after sending.");
        }
        [Test]
        public async Task NavigateToAddApprentice_WhenTrainingIsNotNull_NavigatesAndSendsMessage()
        {
            // Arrange
            var training = new TrainingDetail { Code = "Training-1" };
            bool messageSent = false;

            // Register for the message
            WeakReferenceMessenger.Default.Register<TrainingSelectedMessage>(this, (r, m) =>
            {
                messageSent = true;
            });

            // Act
            await _viewModel.NavigateToAddApprentice(training);

            // Assert
            _navigationServiceMock.Verify(s => s.NavigateRelativeAsync("AddApprenticePage"), Times.Once);
            Assert.IsTrue(messageSent, "The message should be sent.");
        }

        [Test]
        public async Task RegisterCommand_ExecutesExecuteRegisterCommand()
        {
            // Arrange
            var training = new TrainingDetail { Code = "Training-1" };
            _userServiceMock.Setup(u => u.Id).Returns("User1");

            // Act
            await Task.Run(() => _viewModel.RegisterCommand.Execute(training));

            // Assert
            _trainingServiceMock.Verify(s => s.RegisterApprenticeAsync("Training-1", "User1"), Times.Once);
        }

        [Test]
        public async Task LoadTrainingsCommand_ExecutesExecuteLoadTrainingsCommand()
        {
            // Arrange
            var trainings = new List<TrainingDetail>
            {
                new TrainingDetail { Code = "Training-1", MaximumCapacity = 2, Participations = new List<Participation>() },
                new TrainingDetail { Code = "Training-2", MaximumCapacity = 1, Participations = new List<Participation> { new Participation() } }
            };
            _trainingServiceMock.Setup(s => s.GetFutureTrainingsAsync()).ReturnsAsync(trainings);

            // Act
            await Task.Run(() => _viewModel.LoadTrainingsCommand.Execute(null));

            // Assert
            Assert.That(_viewModel.Trainings.Count, Is.EqualTo(2));
            Assert.IsFalse(_viewModel.Trainings[0].IsFull);
            Assert.IsTrue(_viewModel.Trainings[1].IsFull);
        }

        [Test]
        public async Task NavigateToAddTrainingCommand_ExecutesNavigateToAddTraining()
        {
            // Act
            await Task.Run(() => _viewModel.NavigateToAddTrainingCommand.Execute(null));

            // Assert
            _navigationServiceMock.Verify(s => s.NavigateRelativeAsync("AddTrainingPage"), Times.Once);
        }

        [Test]
        public async Task NavigateToPastTrainingsCommand_ExecutesNavigateToPastTrainings()
        {
            // Act
            await Task.Run(() => _viewModel.NavigateToPastTrainingsCommand.Execute(null));

            // Assert
            _navigationServiceMock.Verify(s => s.NavigateRelativeAsync("PastTrainingsPage"), Times.Once);
        }

        [Test]
        public async Task NavigateToAddApprenticeCommand_ExecutesNavigateToAddApprentice()
        {
            // Arrange
            var training = new TrainingDetail { Code = "Training-1" };
            bool messageReceived = false;

            // Register for the message before calling the method.
            WeakReferenceMessenger.Default.Register<TrainingSelectedMessage>(this, (r, m) =>
            {
                messageReceived = m.Value == training;
            });

            // Act
            await Task.Run(() => _viewModel.NavigateToAddApprenticeCommand.Execute(training));

            // Assert
            _navigationServiceMock.Verify(s => s.NavigateRelativeAsync("AddApprenticePage"), Times.Once);
            Assert.IsTrue(messageReceived, "The expected message was not received.");
        }
    }
}