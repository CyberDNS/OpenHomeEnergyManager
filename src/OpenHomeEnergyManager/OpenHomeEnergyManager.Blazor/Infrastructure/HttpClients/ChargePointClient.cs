using Newtonsoft.Json;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients
{
    public class ChargePointClient
    {
        private readonly HttpClient _httpClient;

        public ChargePointClient(HttpClient httpClient)
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

        public async Task<ChargePointDto> AddAsync(ChargePointDto chargePoint, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsync("", new StringContent(JsonConvert.SerializeObject(chargePoint), Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();

            var stringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ChargePointDto>(stringContent);
        }

        public async Task UpdateAsync(ChargePointDto chargePoint, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsync($"{chargePoint.Id}", new StringContent(JsonConvert.SerializeObject(chargePoint), Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(ChargePointDto chargePoint, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"{chargePoint.Id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<ChargePointDatasetDto> GetCurrentDataAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{id}/Data/Current", cancellationToken);
            response.EnsureSuccessStatusCode();

            var stringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ChargePointDatasetDto>(stringContent);
        }
    }
}
