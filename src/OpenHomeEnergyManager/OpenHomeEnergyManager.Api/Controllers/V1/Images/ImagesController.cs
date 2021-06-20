using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Parsing;
using static System.Net.WebRequestMethods;

namespace OpenHomeEnergyManager.Api.Controllers.V1.Images
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public ImagesController(IWebHostEnvironment env, ILogger<ImagesController> logger)
        {
            _env = env;
        }

        [HttpGet("{filename}")]
        public async Task<IActionResult> Download(string filename)
        {
            try
            {
                var folderPath = Path.Combine(_env.ContentRootPath, "data", "images");

                var filenameWithExtension = Directory.GetFiles(folderPath, $"{filename}.*").SingleOrDefault();
                if (filenameWithExtension is null) { return NotFound(); } 

                var filePath = Path.Combine(folderPath, filenameWithExtension);

                MemoryStream memory = new MemoryStream();
                using (FileStream stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;
                return File(memory, $"image/{Path.GetExtension(filePath).TrimStart('.').ToLower()}", Path.GetFileName(filePath));
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Upload([FromForm] IFormFile file)
        {
            var folderPath = Path.Combine(_env.ContentRootPath, "data", "images");
            if (!Directory.Exists(folderPath)) { Directory.CreateDirectory(folderPath); }

            var filePath = Path.Combine(folderPath, file.FileName);
            await using FileStream fs = new(filePath, FileMode.Create);
            await file.CopyToAsync(fs);

            return NoContent();
        }
    }
}
