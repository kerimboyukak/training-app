using Training.Mobile.ViewModels;


namespace Training.Mobile.Views

{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

        }
    }
}
