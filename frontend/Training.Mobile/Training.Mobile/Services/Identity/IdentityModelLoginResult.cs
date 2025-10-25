using IdentityModel.OidcClient;
using Training.Mobile.Services.Identity;

namespace Training.Mobile.Services.Identity;

// This class wraps the LoginResult class in the IdentityModel.OidcClient namespace and implements ILoginResult
// Thanks to this class it is possible to return an ILoginResult in the LoginAsync method of the IIdentityService (instead of directly returning IdentityModel.OidcClient.LoginResult)
// This keeps the IIdentityService independent from the IdentityModel framework which in turn makes the application more testable and maintainable
public class IdentityModelLoginResult : ILoginResult        
{
    private readonly LoginResult _result;

    public IdentityModelLoginResult(LoginResult result)
    {
        _result = result;
    }

    public IdentityModelLoginResult(string error, string errorDescription)
    {
        _result = new LoginResult(error, errorDescription);
    }

    public bool IsError => _result.IsError;
    public string AccessToken => _result.AccessToken;
    public string IdentityToken => _result.IdentityToken;
    public string Error => _result.Error;
    public string ErrorDescription => _result.ErrorDescription;
}