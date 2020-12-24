using System;
using System.IO;

namespace JpegCompression.ArithmeticCoding
{
    class Decoder
    {
        private static readonly int BitsUsed = 64;
        private static readonly ulong Half = 1UL << (BitsUsed - 1);
        private static readonly ulong Quarter = 1UL << (BitsUsed - 2);

        private BitReader _reader;

        private ulong _low;
        private ulong _range;
        private ulong _length;
        private ulong[] _byteCounts;

        public void Decode(BitReader reader, Stream writer)
        {
            if (!writer.CanWrite)
            {
                throw new InvalidOperationException();
            }

            _reader = reader;

            Decode(writer);
        }

        private void Decode(Stream writer)
        {
            _length = _reader.ReadUInt64();

            _byteCounts = new ulong[256];

            for (int i = 0; i < _byteCounts.Length; i++)
            {
                _byteCounts[i] = _reader.ReadUInt64();
                if (i != 0)
                {
                    _byteCounts[i] += _byteCounts[i - 1];
                }
            }

            _range = Half;
            for (int i = 0; i < BitsUsed; i++)
            {
                _low = (_low << 1) + Convert.ToUInt64(_reader.ReadBit());
            }

            for (ulong i = 0; i < _length; i++)
            {
                ulong rangeUnit = _range / _length;

                ulong target = Math.Min(_length - 1, _low / rangeUnit);

                int pos = Array.BinarySearch(_byteCounts, target);

                byte b = pos >= 0 ? (byte)(pos + 1) : (byte)(~pos);

                writer.WriteByte(b);

                FixValues(b);
            }
        }

        private void FixValues(byte b)
        {
            ulong rangeUnit = _range / _length;

            ulong byteCumulatedCount = _byteCounts[b];
            ulong previousByteCumultedCount = b == 0 ? 0 : _byteCounts[b - 1];

            _low -= rangeUnit * previousByteCumultedCount;

            if (byteCumulatedCount < _length)
            {
                _range = rangeUnit * (byteCumulatedCount - previousByteCumultedCount);
            }
            else
            {
                _range -= rangeUnit * previousByteCumultedCount;
            }

            while (_range <= Quarter)
            {
                _range *= 2;
                _low *= 2;
                if (_reader.ReadBit())
                {
                    _low++;
                }
            }
        }
    }
}