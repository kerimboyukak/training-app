using Training.Mobile.ViewModels;

namespace Training.Mobile.Views;

public partial class TrainingDetailPage : ContentPage
{
    public TrainingDetailPage(TrainingDetailViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}