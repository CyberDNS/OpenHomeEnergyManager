using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules.Commands;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules.Queries;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules
{
    public class ModulesClient
    {
        private readonly HttpClient _httpClient;

        public ModulesClient(HttpClient httpClient)
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

        public async Task<ModuleDto> AddAsync(AddModuleDto command, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsync("", new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();

            var stringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ModuleDto>(stringContent);
        }

        public async Task UpdateInformationAsync(int id, UpdateInformationDto command, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsync($"{id}/Commands/UpdateInformation/", new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
