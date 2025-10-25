namespace Training.Mobile.Services.Identity;

public interface IIdentityService
{
    Task<ILoginResult> LoginAsync();
    Task LogoutAsync(string identityToken);
}