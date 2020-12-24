using System.Collections.Generic;
using System.Linq;

namespace JpegCompression.Core.Mtf
{
    internal class MtfCoder
    {
        private List<byte> _bytes;

        private byte MoveToFront(List<byte> symbols, byte element)
        {
            if (symbols[0] == element)
            {
                return 0;
            }

            var elementIndex = -1;
            for (int i = symbols.Count - 1; i > 0; i--)
            {
                if (symbols[i] == element)
                {
                    elementIndex = i;
                }

                if (elementIndex != -1)
                {
                    symbols[i] = symbols[i - 1];
                }
            }

            symbols[0] = element;
            return (byte)elementIndex;
        }

        private void MoveToFront(List<byte> symbols, int elementIndex)
        {
            var element = symbols[elementIndex];

            for (int i = elementIndex; i > 0; i--)
            {
                symbols[i] = symbols[i - 1];
            }

            symbols[0] = element;
        }

        internal List<byte> Encode(List<byte> bytes)
        {
            _bytes = bytes;

            var byteLength = 256;

            byte initialValue = 0;
            var symbols = Enumerable.Range(0, byteLength).Select(i => initialValue++).ToList();

            var encodedBytes = new List<byte>(_bytes.Count);

            for (int i = 0; i < _bytes.Count; i++)
            {
                var index = MoveToFront(symbols, _bytes[i]);
                encodedBytes.Add(index);
            }

            return encodedBytes;
        }

        internal List<byte> Decode(List<byte> bytes)
        {
            _bytes = bytes;

            var byteLength = 256;

            byte initialValue = 0;
            var symbols = Enumerable.Range(0, byteLength).Select(i => initialValue++).ToList();

            var decodedBytes = new List<byte>(_bytes.Count);

            for (int i = 0; i < _bytes.Count; i++)
            {
                int index = _bytes[i];
                decodedBytes.Add(symbols[index]);
                MoveToFront(symbols, index);
            }

            return decodedBytes;
        }
    }
}
