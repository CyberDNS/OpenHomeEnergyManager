using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles.Commands;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles.Queries;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles
{
    public class VehiclesClient
    {
        private readonly HttpClient _httpClient;

        public VehiclesClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<VehicleDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync("", cancellationToken);
            response.EnsureSuccessStatusCode();

            var stringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<VehicleDto>>(stringContent);
        }

        public async Task<VehicleDto> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{id}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var stringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<VehicleDto>(stringContent);
        }

        public async Task<VehicleDatasetDto> GetNowDataAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{id}/Data/Now", cancellationToken);
            response.EnsureSuccessStatusCode();

            var stringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<VehicleDatasetDto>(stringContent);
        }


        public async Task<IEnumerable<VehicleDatasetDto>> GetHistorizationDataAsync(int id, TimeSpan timeSpan, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{id}/Data/Historization?minutes={timeSpan.TotalMinutes}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var stringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<VehicleDatasetDto>>(stringContent);
        }


        public async Task AddAsync(AddVehicleDto command, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsync("", new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
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
