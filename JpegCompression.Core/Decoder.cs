using JpegCompression.Core.Bwt;
using JpegCompression.Core.Mtf;
using System.Collections.Generic;

namespace JpegCompression.Core
{
    public class Decoder
    {
        private readonly List<byte> _encodedBytes;
        private List<byte> _decodedBytes;

        public Decoder(List<byte> encodedBytes)
        {
            _encodedBytes = encodedBytes;
        }

        public List<byte> Decode()
        {
            var encodedModel = new EncodedModel(_encodedBytes);

            var mtfBytes = encodedModel.Bytes;


            var mtfCoder = new MtfCoder();
            var bwtBytes = mtfCoder.Decode(mtfBytes);

            var bwtModel = new BwtModel(encodedModel.OriginalIndex, bwtBytes);
            var bwtCoder = new BwtCoder();
            _decodedBytes = bwtCoder.Decode(bwtModel);

            return _decodedBytes;
        }
    }
}
