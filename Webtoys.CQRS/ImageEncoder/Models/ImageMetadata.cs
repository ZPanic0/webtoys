using System;

namespace Webtoys.CQRS.ImageEncoder.Models
{
    [Serializable]
    public class ImageMetadata
    {
        public string FileName { get; set; }
        public int ByteCount { get; set; }
    }
}
