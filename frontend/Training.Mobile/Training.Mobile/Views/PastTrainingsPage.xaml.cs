using Training.Mobile.ViewModels;

namespace Training.Mobile.Views;

public partial class PastTrainingsPage : ContentPage
{
    private PastTrainingsViewModel _viewModel;

    public PastTrainingsPage(PastTrainingsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }
}