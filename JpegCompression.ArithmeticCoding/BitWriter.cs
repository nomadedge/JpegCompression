using System;
using System.IO;

namespace JpegCompression.ArithmeticCoding
{
    class BitWriter : IDisposable
    {
        private readonly Stream _stream;

        private uint _bits = 0;
        private int _bitsCount = 0;

        public BitWriter(string fileName)
            : this(File.OpenWrite(fileName))
        { }

        public BitWriter(Stream stream)
        {
            _stream = stream;
        }

        byte DequeueByte()
        {
            byte result = unchecked((byte)(_bits >> (_bitsCount - 8)));

            _bitsCount -= Math.Min(8, _bitsCount);

            return result;
        }

        public void Write(bool bit)
        {
            _bits = _bits << 1 | (bit ? 1U : 0U);

            _bitsCount += 1;

            Flush();
        }

        public void Write(byte value)
        {
            _bits = _bits << 8 | value;

            _bitsCount += 8;

            Flush();
        }

        public void Write(ulong value)
        {
            value += 1;

            int size = 0;
            ulong tmp = value;

            while (tmp >= 1)
            {
                size++;
                tmp /= 2;
            }

            int zeroes = size - 1;

            for (int i = 0; i < zeroes; i++)
            {
                Write(false);
            }

            for (int i = size - 1; i >= 0; i--)
            {
                bool bit = (value & (1UL << i)) != 0;
                Write(bit);
            }
        }

        private void Flush()
        {
            while (_bitsCount >= 8)
            {
                _stream.WriteByte(DequeueByte());
            }
        }

        public void Dispose()
        {
            while (_bitsCount > 0)
            {
                _stream.WriteByte(DequeueByte());
            }

            _stream.Dispose();
        }
    }
}