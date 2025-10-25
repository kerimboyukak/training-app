using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;
using Training.Mobile.Messages;
using Training.Mobile.Models;
using Training.Mobile.Services;
using Training.Mobile.Services.Backend;

namespace Training.Mobile.ViewModels
{
    public class TrainingDetailViewModel : BaseViewModel
    {
        private readonly ITrainingService _trainingService;
        private readonly IToastService _toastService;
        private TrainingDetail? _training;

        public TrainingDetail? Training
        {
            get => _training;
            set => SetProperty(ref _training, value);   // SetProperty is a method from BaseViewModel for setting the value and raising the PropertyChanged event
        }

        public ICommand CompleteParticipationCommand { get; }

        public TrainingDetailViewModel(ITrainingService trainingService, IToastService toastService)
        {
            _trainingService = trainingService;
            _toastService = toastService;

            CompleteParticipationCommand = new Command<Participation>(async (participation) => await ExecuteCompleteParticipationCommand(participation));

            WeakReferenceMessenger.Default.Register<TrainingSelectedMessage>(this, (pastTrainingsViewModel, selectedTrainingMessage) =>
            {
                Training = selectedTrainingMessage.Value;
            });
        }

        private async Task ExecuteCompleteParticipationCommand(Participation participation)
        {
            if (participation == null || Training == null || participation.Apprentice == null)
            {
                return;
            }
            try
            {
                await _trainingService.CompleteParticipation(Training.Code, participation.Apprentice.Id);
                await _toastService.DisplayToastAsync("Training afgerond voor " + participation.Apprentice.FirstName);
                await ReloadTrainingDetailsAsync();
            }
            catch (Exception ex)
            {
                await _toastService.DisplayToastAsync(ex.Message);
            }
        }
        public async Task ReloadTrainingDetailsAsync()
        {
            if (Training == null)
            {
                return;
            }
            try
            {
                var updatedTraining = await _trainingService.GetTrainingDetailsAsync(Training.Code);
                Training = updatedTraining;
            }
            catch (Exception ex)
            {
                await _toastService.DisplayToastAsync(ex.Message);
            }
        }
    }
}
