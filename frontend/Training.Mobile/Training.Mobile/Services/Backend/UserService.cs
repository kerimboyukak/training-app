using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Training.Mobile.Exceptions;
using Training.Mobile.Models;
using Training.Mobile.Services.Identity;
using Training.Mobile.Settings;

namespace Training.Mobile.Services.Backend
{
    public class UserService : IUserService
    {
        private readonly IBackendService _backendService;
        private readonly ITokenProvider _tokenProvider;
        private readonly IAppSettings _appSettings;

        private bool _isCoach;

        public UserService(IBackendService backendService, ITokenProvider tokenProvider, IAppSettings appSettings)
        {
            _backendService = backendService;
            _tokenProvider = tokenProvider;
            _appSettings = appSettings;
        }

        public bool IsCoach
        {
            get => _isCoach;
            set
            {
                if (_isCoach != value)
                {
                    _isCoach = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Id => _tokenProvider.UserId;

        public async Task ClearUserInfoAsync()
        {
            IsCoach = false;
        }

        public async Task<bool> CheckIfUserIsCoachAsync(string subjectId)
        {
            try
            {
                var coach = await _backendService.GetAsync<Coach>($"{_appSettings.TrainingBackendBaseUrl}/api/coaches/{subjectId}", _tokenProvider.AuthAccessToken);
                return coach != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}