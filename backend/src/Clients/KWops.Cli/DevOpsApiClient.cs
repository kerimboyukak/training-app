using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace KWops.Cli
{
    class DevOpsApiClient
    {
        private readonly HttpClient _httpClient;

        public DevOpsApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> GetTeamsAsJsonAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response =  await _httpClient.GetAsync("https://localhost:8001/Teams");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"No successful response from API. Status code: {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}