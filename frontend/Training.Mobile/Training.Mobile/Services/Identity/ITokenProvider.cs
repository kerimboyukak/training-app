namespace Training.Mobile.Services.Identity;

public interface ITokenProvider     // Can store and retrieve the access and identity tokens in the device
{
    string AuthAccessToken { get; set; }
    string AuthIdentityToken { get; set; }
    string UserId { get; }

}