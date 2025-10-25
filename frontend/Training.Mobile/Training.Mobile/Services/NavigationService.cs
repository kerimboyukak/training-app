namespace Training.Mobile.Services;

public class NavigationService : INavigationService
{
    public async Task NavigateAsync(string routeName)
    {
        await Shell.Current.GoToAsync($"//{routeName}");
    }

    public async Task NavigateRelativeAsync(string routeName)
    {
        await Shell.Current.GoToAsync(routeName);
    }
}