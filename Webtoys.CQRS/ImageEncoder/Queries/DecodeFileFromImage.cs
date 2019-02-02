using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Webtoys.CQRS.ImageEncoder.Models;

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
            var fileBytes = GetBytesFromImage(imageBytes).ToArray();
            var metadataLength = BitConverter.ToInt32(fileBytes.Take(4).ToArray());
            var metadata = GetMetadata(fileBytes, metadataLength);
            return new DecodeFileFromImageResult(
                metadata.FileName,
                fileBytes.Skip(4 + metadataLength).Take(metadata.ByteCount).ToArray()
                );
        }

        private async Task<byte[]> GetImageBytes(DecodeFileFromImage request)
        {
            using (var memory = new MemoryStream())
            using (var fileStream = request.File.OpenReadStream())
            {
                await fileStream.CopyToAsync(memory);
                return memory.ToArray();
            }
        }

        private IEnumerable<byte> GetBytesFromImage(byte[] imageBytes)
        {
            using (var image = Image.Load(imageBytes))
            {
                for (var y = 0; y < image.Height; y++)
                {
                    for (var x = 0; x < image.Width; x++)
                    {
                        yield return image[x, y].R;
                        yield return image[x, y].G;
                        yield return image[x, y].B;
                        yield return image[x, y].A;
                    }
                }
            }
        }

        private ImageMetadata GetMetadata(byte[] decompressedBytes, int metadataLength)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(decompressedBytes, 4, metadataLength))
            {
                return binaryFormatter.Deserialize(memoryStream) as ImageMetadata;
            }
        }
    }
}
