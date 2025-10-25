using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Training.Mobile.Services;

public class ToastService : IToastService
{
    public Task DisplayToastAsync(string message)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        string text = message;
        ToastDuration duration = ToastDuration.Long;
        double fontSize = 14;

        var toast = Toast.Make(text, duration, fontSize);

        return toast.Show(cancellationTokenSource.Token);
    }
}