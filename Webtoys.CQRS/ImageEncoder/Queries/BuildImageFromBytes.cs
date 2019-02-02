using MediatR;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Webtoys.CQRS.ImageEncoder.Queries
{
    public class BuildImageFromBytes : IRequest<byte[]>
    {
        public BuildImageFromBytes(byte[] bytes)
        {
            Bytes = bytes;
        }

        public byte[] Bytes { get; set; }
    }

    public class BuildImageFromBytesHandler : IRequestHandler<BuildImageFromBytes, byte[]>
    {
        public Task<byte[]> Handle(BuildImageFromBytes request, CancellationToken cancellationToken)
        {
            var pixels = BuildPixels(request.Bytes).ToArray();
            var dimensions = (int)Math.Ceiling(Math.Sqrt(pixels.Length));
            return Task.FromResult(BuildImage(dimensions, dimensions, pixels));
        }

        private IEnumerable<Rgba32> BuildPixels(byte[] bytes)
        {
            var queue = new Queue<byte>(bytes);

            while (queue.Count > 0)
            {
                yield return new Rgba32(
                    queue.Dequeue(),
                    queue.Count > 0 ? queue.Dequeue() : (byte)0,
                    queue.Count > 0 ? queue.Dequeue() : (byte)0,
                    queue.Count > 0 ? queue.Dequeue() : (byte)0
                    );
            }
        }

        private byte[] BuildImage(int width, int height, Rgba32[] pixels)
        {
            var queue = new Queue<Rgba32>(pixels);

            using (var memoryStream = new MemoryStream())
            using (var image = new Image<Rgba32>(width, height))
            {
                for (var y = 0; y < height && queue.Count > 0; y++)
                {
                    for (var x = 0; x < width && queue.Count > 0; x++)
                    {
                        image[x, y] = queue.Dequeue();
                    }
                }

                image.SaveAsPng(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
