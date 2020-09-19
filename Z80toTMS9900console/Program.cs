using System;
using System.Collections.Generic;
using System.IO;
using TMS9900Translating;
using TMS9900Translating.Translating;
using Z80AssemblyParsing.Parsing;

namespace Z80toTMS9900console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 4)
                throw new Exception("Required arguments are a source file of z80 code and a destination file of TMS9900 code.");
            var z80AssemblyFile = args[0];
            var tms9900AssemblyFile = args[1];
            var hexPrefix = args.Length > 2 ? args[2] : string.Empty;
            var hexSuffix = args.Length > 3 ? args[3] : "h";

            using (var reader = new StreamReader(z80AssemblyFile))
            using (var writer = new StreamWriter(tms9900AssemblyFile))
            {
                var translator = InstantiateTranslator(hexPrefix, hexSuffix);
                var z80codeLines = GetZ80sourceCode(reader);
                foreach (var tmsCommand in translator.Translate(z80codeLines))
                    writer.WriteLine(tmsCommand.CommandText);
            }
        }

        private static TMS9900MultilineTranslator InstantiateTranslator(string hexPrefix, string hexSuffix)
        {
            var singleLineTranslator = new TMS9900Translator(new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.IX, WorkspaceRegister.R1),
                    (Z80SourceRegister.IY, WorkspaceRegister.R2),
                    (Z80SourceRegister.SP, WorkspaceRegister.R3),
                    (Z80SourceRegister.A, WorkspaceRegister.R4),
                    (Z80SourceRegister.B, WorkspaceRegister.R5),
                    (Z80SourceRegister.C, WorkspaceRegister.R5),
                    (Z80SourceRegister.D, WorkspaceRegister.R7),
                    (Z80SourceRegister.E, WorkspaceRegister.R7),
                    (Z80SourceRegister.H, WorkspaceRegister.R9),
                    (Z80SourceRegister.L, WorkspaceRegister.R9)
                },
                new List<MemoryMapElement>()
                {
                },
                new LabelHighlighter());
            return new TMS9900MultilineTranslator(new Z80LineParser(hexPrefix, hexSuffix), singleLineTranslator);
        }

        static IEnumerable<string> GetZ80sourceCode(StreamReader reader)
        {
            //Return to the beginning of the file as the list of strings shall be enumerated twice.
            reader.BaseStream.Position = 0;
            reader.DiscardBufferedData();

            string oneLine;
            while ((oneLine = reader.ReadLine()) != null)
                yield return oneLine;
        }
    }
}
