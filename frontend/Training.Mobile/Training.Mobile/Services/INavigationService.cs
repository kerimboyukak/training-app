namespace Training.Mobile.Services;

public interface INavigationService     // Encapsulates navigation to other screens
{
    Task NavigateAsync(string routeName);
    Task NavigateRelativeAsync(string routeName);
}