using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.HomeAssistant
{
    public class HomeAssistantHttpClient
    {
        internal readonly HttpClient _httpClient;

        public HomeAssistantHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<dynamic> GetState(string entityId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/states/{entityId}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var stringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<dynamic>(stringContent);
        }

        public async Task CallService(string service, string entityId, CancellationToken cancellationToken = default)
        {
            string body = "{ \"entity_id\": \"" + entityId + "\" }";

            var response = await _httpClient.PostAsync($"api/services/{service}", new StringContent(body, Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
