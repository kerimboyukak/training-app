namespace Training.Mobile.Services.Backend;

public interface IBackendService
{
    Task<TResult> GetAsync<TResult>(string uri, string token = "");

    Task<TResult> PostAsync<TResult>(string uri, TResult data, string token = "");

    Task<TResult> PostAsync<TResult>(string uri, string data, string clientId = "", string clientSecret = "");

    Task<TResult> PutAsync<TResult>(string uri, TResult data, string token = "");

    Task DeleteAsync(string uri, string token = "");
}