using JpegCompression.Core.Bwt;
using JpegCompression.Core.Mtf;
using System.Collections.Generic;

namespace JpegCompression.Core
{
    public class Encoder
    {
        private readonly List<byte> _originalBytes;
        private List<byte> _encodedBytes;

        public Encoder(List<byte> originalBytes)
        {
            _originalBytes = originalBytes;
        }

        public List<byte> Encode()
        {
            var bwtCoder = new BwtCoder();
            var bwtModel = bwtCoder.Encode(_originalBytes);

            var mtfCoder = new MtfCoder();
            var mtfBytes = mtfCoder.Encode(bwtModel.Bytes);

            var encodedModel = new EncodedModel(bwtModel.OriginalIndex, mtfBytes);
            _encodedBytes = encodedModel.ToByteList();

            return _encodedBytes;
        }
    }
}
