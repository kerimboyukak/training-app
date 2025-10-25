using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Mobile.Messages;
using Training.Mobile.Models;
using Training.Mobile.Services;
using Training.Mobile.Services.Backend;

namespace Training.Mobile.ViewModels
{
    public class AddApprenticeViewModel : BaseViewModel
    {
        private TrainingDetail? _training;

        private readonly ITrainingService _trainingService;
        private readonly IToastService _toastService;
        private readonly INavigationService _navigationService;

        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Company { get; set; }
        public Command? AddApprenticeCommand { get; }



        public TrainingDetail? Training
        {
            get => _training;
            set => SetProperty(ref _training, value);   // SetProperty is a method from BaseViewModel for setting the value and raising the PropertyChanged event
        }
        public AddApprenticeViewModel(ITrainingService trainingService, IToastService toastService, INavigationService navigationService)
        {
            _trainingService = trainingService;
            _toastService = toastService;
            _navigationService = navigationService;

            WeakReferenceMessenger.Default.Register<TrainingSelectedMessage>(this, (trainingViewModel, selectedTrainingMessage) =>
            {
                Training = selectedTrainingMessage.Value;
            });

            AddApprenticeCommand = new Command(async () => await AddApprenticeAsync());


        }
        private async Task AddApprenticeAsync()
        {
            IsBusy = true;

            try
            {
                var newApprentice = new ApprenticeCreateModel
                {
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    Company = this.Company,            
                };

                await _trainingService.RegisterExternalApprenticeAsync(Training.Code,newApprentice);
                await _toastService.DisplayToastAsync("Externe leerling succesvol toegevoegd!");
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
