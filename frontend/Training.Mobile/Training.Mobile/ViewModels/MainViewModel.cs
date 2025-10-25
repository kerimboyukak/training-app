using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Training.Mobile.Services;

namespace Training.Mobile.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateToLoginCommand = new Command(async () => await NavigateToLogin());

        }
        public ICommand NavigateToLoginCommand { get; }

        private async Task NavigateToLogin()
        {
            await _navigationService.NavigateAsync("LoginPage");
        }

    }
}
