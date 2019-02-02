using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Webtoys.CQRS.ImageEncoder.Models;

namespace Webtoys.CQRS.ImageEncoder.Queries
{
    public class EncodeFileToImage : IRequest<string>
    {
        public EncodeFileToImage(IFormFile file)
        {
            File = file;
        }

        public IFormFile File { get; set; }
    }

    public class EncodeFileToImageHandler : IRequestHandler<EncodeFileToImage, string>
    {
        private IMediator Mediator;
        public EncodeFileToImageHandler(IMediator mediator)
        {
            Mediator = mediator;
        }

        public async Task<string> Handle(EncodeFileToImage request, CancellationToken cancellationToken)
        {
            var fileBytes = await GetBytes(request.File);
            var metadataBytes = GetMetadataBytes(new ImageMetadata
            {
                ByteCount = fileBytes.Length,
                FileName = request.File.FileName
            });
            var metadataLengthInBytes = BitConverter.GetBytes(metadataBytes.Length);

            var imageBytes = await Mediator.Send(
                new BuildImageFromBytes(
                    metadataLengthInBytes
                        .Concat(metadataBytes)
                        .Concat(fileBytes)
                        .ToArray()));

            return Convert.ToBase64String(imageBytes);
        }

        private async Task<byte[]> GetBytes(IFormFile file)
        {
            using (var memory = new MemoryStream())
            using (var fileStream = file.OpenReadStream())
            {
                await fileStream.CopyToAsync(memory);
                return memory.ToArray();
            }
        }

        private byte[] GetMetadataBytes(ImageMetadata metadata)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, metadata);
                return memoryStream.ToArray();
            }
        }
    }
}
