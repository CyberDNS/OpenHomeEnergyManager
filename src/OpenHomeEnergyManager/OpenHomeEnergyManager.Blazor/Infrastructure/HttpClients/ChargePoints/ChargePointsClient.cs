using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Commands;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Queries;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints
{
    public class ChargePointsClient
    {
        private readonly HttpClient _httpClient;

        public ChargePointsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ChargePointDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync("", cancellationToken);
            response.EnsureSuccessStatusCode();

            var stringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<ChargePointDto>>(stringContent);
        }

        public async Task<ChargePointDto> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{id}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var stringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ChargePointDto>(stringContent);
        }

        public async Task<ChargePointDatasetDto> GetNowDataAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{id}/Data/Now", cancellationToken);
            response.EnsureSuccessStatusCode();

            var stringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ChargePointDatasetDto>(stringContent);
        }


        public async Task AddAsync(AddChargePointDto command, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsync("", new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateInformationAsync(int id, UpdateInformationDto command, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsync($"{id}/Commands/UpdateInformation/", new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task SetCurrentAsync(int id, SetCurrentDto command, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsync($"{id}/Commands/SetCurrent", new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task SelectChargeModeAsync(int id, SelectChargeModeDto command, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsync($"{id}/Commands/SelectChargeMode", new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task AttributeVehicleAsync(int id, AttributeVehicleDto command, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsync($"{id}/Commands/AttributeVehicle", new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

    }
}
