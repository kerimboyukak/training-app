using Training.Mobile.ViewModels;

namespace Training.Mobile.Views;

public partial class AddApprenticePage : ContentPage
{
	public AddApprenticePage(AddApprenticeViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}