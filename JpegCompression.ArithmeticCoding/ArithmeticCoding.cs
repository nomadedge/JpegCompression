using System.IO;

namespace JpegCompression.ArithmeticCoding
{
    public static class ArithmeticCoding
    {
        public static void Encode(string inputFile, string outputFile)
        {
            var encoder = new Encoder();

            using (var reader = File.OpenRead(inputFile))
            using (var writer = new BitWriter(outputFile))
            {
                encoder.Encode(reader, writer);
            }
        }

        public static void Decode(string inputFile, string outputFile)
        {
            var decoder = new Decoder();

            using (var reader = new BitReader(inputFile))
            using (var writer = new FileStream(outputFile, FileMode.Create))
            {
                decoder.Decode(reader, writer);
            }
        }
    }
}