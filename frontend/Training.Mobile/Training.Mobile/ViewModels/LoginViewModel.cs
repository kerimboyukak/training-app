using Training.Mobile.Services;
using Training.Mobile.Services.Backend;
using Training.Mobile.Services.Identity;

namespace Training.Mobile.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IIdentityService? _identityService;
        private readonly ITokenProvider? _tokenProvider;
        private readonly INavigationService? _navigationService;
        private readonly IToastService? _toastService;
        private readonly IUserService? _userService;

        private bool _isLoggedIn;
        private bool _isLoggedOut;


        public LoginViewModel(IIdentityService identityService, ITokenProvider tokenProvider, INavigationService navigationService, IToastService toastService, IUserService userService)
        {
            _identityService = identityService;
            _tokenProvider = tokenProvider;
            _navigationService = navigationService;
            _toastService = toastService;
            _userService = userService;

            LoginCommand = new Command(async () => await Login(), () => !IsBusy && IsLoggedOut);
            LogoutCommand = new Command(async() => await Logout(), () => !IsBusy && IsLoggedIn);

        }

        public bool IsLoggedIn
        {
            get => !string.IsNullOrEmpty(_tokenProvider?.AuthIdentityToken);
            set => SetProperty(ref _isLoggedIn, value);
        }

        public bool IsLoggedOut
        {
            get => string.IsNullOrEmpty(_tokenProvider?.AuthIdentityToken);
            set => SetProperty(ref _isLoggedOut, value);
        }

        public Command LoginCommand { get; }
        public Command LogoutCommand { get; }


        public async Task Login()
        {
            if (IsBusy) return;

            if (_identityService == null || _tokenProvider == null || _navigationService == null || _toastService == null || _userService == null)
                return;

            IsBusy = true;
            UpdateCommandStates();

            var loginResult = await _identityService.LoginAsync();

            if (loginResult.IsError)
            {
                await _toastService.DisplayToastAsync(loginResult.ErrorDescription);
                IsBusy = false;
                UpdateCommandStates();
                return;
            }

            _tokenProvider.AuthAccessToken = loginResult.AccessToken;
            _tokenProvider.AuthIdentityToken = loginResult.IdentityToken;

            var subjectId = _tokenProvider.UserId;
            var isCoach = await _userService.CheckIfUserIsCoachAsync(subjectId);
            _userService.IsCoach = isCoach;

            await _navigationService.NavigateAsync("TrainingPage");

            IsBusy = false;
            UpdateCommandStates();
        }
        public async Task Logout()
        {
            if (_tokenProvider != null && _identityService != null && _userService != null)
            {
                IsBusy = true;
                UpdateCommandStates();

                await _identityService.LogoutAsync(_tokenProvider.AuthIdentityToken);
                _tokenProvider.AuthIdentityToken = string.Empty;
                await _userService.ClearUserInfoAsync();

                IsBusy = false;
                UpdateCommandStates();
            }
        }

        private void UpdateCommandStates()
        {
            LoginCommand.ChangeCanExecute();
            LogoutCommand.ChangeCanExecute();
        }
    }
}
