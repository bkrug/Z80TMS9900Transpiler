using JumpTracking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Z80AssemblyParsing;
using Z80AssemblyParsing.Parsing;

namespace JumpTrackingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
                throw new Exception("Required arguments are a source file of z80 code and a destination file of TMS9900 code.");
            var z80AssemblyInputFile = args[0];
            var z80AssemblyOutputFile = args[1];
            var entryLabels = args[2].Split(';').ToList();
            using (var reader = new StreamReader(z80AssemblyInputFile))
            using (var writer = new StreamWriter(z80AssemblyOutputFile))
            {
                var parsedLines = GetZ80sourceCode(reader);
                var jumpTracker = new JumpTracker(entryLabels);
                var actualCommands = jumpTracker.FindJumps(parsedLines);
                foreach (var command in actualCommands)
                    writer.WriteLine(command.SourceText);
            }
        }

        static IEnumerable<Command> GetZ80sourceCode(StreamReader reader)
        {
            //Return to the beginning of the file as the list of strings shall be enumerated twice.
            reader.BaseStream.Position = 0;
            reader.DiscardBufferedData();

            var parser = new Z80LineParser();
            string oneLine;
            while ((oneLine = reader.ReadLine()) != null)
                yield return parser.ParseLine(oneLine);
        }
    }
}
