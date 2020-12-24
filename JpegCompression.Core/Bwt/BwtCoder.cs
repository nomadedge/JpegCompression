using System.Collections.Generic;
using System.Linq;

namespace JpegCompression.Core.Bwt
{
    internal class BwtCoder
    {
        private List<byte> _bytes;

        private int Comparer(int left, int right)
        {
            if (left == right)
            {
                return 0;
            }

            var result = 0;
            var left_ = left;
            var right_ = right;
            do
            {
                result = _bytes[left_].CompareTo(_bytes[right_]);
                left_ = (left_ + 1) % _bytes.Count;
                right_ = (right_ + 1) % _bytes.Count;
                if (left_ == left)
                {
                    return 0;
                }
            }
            while (result == 0);

            return result;
        }

        internal BwtModel Encode(List<byte> bytes)
        {
            _bytes = bytes;

            var initialValue = 0;
            var indexes = Enumerable.Range(0, _bytes.Count).Select(i => initialValue++).ToList();

            indexes.Sort(Comparer);

            var originalIndex = indexes.IndexOf(0);

            var encodedBytes = new List<byte>(_bytes.Count);
            for (int i = 0; i < _bytes.Count; i++)
            {
                encodedBytes.Add(_bytes[(indexes[i] + _bytes.Count - 1) % _bytes.Count]);
            }

            var model = new BwtModel(originalIndex, encodedBytes);
            return model;
        }

        internal List<byte> Decode(BwtModel model)
        {
            var byteLength = 256;

            var counts = Enumerable.Range(0, byteLength).Select(i => 0).ToList();

            foreach (var element in model.Bytes)
            {
                counts[element]++;
            }

            var sum = 0;
            for (int i = 0; i < byteLength; i++)
            {
                sum += counts[i];
                counts[i] = sum - counts[i];
            }

            var invertVector = Enumerable.Range(0, model.Bytes.Count).Select(i => 0).ToList();
            for (int i = 0; i < model.Bytes.Count; i++)
            {
                invertVector[counts[model.Bytes[i]]] = i;
                counts[model.Bytes[i]]++;
            }

            var currentIndex = invertVector[model.OriginalIndex];
            var decodedBytes = new List<byte>(model.Bytes.Count);
            for (int i = 0; i < model.Bytes.Count; i++)
            {
                decodedBytes.Add(model.Bytes[currentIndex]);
                currentIndex = invertVector[currentIndex];
            }

            return decodedBytes;
        }
    }
}
