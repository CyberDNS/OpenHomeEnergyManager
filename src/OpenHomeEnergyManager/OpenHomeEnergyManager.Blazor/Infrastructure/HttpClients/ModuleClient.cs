using Newtonsoft.Json;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients
{
    public class ModuleClient
    {
        private readonly HttpClient _httpClient;

        public ModuleClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ModuleDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync("", cancellationToken);
            response.EnsureSuccessStatusCode();

            var stringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<ModuleDto>>(stringContent);
        }

        public async Task<ModuleDto> AddAsync(ModuleDto module, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsync("", new StringContent(JsonConvert.SerializeObject(module), Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();

            var stringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ModuleDto>(stringContent);
        }

        public async Task UpdateAsync(ModuleDto module, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsync($"{module.Id}", new StringContent(JsonConvert.SerializeObject(module), Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(ModuleDto module, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"{module.Id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }


        public async Task RestartModules(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsync("op/restart", null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
