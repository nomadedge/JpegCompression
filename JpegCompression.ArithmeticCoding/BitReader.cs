using System;
using System.IO;

namespace JpegCompression.ArithmeticCoding
{
    class BitReader : IDisposable
    {
        private readonly Stream _stream;

        private byte _bits;
        private int _bitsRead = 8;

        public BitReader(Stream stream)
        {
            _stream = stream;
        }

        public BitReader(string fileName)
        {
            _stream = new FileStream(fileName, FileMode.Open);
        }

        public bool ReadBit()
        {
            if (_bitsRead == 8)
            {
                int read = _stream.ReadByte();
                if (read == -1)
                {
                    return false;
                }

                _bits = (byte)read;
                _bitsRead = 0;
            }

            bool result = (_bits & 1 << (7 - _bitsRead)) != 0;
            _bitsRead++;

            return result;
        }

        public ulong ReadUInt64()
        {
            int size = 0;

            while (ReadBit() == false)
            {
                size++;
            }

            ulong result = 1;

            for (int i = 0; i < size; i++)
            {
                result = (result << 1) + (ReadBit() ? 1UL : 0UL);
            }

            return result - 1;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}