using Microsoft.Extensions.Logging;
using Training.Mobile.ViewModels;
using Training.Mobile.Views;
using CommunityToolkit.Maui;
using Training.Mobile.Services;
using Training.Mobile.Services.Identity;
using Training.Mobile.Settings;
using Training.Mobile.Services.Backend;

namespace Training.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            RegisterDependencies(builder);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static void RegisterDependencies(this MauiAppBuilder builder)
        {
            IServiceCollection services = builder.Services;
            // Register your dependencies here
            // Registering interfaces and their implementations --> loosely coupled because we can change the implementation without changing the interface

            // Views
            services.AddTransient<LoginPage>(); // Transient means that a new instance is created every time the service is requested
            services.AddTransient<MainPage>();
            services.AddTransient<TrainingPage>();
            services.AddTransient<AddTrainingPage>();
            services.AddTransient<PastTrainingsPage>();
            services.AddTransient<TrainingDetailPage>();
            services.AddTransient<AddApprenticePage>();


            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<TrainingViewModel>();
            services.AddTransient<AddTrainingViewModel>();
            services.AddTransient<PastTrainingsViewModel>();
            services.AddTransient<TrainingDetailViewModel>();
            services.AddTransient<AddApprenticeViewModel>();



            // Services
            services.AddTransient<IToastService, ToastService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<INavigationService, NavigationService>();
            services.AddTransient<IBackendService, BackendService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ITrainingService, TrainingService>();



            // Other
            services.AddSingleton<ITokenProvider, TokenProvider>(); // Singleton means that a single instance ever gets created once
            services.AddSingleton<IAppSettings, DevAppSettings>();
        }
    }
}
