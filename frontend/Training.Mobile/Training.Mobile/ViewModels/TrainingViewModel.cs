using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Training.Mobile.Messages;
using Training.Mobile.Models;
using Training.Mobile.Services;
using Training.Mobile.Services.Backend;

namespace Training.Mobile.ViewModels
{
    public class TrainingViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        private readonly ITrainingService _trainingService;
        private readonly IToastService _toastService;

        public ObservableCollection<TrainingDetail> Trainings { get; }
        public Command LoadTrainingsCommand { get; }
        public Command RegisterCommand { get; }
        public Command NavigateToAddTrainingCommand { get; }
        public Command NavigateToPastTrainingsCommand { get; }
        public Command NavigateToAddApprenticeCommand { get; }
        public string? UserId => _userService.Id;

        public bool IsCoach => _userService.IsCoach;


        public TrainingViewModel(IUserService userService, INavigationService navigationService, ITrainingService trainingService, IToastService toastService)
        {
            _userService = userService;
            _navigationService = navigationService;
            _trainingService = trainingService;
            _toastService = toastService;

            Trainings = new ObservableCollection<TrainingDetail>();

            _userService.PropertyChanged += OnUserServicePropertyChanged;

            RegisterCommand = new Command<TrainingDetail>(async (training) => await ExecuteRegisterCommand(training));
            LoadTrainingsCommand = new Command(async () => await ExecuteLoadTrainingsCommand());
            NavigateToAddTrainingCommand = new Command(async () => await NavigateToAddTraining());
            NavigateToPastTrainingsCommand = new Command(async () => await NavigateToPastTrainings());
            NavigateToAddApprenticeCommand = new Command<TrainingDetail>(async (training) => await NavigateToAddApprentice(training));
        }
        public void OnAppearing()
        {
            IsBusy = true;
        }

        public async Task ExecuteLoadTrainingsCommand()
        {
            IsBusy = true;

            try
            {
                IReadOnlyList<TrainingDetail> allTrainings = await _trainingService.GetFutureTrainingsAsync();
                Trainings.Clear();
                foreach (var training in allTrainings)
                {
                    training.IsUserRegistered = CheckIfUserRegistered(training);
                    training.IsFull = training.Participations.Count >= training.MaximumCapacity;
                    Trainings.Add(training);
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
        public bool CheckIfUserRegistered(TrainingDetail training)
        {
            return training.Participations.Any(p => p.Apprentice?.Id == UserId);
        }
        public async Task ExecuteRegisterCommand(TrainingDetail training)
        {
            if (UserId == null)
            {
                return;
            }

            IsBusy = true;
            try
            {
                await _trainingService.RegisterApprenticeAsync(training.Code, UserId);
                await _toastService.DisplayToastAsync("Succesvol ingeschreven!");

                await ExecuteLoadTrainingsCommand();    // Instantly loads the new changes instead of having to refresh

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
        private void OnUserServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IUserService.IsCoach))
            {
                OnPropertyChanged(nameof(IsCoach));
            }
        }
        public async Task NavigateToAddApprentice(TrainingDetail training)
        {
            await _navigationService.NavigateRelativeAsync("AddApprenticePage");
            WeakReferenceMessenger.Default.Send(new TrainingSelectedMessage(training));
        }

        public async Task NavigateToAddTraining()
        {
            await _navigationService.NavigateRelativeAsync("AddTrainingPage");
        }
        public async Task NavigateToPastTrainings()
        {
            await _navigationService.NavigateRelativeAsync("PastTrainingsPage");
        }
    }
}
