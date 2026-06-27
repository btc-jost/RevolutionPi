using System.Diagnostics;
using System.Text.Json.Nodes;

namespace IctBaden.RevolutionPi.Model
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class VariableInfo
    {
        public VariableType Type { get; private set; }
        public int Index { get; private set; }
        public string Name { get; set; }
        public object? DefaultValue { get; set; }
        public byte BitOffset { get; set; }              // 0-7 bit position, >= 8 whole byte
        public ushort Length { get; set; }               // length of the variable in bits. Possible values are 1, 8, 16 and 32
        public ushort Address { get; set; }              // Address of the byte in the process image
        public bool Export { get; set; }
        //  "0000",
        public string? Unknown { get; set; }              //"0001"
        public string? Comment { get; set; }

        public DeviceInfo? Device { get; set; }

        public string LengthText
        {
            get
            {
                switch (Length)
                {
                    case 1: return "BIT";
                    case 8: return "BYTE";
                    case 16: return "WORD";
                    case 32: return "DWORD";
                }
                return $"[{Length}bits]";
            }
        }

        /// <summary>
        /// Builds a variable from a config.rsc entry: an 8-element JSON array
        /// [name, default, length, address, export, unknown, comment, bitOffset].
        /// </summary>
        public VariableInfo(DeviceInfo device, VariableType type, int index, JsonArray json)
        {
            Device = device;
            Type = type;
            Index = index;
            Name = JsonScalar.GetString(json[0]) ?? string.Empty;
            DefaultValue = JsonScalar.GetScalar(json[1]);
            Length = JsonScalar.GetUInt16(json[2]);
            Address = JsonScalar.GetUInt16(json[3]);
            Export = JsonScalar.GetBool(json[4]);
            Unknown = JsonScalar.GetString(json[5]);
            Comment = JsonScalar.GetString(json[6]);
            BitOffset = json.Count > 7 ? JsonScalar.GetByte(json[7]) : (byte)0;
        }
    }
}
