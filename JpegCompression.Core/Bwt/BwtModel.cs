using System.Collections.Generic;

namespace JpegCompression.Core.Bwt
{
    internal class BwtModel
    {
        internal int OriginalIndex { get; }
        internal List<byte> Bytes { get; }

        internal BwtModel(int originalIndex, List<byte> bytes)
        {
            OriginalIndex = originalIndex;
            Bytes = bytes;
        }
    }
}
