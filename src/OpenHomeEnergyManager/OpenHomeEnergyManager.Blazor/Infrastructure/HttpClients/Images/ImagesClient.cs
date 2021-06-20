using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Images
{
    public class ImagesClient
    {
        private readonly ILogger<ImagesClient> _logger;
        private readonly HttpClient _httpClient;

        public ImagesClient(ILogger<ImagesClient> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }


        public async Task<(Stream Stream, string MediaType)> DownloadAsync(string fileName)
        {
            var response = await _httpClient.GetAsync($"{fileName}");
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType.MediaType);
        }


        public async Task UploadAsync(IBrowserFile file, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 20 * 1024 * 1024));

                content.Add(content: fileContent, "file", fileName);

                var response = await _httpClient.PostAsync("", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A problem occured on uploading image");
            }
        }
    }
}
