using API.DTOs.WordPress;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace API.Services
{
    public class BaseWordPressService <T> where T : BasedWordPressDTO
    {
        private readonly string _siteUrl;

        public BaseWordPressService(string siteUrl)
        {
            this._siteUrl = siteUrl;
        }

        private string buildEndpointUrl(string siteUrl, Dictionary<string, string> queryParams)
        {

            var queryString = string.Join("&", queryParams
                           .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
                                      .Select(kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));
            return $"{siteUrl}?{queryString}";
        }

        protected virtual List<T> deserializeJsonCollection(string content)
        {
            var result = JsonConvert.DeserializeObject<List<T>>(content);

            return result ?? new List<T>();
        }

        protected virtual T deserializeJson(string content)
        {
            var result = JsonConvert.DeserializeObject<T>(content);
            return result;
        }

        private async Task<string> DoGetAsync(string url, CancellationToken ct)
        {
            using HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync(url, ct);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync(ct);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            return string.Empty;
        }

        protected async Task<List<T>> GetAllAsync(string endpoint, Dictionary<string, string> queryParams, CancellationToken ct)
        {
            string url = buildEndpointUrl($"{_siteUrl}/{endpoint}", queryParams);
            var responseBody = await DoGetAsync(url, ct);
            return deserializeJsonCollection(responseBody);
        }

        protected async Task<T> GetAsync(string endpoint, Dictionary<string, string> queryParams, CancellationToken ct)
        {
            string url = buildEndpointUrl($"{_siteUrl}/{endpoint}", queryParams);
            var responseBody = await DoGetAsync(url, ct);
            return deserializeJson(responseBody);
        }
    }
}
