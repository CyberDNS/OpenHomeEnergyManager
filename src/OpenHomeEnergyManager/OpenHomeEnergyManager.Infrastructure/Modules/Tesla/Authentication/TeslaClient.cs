using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenHomeEnergyManager.Infrastructure.Modules.Tesla.Authentication.Dtos;
using Polly;

namespace OpenHomeEnergyManager.Infrastructure.Modules.Tesla.Authentication
{
    public class TeslaClient
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public TeslaClient(ILogger<TeslaClient> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public void SetAccessToken(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<ChargeStateDto> GetChargeState()
        {
            string query = "vehicles/151458224580/data_request/charge_state";
            var response = await _httpClient.GetAsync(query);
            response.EnsureSuccessStatusCode();

            var dto = await response.Content.ReadFromJsonAsync<ChargeStateDto>();
            return dto;
        }

        public async Task<VehicleDto> GetVehicle()
        {
            string query = "vehicles/151458224580";
            var response = await _httpClient.GetAsync(query);
            response.EnsureSuccessStatusCode();

            var dto = await response.Content.ReadFromJsonAsync<VehicleDto>();
            return dto;
        }

        public async Task WakeUp()
        {
            string query = "vehicles/151458224580/wake_up";

            int retryCountdown = 6;
            bool isOnline = false;
            do
            {
                if (retryCountdown < 0) { throw new ApplicationException("Could not wake up car"); }

                var response = await _httpClient.PostAsync(query, null);
                response.EnsureSuccessStatusCode();
                var dto = await response.Content.ReadFromJsonAsync<WakeUpDto>();

                isOnline = dto.Response.State.Equals("online");

                await Task.Delay(TimeSpan.FromSeconds(10));
            } while (!isOnline);
        }

        public async Task StartCharging()
        {
            string query = "vehicles/151458224580/command/charge_start";
            var response = await _httpClient.PostAsync(query, null);
            response.EnsureSuccessStatusCode();
        }

        public async Task StopCharging()
        {
            string query = "vehicles/151458224580/command/charge_stop";
            var response = await _httpClient.PostAsync(query, null);
            response.EnsureSuccessStatusCode();
        }
    }
}
