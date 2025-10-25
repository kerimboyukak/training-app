using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Training.Mobile.ViewModels;

//  INotifyPropertyChanged interface allows your ViewModel properties to notify the UI(View) when their values change,
//  enabling data binding and keeping the UI responsive to data changes without manual updates.
public class BaseViewModel : INotifyPropertyChanged
{
    private bool _isBusy = false;
    public bool IsBusy
    {
        get => _isBusy; 
        set => SetProperty(ref _isBusy, value);
    }

    string _title = string.Empty;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    // This is a generic helper method used to update a property’s value and raise the PropertyChanged event.
    protected bool SetProperty<T>(ref T backingStore, T value,
        [CallerMemberName] string propertyName = "",
        Action? onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;
    // The INotifyPropertyChanged interface defines an event PropertyChanged, which is raised whenever a property value changes.

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        var changed = PropertyChanged;
        if (changed == null)
            return;

        changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        // The OnPropertyChanged method raises this event, passing the name of the changed property.
    }
    #endregion
}
