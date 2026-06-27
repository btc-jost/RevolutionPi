using System.Runtime.InteropServices;

namespace IctBaden.RevolutionPi.Model
{
    /// <summary>
    /// Mirrors the driver's <c>SPIVariable</c> (piControl.h), used by the KB_FIND_VARIABLE ioctl.
    /// Layout: char strVarName[32]; __u16 i16uAddress; __u8 i8uBit; __u8 pad; __u16 i16uLength; (38 bytes)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SpiVariable
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string VarName;             // Variable name (char[32])
        public ushort Address;             // Address of the byte in the process image
        public byte BitOffset;             // 0-7 bit position, >= 8 whole byte
        public byte Pad;                   // driver padding byte to align Length
        public ushort Length;              // length of the variable in bits. Possible values are 1, 8, 16 and 32
    }
}
