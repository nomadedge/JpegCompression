using System;
using System.IO;

namespace JpegCompression.ArithmeticCoding
{
    class Encoder
    {
        private static readonly int BitsUsed = 64;
        private static readonly ulong Half = 1UL << (BitsUsed - 1);
        private static readonly ulong Quarter = 1UL << (BitsUsed - 2);

        private BitWriter _writer;

        private ulong _low;
        private ulong _range;
        private ulong _counter;
        private ulong _length;
        private ulong[] _byteCounts;

        public void Encode(Stream reader, BitWriter writer)
        {
            if (!reader.CanRead || !reader.CanSeek)
            {
                throw new InvalidOperationException();
            }

            _writer = writer;

            Encode(reader);
        }

        private void Encode(Stream reader)
        {
            _byteCounts = new ulong[256];

            int b;
            _length = 0;

            while ((b = reader.ReadByte()) != -1)
            {
                _byteCounts[b]++;
                _length++;
            }

            _writer.Write(_length);

            for (int i = 0; i < _byteCounts.Length; i++)
            {
                _writer.Write(_byteCounts[i]);

                if (i != 0)
                {
                    _byteCounts[i] += _byteCounts[i - 1];
                }
            }

            _low = 0;
            _range = Half;
            _counter = 0;

            reader.Position = 0;

            while ((b = reader.ReadByte()) != -1)
            {
                EncodeByte((byte)b);
            }

            for (int i = BitsUsed - 1; i >= 0; i--)
            {
                bool bit = ((1UL << i) & _low) != 0;
                OutputBit(bit);
            }
        }

        private void EncodeByte(byte b)
        {
            ulong rangeUnit = _range / _length;

            ulong byteCumulatedCount = _byteCounts[b];
            ulong previousByteCumultedCount = b == 0 ? 0 : _byteCounts[b - 1];

            _low += rangeUnit * previousByteCumultedCount;

            _range = byteCumulatedCount != _length
                ? rangeUnit * (byteCumulatedCount - previousByteCumultedCount)
                : _range - rangeUnit * previousByteCumultedCount;

            while (_range <= Quarter)
            {
                if (_low <= Half && _range <= Half && _low + _range <= Half)
                    OutputBit(false);
                else if (_low >= Half)
                {
                    OutputBit(true);
                    _low -= Half;
                }
                else
                {
                    _counter++;
                    _low -= Quarter;
                }

                _low *= 2;
                _range *= 2;
            }
        }

        private void OutputBit(bool bit)
        {
            _writer.Write(bit);

            while (_counter > 0)
            {
                _writer.Write(!bit);
                _counter--;
            }
        }
    }
}