using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ModuleServiceDefinitions.Queries;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ModuleServiceDefinitions
{
    public class ModuleServiceDefinitionsClient
    {
        private readonly HttpClient _httpClient;

        public ModuleServiceDefinitionsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ModuleServiceDefinitionDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync("", cancellationToken);
            response.EnsureSuccessStatusCode();

            var stringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<ModuleServiceDefinitionDto>>(stringContent);
        }
    }
}
