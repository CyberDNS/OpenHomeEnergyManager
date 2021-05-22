using Newtonsoft.Json;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients
{
    public class ModuleServiceDefinitionClient
    {
        private readonly HttpClient _httpClient;

        public ModuleServiceDefinitionClient(HttpClient httpClient)
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
