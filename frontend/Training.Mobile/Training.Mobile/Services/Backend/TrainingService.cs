using Microsoft.Extensions.Logging;
using Training.Mobile.Exceptions;
using Training.Mobile.Models;
using Training.Mobile.Services.Identity;
using Training.Mobile.Settings;

namespace Training.Mobile.Services.Backend
{
    public class TrainingService : ITrainingService
    {
        private readonly IBackendService _backend;
        private readonly IAppSettings _appSettings;
        private readonly ITokenProvider _tokenProvider;
        private readonly INavigationService _navigationService;


        public TrainingService(IBackendService backend,
            IAppSettings appSettings,
            ITokenProvider tokenProvider,
            INavigationService navigationService)
        {
            _backend = backend;
            _appSettings = appSettings;
            _tokenProvider = tokenProvider;
            _navigationService = navigationService;
        }

        public async Task CreateTrainingAsync(TrainingCreateModel model)
        {
            try
            {
                await _backend.PostAsync($"{_appSettings.TrainingBackendBaseUrl}/api/trainings", model, _tokenProvider.AuthAccessToken);
            }
            catch (BackendAuthenticationException)
            {
                await _navigationService.NavigateAsync("/LoginPage");
            }
        }
        public async Task<IReadOnlyList<Room>> GetAllRoomsAsync()
        {
            try
            {
                return await _backend.GetAsync<List<Room>>($"{_appSettings.TrainingBackendBaseUrl}/api/trainings/rooms", _tokenProvider.AuthAccessToken);
            }
            catch (BackendAuthenticationException)
            {
                await _navigationService.NavigateAsync("/LoginPage");
                return new List<Room>();
            }
        }
        public async Task<TrainingDetail> GetTrainingDetailsAsync(string code) 
        {
            try
            {
                return await _backend.GetAsync<TrainingDetail>($"{_appSettings.TrainingBackendBaseUrl}/api/trainings/{code}", _tokenProvider.AuthAccessToken);
            }
            catch (BackendAuthenticationException)
            {
                await _navigationService.NavigateAsync("/LoginPage");
                return null;
            }
        }
        public async Task<IReadOnlyList<TrainingDetail>> GetFutureTrainingsAsync()
        {
            try
            {
                return await _backend.GetAsync<List<TrainingDetail>>($"{_appSettings.TrainingBackendBaseUrl}/api/trainings/future", _tokenProvider.AuthAccessToken);
            }
            catch (BackendAuthenticationException)
            {
                await _navigationService.NavigateAsync("/LoginPage");

                return new List<TrainingDetail>();
            }
        }
        public async Task<IReadOnlyList<TrainingDetail>> GetPastTrainingsAsync()
        {
            try
            {
                return await _backend.GetAsync<List<TrainingDetail>>($"{_appSettings.TrainingBackendBaseUrl}/api/trainings/past", _tokenProvider.AuthAccessToken);
            }
            catch (BackendAuthenticationException)
            {
                await _navigationService.NavigateAsync("/LoginPage");

                return new List<TrainingDetail>();
            }
        }
        public async Task RegisterApprenticeAsync(string code, string id)
        {
            try
            {
                await _backend.PostAsync($"{_appSettings.TrainingBackendBaseUrl}/api/trainings/{code}/register/{id}", new { }, _tokenProvider.AuthAccessToken);
            }
            catch (BackendAuthenticationException)
            {
                await _navigationService.NavigateAsync("/LoginPage");
            }
        }
        public async Task RegisterExternalApprenticeAsync(string code, ApprenticeCreateModel model)
        {
            try
            {
                await _backend.PostAsync($"{_appSettings.TrainingBackendBaseUrl}/api/trainings/{code}/registerExternal", model, _tokenProvider.AuthAccessToken);
            }
            catch (BackendAuthenticationException)
            {
                await _navigationService.NavigateAsync("/LoginPage");
            }
        }
        public async Task CompleteParticipation(string code, string id)
        {
            try
            {
                await _backend.PostAsync($"{_appSettings.TrainingBackendBaseUrl}/api/trainings/{code}/finish/{id}", new { }, _tokenProvider.AuthAccessToken);
            }
            catch (BackendAuthenticationException)
            {
                await _navigationService.NavigateAsync("/LoginPage");
            }
        }
    }
}
