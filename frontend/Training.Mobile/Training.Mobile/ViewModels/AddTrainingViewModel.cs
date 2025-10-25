using System.Collections.ObjectModel;
using Training.Mobile.Models;
using Training.Mobile.Services;
using Training.Mobile.Services.Backend;

namespace Training.Mobile.ViewModels
{
    public class AddTrainingViewModel : BaseViewModel
    {
        private readonly ITrainingService _trainingService;
        private readonly IToastService _toastService;
        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required DateTime StartDate { get; set; }
        public required TimeSpan StartTime { get; set; }

        public required DateTime EndDate { get; set; }
        public required TimeSpan EndTime { get; set; }


        public int MaximumCapacity { get; set; }


        public Room? SelectedRoom { get; set; }

        public Command? AddTrainingCommand { get; }
        public ObservableCollection<Room> Rooms { get; }

        public Command LoadRoomsCommand { get; }


        public AddTrainingViewModel(ITrainingService trainingService, IUserService userService, IToastService toastService, INavigationService navigationService)
        {
            _trainingService = trainingService;
            _userService = userService;
            _toastService = toastService;
            _navigationService = navigationService;
            Rooms = new ObservableCollection<Room>();

            MaximumCapacity = 2;
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;

            LoadRoomsCommand = new Command(async () => await ExecuteLoadRoomsCommand());
            AddTrainingCommand = new Command(async () => await AddTrainingAsync());
        }
        public void OnAppearing()
        {
            IsBusy = true;
            LoadRoomsCommand.Execute(null);
        }
        public async Task ExecuteLoadRoomsCommand()
        {
            IsBusy = true;
            try
            {
                Rooms.Clear();
                IReadOnlyList<Room> allRooms = await _trainingService.GetAllRoomsAsync();
                foreach (var room in allRooms)
                {
                    Rooms.Add(room);
                }
            }
            catch (Exception ex)
            {
                await _toastService.DisplayToastAsync(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
      
        public async Task AddTrainingAsync()
        {
            IsBusy = true;

            try
            {
                var newTraining = new TrainingCreateModel
                {
                    Name = this.Name,
                    Description = this.Description,
                    MaximumCapacity = this.MaximumCapacity,
                    RoomCode = SelectedRoom?.RoomCode ?? string.Empty,
                    CoachId = _userService.Id ?? string.Empty,
                    StartTime = this.StartDate.Add(this.StartTime),
                    EndTime = this.EndDate.Add(this.EndTime)
                };

                await _trainingService.CreateTrainingAsync(newTraining);
                await _toastService.DisplayToastAsync("Training sessie succesvol aangemaakt!");
                await _navigationService.NavigateAsync("TrainingPage");
            }
            catch (Exception ex)
            {
                await _toastService.DisplayToastAsync(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
