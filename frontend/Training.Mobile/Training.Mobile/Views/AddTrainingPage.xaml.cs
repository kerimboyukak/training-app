using Training.Mobile.ViewModels;

namespace Training.Mobile.Views;

public partial class AddTrainingPage : ContentPage
{
    private AddTrainingViewModel _viewModel;
    public AddTrainingPage(AddTrainingViewModel viewModel)
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