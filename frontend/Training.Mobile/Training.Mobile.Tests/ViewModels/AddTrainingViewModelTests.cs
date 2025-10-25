using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Training.Mobile.Models;
using Training.Mobile.Services;
using Training.Mobile.Services.Backend;
using Training.Mobile.ViewModels;

namespace Training.Mobile.Tests.ViewModels
{
    [TestFixture]
    public class AddTrainingViewModelTests
    {
        private Mock<ITrainingService> _trainingServiceMock;
        private Mock<IToastService> _toastServiceMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<INavigationService> _navigationServiceMock;
        private AddTrainingViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _trainingServiceMock = new Mock<ITrainingService>();
            _toastServiceMock = new Mock<IToastService>();
            _userServiceMock = new Mock<IUserService>();
            _navigationServiceMock = new Mock<INavigationService>();

            _viewModel = new AddTrainingViewModel(_trainingServiceMock.Object, _userServiceMock.Object, _toastServiceMock.Object, _navigationServiceMock.Object)
            {
                Name = string.Empty,
                Description = string.Empty,
                StartDate = DateTime.Today,
                StartTime = new TimeSpan(0, 0, 0),
                EndDate = DateTime.Today,
                EndTime = new TimeSpan(0, 0, 0)
            };
        }

        [Test]
        public void Constructor_InitializesCommandsCorrectly()
        {
            // Assert
            Assert.IsNotNull(_viewModel.LoadRoomsCommand);
            Assert.IsNotNull(_viewModel.AddTrainingCommand);
        }

        [Test]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Assert
            Assert.That(_viewModel.MaximumCapacity, Is.EqualTo(2));
            Assert.That(_viewModel.StartDate, Is.EqualTo(DateTime.Today));
            Assert.That(_viewModel.EndDate, Is.EqualTo(DateTime.Today));
            Assert.IsNotNull(_viewModel.Rooms);
        }

        [Test]
        public async Task LoadRoomsCommand_ExecutesExecuteLoadRoomsCommand()
        {
            // Arrange
            var rooms = new List<Room>
            {
                new Room { RoomCode = "Room-1", Name = "Room 1" },
                new Room { RoomCode = "Room-2", Name = "Room 2" }
            };
            _trainingServiceMock.Setup(s => s.GetAllRoomsAsync()).ReturnsAsync(rooms);

            // Act
            await Task.Run(() => _viewModel.LoadRoomsCommand.Execute(null));

            // Assert
            Assert.That(_viewModel.Rooms.Count, Is.EqualTo(2));
            Assert.That(_viewModel.Rooms[0].RoomCode, Is.EqualTo("Room-1"));
            Assert.That(_viewModel.Rooms[1].RoomCode, Is.EqualTo("Room-2"));
        }

        [Test]
        public async Task LoadRoomsCommand_WhenServiceThrowsException_DisplaysToast()
        {
            // Arrange
            var exceptionMessage = "Error loading rooms";
            _trainingServiceMock.Setup(s => s.GetAllRoomsAsync()).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            await Task.Run(() => _viewModel.LoadRoomsCommand.Execute(null));

            // Assert
            _toastServiceMock.Verify(t => t.DisplayToastAsync(exceptionMessage), Times.Once);
        }

        [Test]
        public async Task AddTrainingCommand_ExecutesAddTrainingAsync()
        {
            // Arrange
            _viewModel.Name = "Training 1";
            _viewModel.Description = "Description 1";
            _viewModel.StartDate = DateTime.Today;
            _viewModel.StartTime = new TimeSpan(9, 0, 0);
            _viewModel.EndDate = DateTime.Today;
            _viewModel.EndTime = new TimeSpan(17, 0, 0);
            _viewModel.MaximumCapacity = 10;
            _viewModel.SelectedRoom = new Room { RoomCode = "Room-1" };
            _userServiceMock.Setup(u => u.Id).Returns("Coach-1");

            // Act
            if (_viewModel.AddTrainingCommand != null)
            {
                await Task.Run(() => _viewModel.AddTrainingCommand.Execute(null));
            }

            // Assert
            _trainingServiceMock.Verify(s => s.CreateTrainingAsync(It.Is<TrainingCreateModel>(t =>
                t.Name == "Training 1" &&
                t.Description == "Description 1" &&
                t.MaximumCapacity == 10 &&
                t.RoomCode == "Room-1" &&
                t.CoachId == "Coach-1" &&
                t.StartTime == DateTime.Today.Add(new TimeSpan(9, 0, 0)) &&
                t.EndTime == DateTime.Today.Add(new TimeSpan(17, 0, 0))
            )), Times.Once);
            _toastServiceMock.Verify(t => t.DisplayToastAsync("Training sessie succesvol aangemaakt!"), Times.Once);
            _navigationServiceMock.Verify(n => n.NavigateAsync("TrainingPage"), Times.Once);
        }

        [Test]
        public async Task AddTrainingCommand_WhenServiceThrowsException_DisplaysToast()
        {
            // Arrange
            var exceptionMessage = "Error creating training";
            _viewModel.Name = "Training 1";
            _viewModel.Description = "Description 1";
            _viewModel.StartDate = DateTime.Today;
            _viewModel.StartTime = new TimeSpan(9, 0, 0);
            _viewModel.EndDate = DateTime.Today;
            _viewModel.EndTime = new TimeSpan(17, 0, 0);
            _viewModel.MaximumCapacity = 10;
            _viewModel.SelectedRoom = new Room { RoomCode = "Room-1" };
            _userServiceMock.Setup(u => u.Id).Returns("Coach-1");
            _trainingServiceMock.Setup(s => s.CreateTrainingAsync(It.IsAny<TrainingCreateModel>())).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            if (_viewModel.AddTrainingCommand != null)
            {
                await Task.Run(() => _viewModel.AddTrainingCommand.Execute(null));
            }

            // Assert
            _toastServiceMock.Verify(t => t.DisplayToastAsync(exceptionMessage), Times.Once);
        }
        [Test]
        public void OnAppearing_WhenCalled_SetsIsBusyToTrueAndExecutesLoadRoomsCommand()
        {
            // Arrange
            var rooms = new List<Room>
            {
                new Room { RoomCode = "Room-1", Name = "Room 1" },
                new Room { RoomCode = "Room-2", Name = "Room 2" }
            };
            _trainingServiceMock.Setup(s => s.GetAllRoomsAsync()).ReturnsAsync(rooms);

            // Act
            _viewModel.OnAppearing();

            // Assert 
            _trainingServiceMock.Verify(s => s.GetAllRoomsAsync(), Times.Once);
            Assert.That(_viewModel.Rooms.Count, Is.EqualTo(2));
            Assert.That(_viewModel.Rooms[0].RoomCode, Is.EqualTo("Room-1"));
            Assert.That(_viewModel.Rooms[1].RoomCode, Is.EqualTo("Room-2"));

            // Assert that IsBusy is set to false after the operation is completed
            Assert.IsFalse(_viewModel.IsBusy);
        }
    }
}