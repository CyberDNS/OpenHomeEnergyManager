using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Images;

namespace OpenHomeEnergyManager.Blazor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ImagesClient _imagesClient;

        public ImagesController(ImagesClient imagesClient)
        {
            _imagesClient = imagesClient;
        }

        [HttpGet("{filename}")]
        public async Task<IActionResult> Download(string filename)
        {
            try
            {
                MemoryStream memory = new MemoryStream();

                var result = await _imagesClient.DownloadAsync(filename);

                using (Stream stream = result.Stream)
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, result.MediaType, Path.GetFileName(filename));
            }
            catch
            {
                return NotFound();
            }


        }
    }
}
