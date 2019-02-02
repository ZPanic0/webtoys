using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Webtoys.CQRS.ImageEncoder.Queries;
using Webtoys.Web.Models;

namespace Webtoys.Web.Controllers
{
    [Route("api/[controller]")]
    public class ImageEncoderController : Controller
    {
        public readonly IMediator Mediator;
        public readonly IMemoryCache Cache;
        public ImageEncoderController(IMediator mediator, IMemoryCache cache)
        {
            Mediator = mediator;
            Cache = cache;
        }

        [HttpPost("[action]")]
        public async Task<string> Encode(ImageEncodeDecodeViewModel file)
        {
            return await Mediator.Send(new EncodeFileToImage(file.File));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Decode(IFormFile file)
        {
            var guid = Guid.NewGuid().ToString();
            Cache.Set(guid, await Mediator.Send(new DecodeFileFromImage(file)));
            return Json(new { url = Url.Action("DownloadDecodedFile", new { id = guid }) });
        }

        [HttpGet("[action]")]
        public IActionResult DownloadDecodedFile(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Cache.TryGetValue(id, out DecodeFileFromImageResult file))
            {
                return NotFound();
            }
            if (!new FileExtensionContentTypeProvider().TryGetContentType(file.Name, out string contentType))
            {
                contentType = "application/octet-stream";
            }
            Cache.Remove(id);
            return File(file.Data, contentType, file.Name);
        }
    }
}
