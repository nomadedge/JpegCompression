using JpegCompression.ArithmeticCoding;
using JpegCompression.Cli.Enums;
using JpegCompression.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

internal static class Program
{
    internal static void Main(string[] args)
    {
        try
        {
            var mode = args[0] == "e" ? Mode.Encoding : Mode.Decoding;
            var fileFullName = args[1];
            if (!File.Exists(fileFullName))
            {
                throw new FileNotFoundException("File not found.");
            }

            var stopwatch = new Stopwatch();

            Console.WriteLine("Starting the task...");
            stopwatch.Start();

            var resultFileBytes = new List<byte>();
            var resultFileFullName = string.Empty;

            switch (mode)
            {
                case Mode.Encoding:
                    Encode(fileFullName);
                    Console.WriteLine("Encoding has been finished successfully.");
                    break;
                case Mode.Decoding:
                    Decode(fileFullName);
                    Console.WriteLine("Decoding has been finished successfully.");
                    break;
            }

            stopwatch.Stop();
            Console.WriteLine($"It took {stopwatch.Elapsed.TotalSeconds} seconds.");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    private static void Encode(string fileFullName)
    {
        var fileBytes = File.ReadAllBytes(fileFullName).ToList();

        var resultFileFullName = $"{fileFullName}.encoded";
        var encoder = new Encoder(fileBytes);
        var tempFileName = $"{fileFullName}.temp";
        var tempResult = encoder.Encode();
        File.WriteAllBytes(tempFileName, tempResult.ToArray());
        ArithmeticCoding.Encode(tempFileName, resultFileFullName);
        if (new FileInfo(fileFullName).Length <= new FileInfo(resultFileFullName).Length)
        {
            File.WriteAllBytes(resultFileFullName, fileBytes.ToArray());
        }
        File.Delete(tempFileName);
    }

    private static void Decode(string fileFullName)
    {
        var resultFileFullName = $"{fileFullName}.decoded";
        var tempFileName = $"{fileFullName}.temp";
        var fileEncodedBytes = File.ReadAllBytes(fileFullName).ToList();
        var firstTwoBytes = fileEncodedBytes.Take(2).ToArray();
        if (firstTwoBytes[0] == 0xFF && firstTwoBytes[1] == 0xD8)
        {
            File.WriteAllBytes(resultFileFullName, fileEncodedBytes.ToArray());
            return;
        }
        ArithmeticCoding.Decode(fileFullName, tempFileName);
        var fileBytes = File.ReadAllBytes(tempFileName).ToList();
        File.Delete(tempFileName);
        var decoder = new Decoder(fileBytes);
        var result = decoder.Decode();
        File.WriteAllBytes(resultFileFullName, result.ToArray());
    }
}