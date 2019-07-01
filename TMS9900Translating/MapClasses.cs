using System;

namespace TMS9900Translating
{
    public enum WorkspaceRegister
    {
        R0, R1, R2, R3, R4, R5, R6, R7, R8, R9, R10, R11, R12, R13, R14, R15
    }

    public enum Z80SourceRegister
    {
        A, B, C, D, E, F, H, L, SP, IX, IY
    }

    public struct RegisterMapElement
    {
        public Z80AssemblyParsing.Register Z80Register { get; set; }
        public WorkspaceRegister TMS900Register { get; set; }
    }

    public struct MemoryMapElement
    {
        public ushort Z80Start { get; set; }
        public ushort Z80End => (ushort)(Z80Start + BlockLength - 1);
        public ushort TMS9900Start { get; set; }
        public ushort TMS9000End => (ushort)(TMS9900Start + BlockLength - 1);
        public ushort BlockLength { get; set; }
    }
}
