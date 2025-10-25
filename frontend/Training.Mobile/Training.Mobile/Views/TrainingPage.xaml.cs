using Training.Mobile.ViewModels;

namespace Training.Mobile.Views;

public partial class TrainingPage : ContentPage
{
    private TrainingViewModel _viewModel;
	public TrainingPage(TrainingViewModel viewModel)
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