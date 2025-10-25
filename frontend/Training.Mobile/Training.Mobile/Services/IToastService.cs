namespace Training.Mobile.Services;

public interface IToastService  // Encapsulates the display of toast messages (short messages that appear on the screen)
{
    Task DisplayToastAsync(string message);
}