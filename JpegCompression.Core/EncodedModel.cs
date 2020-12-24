using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace JpegCompression.Core
{
    internal class EncodedModel
    {
        internal int OriginalIndex { get; }
        internal List<byte> Bytes { get; }

        internal EncodedModel(
            int originalIndex,
            List<byte> bytes)
        {
            OriginalIndex = originalIndex;
            Bytes = bytes;
        }

        internal EncodedModel(List<byte> bytes)
        {
            var originalIndexArray = bytes.Take(4).ToArray();
            OriginalIndex = BitConverter.ToInt32(originalIndexArray, 0);

            Bytes = bytes.Skip(4).ToList();
        }

        internal List<byte> ToByteList()
        {
            var encodedBytes = new List<byte>();

            encodedBytes.AddRange(BitConverter.GetBytes(OriginalIndex));
            encodedBytes.AddRange(Bytes);

            return encodedBytes;
        }
    }
}
