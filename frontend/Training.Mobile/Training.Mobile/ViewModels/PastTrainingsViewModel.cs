using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Mobile.Messages;
using Training.Mobile.Models;
using Training.Mobile.Services;
using Training.Mobile.Services.Backend;

namespace Training.Mobile.ViewModels
{
    public class PastTrainingsViewModel : BaseViewModel
    {
        private readonly ITrainingService _trainingService;
        private readonly IToastService _toastService;
        private readonly INavigationService _navigationService;

        public Command LoadTrainingsCommand { get; }
        public ObservableCollection<TrainingDetail> Trainings { get; }
        public Command<TrainingDetail> TrainingTapped { get; }


        public PastTrainingsViewModel(ITrainingService trainingService, IToastService toastService, INavigationService navigationService)
        {
            _trainingService = trainingService;
            _toastService = toastService;
            _navigationService = navigationService;

            Trainings = new ObservableCollection<TrainingDetail>();
            LoadTrainingsCommand = new Command(async () => await ExecuteLoadTrainingsCommand());

            TrainingTapped = new Command<TrainingDetail>(OnTrainingSelected);

        }
        public void OnAppearing()
        {
            IsBusy = true;
        }

        private async Task ExecuteLoadTrainingsCommand()
        {
            IsBusy = true;

            try
            {
                Trainings.Clear();

                IReadOnlyList<TrainingDetail> allTrainings = await _trainingService.GetPastTrainingsAsync();
                foreach (var training in allTrainings)
                {
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
        async void OnTrainingSelected(TrainingDetail training)
        {
            await _navigationService.NavigateRelativeAsync("TrainingDetailPage");
            WeakReferenceMessenger.Default.Send(new TrainingSelectedMessage(training));
        }
    }
}
