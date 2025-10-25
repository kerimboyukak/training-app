using System.IdentityModel.Tokens.Jwt;

namespace Training.Mobile.Services.Identity;

public class TokenProvider : ITokenProvider
{
    private const string AccessToken = "access_token";
    private const string IdentityToken = "identity_token";
    private const string UserIdKey = "user_id";


    public string AuthAccessToken
    {
        get => Preferences.Get(AccessToken, string.Empty);      // Preferences storage is used to store the access token --> MAUI class that provides a way to store and retrieve key-value pairs
        set => Preferences.Set(AccessToken, value);
    }

    public string AuthIdentityToken
    {
        get => Preferences.Get(IdentityToken, string.Empty);
        set
        {
            Preferences.Set(IdentityToken, value);
            DecodeAndStoreUserId(value);
        }
    }
    public string UserId
    {
        get => Preferences.Get(UserIdKey, string.Empty);
        private set => Preferences.Set(UserIdKey, value);
    }

    private void DecodeAndStoreUserId(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            UserId = string.Empty;
            return;
        }

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

        UserId = userId ?? string.Empty;
    }
}