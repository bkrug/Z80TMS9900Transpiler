using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ByteInserter
{
    class Program
    {
        private static byte[] _buffer = new byte[1024];
        private static int _bufferSize = 1024;

        static void Main(string[] args)
        {
            if (args.Length != 4)
                throw new Exception("Required arguments are assembly-source-file rom-source-file rom-offset-in-memory assembly-output-file.");
            var z80AssemblyInputFile = args[0];
            var romInputFile = args[1];
            var fileOffset = int.Parse(args[2]);
            var z80AssemblyOutputFile = args[3];
            using (var reader = new StreamReader(z80AssemblyInputFile))
            using (var romReader = new BinaryReader(new FileStream(romInputFile, FileMode.Open)))
            using (var writer = new StreamWriter(z80AssemblyOutputFile))
            {
                string textLine;
                while ((textLine = reader.ReadLine()) != null)
                {
                    var parts = textLine.Trim().Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToList();
                    if (parts.Any() && parts[0].Equals("getb", StringComparison.OrdinalIgnoreCase))
                    {
                        var addresses = parts.Count > 1 ? parts[1].Split(':').ToList() : null;
                        if (addresses == null || addresses.Count != 2)
                            writer.WriteLine("Wrong format, need two colon-separated addresses:  " + textLine);
                        else if (!TryParseOneAddress(addresses[0], out int startAddress) || !TryParseOneAddress(addresses[1], out int endAddress))
                            writer.WriteLine("Wrong format, addresses don't look like numbers:  " + textLine);
                        else
                            WriteOutputAsDataBytes(romReader, writer, startAddress, endAddress, fileOffset);
                    }
                    else
                        writer.WriteLine(textLine);
                }
            }
        }

        private static bool TryParseOneAddress(string address, out int startAddress)
        {
            if (address.StartsWith("0x") || address.EndsWith("h"))
                return int.TryParse(address.TrimEnd('h'), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out startAddress);
            else
                return int.TryParse(address, out startAddress);
        }

        private static void WriteOutputAsDataBytes(BinaryReader romReader, StreamWriter writer, int startAddress, int endAddress, int fileOffset)
        {
            var labelAddress = startAddress;
            foreach (var thirtyTwoBytes in GetBytes(romReader, startAddress, endAddress, fileOffset).InSetsOf(32))
            {
                var asciiVersion = new string(thirtyTwoBytes.Select(b => b < 0x20 || b >= 0x7F && b <= 0x9F ? '.' : (char)b).ToArray());
                writer.WriteLine("; as text: " + asciiVersion);
                foreach (var foundByte in thirtyTwoBytes)
                {
                    var partOfLabel = labelAddress.ToString("X4").ToLower();
                    var byteAsHex = foundByte.ToString("X2");
                    writer.WriteLine($"l{partOfLabel}:  db      {byteAsHex}h");
                    ++labelAddress;
                }
            }
        }

        static IEnumerable<byte> GetBytes(BinaryReader romReader, int startAddress, int endAddress, int fileOffset)
        {
            startAddress -= fileOffset;
            endAddress -= fileOffset;
            var lengthToRead = endAddress - startAddress;
            romReader.BaseStream.Position = startAddress;
            while (lengthToRead > 0) {
                var batchSize = lengthToRead > _bufferSize ? _bufferSize : lengthToRead;
                var bytesRead = romReader.Read(_buffer, 0, batchSize);
                lengthToRead -= bytesRead;
                foreach (var buf in _buffer.Take(batchSize))
                    yield return buf;
            }
        }
    }
}
