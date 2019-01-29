using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO.Compression;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Webtoys.CQRS.ImageEncoder.Queries
{
    public class DecodeFileFromImage : IRequest<DecodeFileFromImageResult>
    {
        public DecodeFileFromImage(IFormFile file)
        {
            File = file;
        }

        public IFormFile File { get; set; }
    }

    public class DecodeFileFromImageResult
    {
        public DecodeFileFromImageResult(string name, byte[] data)
        {
            Name = name;
            Data = data;
        }

        public string Name { get; private set; }
        public byte[] Data { get; private set; }
    }

    public class DecodeFileFromImageHandler : IRequestHandler<DecodeFileFromImage, DecodeFileFromImageResult>
    {
        public async Task<DecodeFileFromImageResult> Handle(DecodeFileFromImage request, CancellationToken cancellationToken)
        {
            var imageBytes = await GetImageBytes(request);
            //var 
            //var decompressedBytes = DecompressBytes();
            throw new Exception();
        }

        private async Task<byte[]> GetImageBytes(DecodeFileFromImage request)
        {
            byte[] imageBytes;
            using (var memory = new MemoryStream())
            using (var fileStream = request.File.OpenReadStream())
            {
                await fileStream.CopyToAsync(memory);
                imageBytes = memory.ToArray();
            }
            return imageBytes;
        }

        //private IEnumerable<byte> GetBytesFromImage(byte[] imageBytes)
        //{
        //    using (var image = Image.Load(imageBytes))
        //    {

        //    }
        //}

        private byte[] DecompressBytes(byte[] compressedBytes)
        {
            using (var memoryStream = new MemoryStream())
            using (var zip = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                zip.Write(compressedBytes, 0, compressedBytes.Length);
                return memoryStream.ToArray();
            }
        }
    }
}
