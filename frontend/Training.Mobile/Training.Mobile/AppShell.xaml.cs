using Training.Mobile.Views;

namespace Training.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("AddTrainingPage", typeof(AddTrainingPage));
            Routing.RegisterRoute("AddApprenticePage", typeof(AddApprenticePage));
            Routing.RegisterRoute("PastTrainingsPage", typeof(PastTrainingsPage));
            Routing.RegisterRoute("TrainingDetailPage", typeof(TrainingDetailPage));
        }
    }
}
