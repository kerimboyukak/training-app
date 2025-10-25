namespace Training.Mobile.Settings;

public class DevAppSettings : IAppSettings
{
    // 10.0.2.2 will be mapped to the host machine's IP address from the Android emulator (localhost)
    public string OidcAuthority => "http://10.0.2.2:9000"; //Or an Azure url like "https://identity-yxe7amqvozxwe-app.azurewebsites.net";
    public string OidcClientId => "training.mobile";
    public string OidcClientSecret => "MobileClientSecret";
    public string OidcScope => "openid training.read manage";
    public string OidcRedirectUri => "myapp://mauicallback";
    public string PostLogoutRedirectUri => "myapp://mauilogout";

    public string TrainingBackendBaseUrl => "http://10.0.2.2:7000"; //Or an Azure url like "https://devops-yxe7amqvozxwe-app.azurewebsites.net";
}