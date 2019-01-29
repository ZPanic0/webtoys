using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using Webtoys.Web.Models;

namespace Webtoys.Web.Controllers
{
    [Route("api/[controller]")]
    public class ImageEncoderController : Controller
    {
        [HttpPost("[action]")]
        public string Encode(ImageEncodeDecodeViewModel file)
        {
            throw new Exception();
            return string.Empty;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Decode(IFormFile file)
        {
            var guid = Guid.NewGuid().ToString();
            byte[] fileBytes;
            using (var memory = new MemoryStream()) //Whole block can probably be part of query
            using (var fileStream = file.OpenReadStream())
            {
                await fileStream.CopyToAsync(memory);
                fileBytes = memory.ToArray();
            }
            TempData[guid] = JsonConvert.SerializeObject(new FileDataCacheModel
            {
                Name = file.FileName,
                Data = fileBytes //Do conversion operation before this
            });
            return Json(new { url = Url.Action("DownloadDecodedFile", new { id = guid }) });
        }

        [HttpGet("[action]")]
        public IActionResult DownloadDecodedFile(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !TempData.ContainsKey(id))
            {
                return NotFound();
            }

            var file = JsonConvert.DeserializeObject<FileDataCacheModel>((string)TempData[id]);

            if (!new FileExtensionContentTypeProvider().TryGetContentType(file.Name, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            return File(file.Data, contentType, file.Name);
        }
    }
}
