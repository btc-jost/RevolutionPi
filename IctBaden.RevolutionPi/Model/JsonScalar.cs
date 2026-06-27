using System;
using System.Globalization;
using System.Text.Json.Nodes;

namespace IctBaden.RevolutionPi.Model
{
    /// <summary>
    /// Helpers to read scalar values from the RevPi config (config.rsc), which encodes
    /// some numbers as JSON strings (e.g. "productType": "95", lengths/addresses as "8").
    /// Newtonsoft coerced these implicitly; System.Text.Json does not, so we do it here.
    /// </summary>
    internal static class JsonScalar
    {
        public static string? GetString(JsonNode? node)
        {
            if (node is JsonValue value && value.TryGetValue<string>(out var s)) return s;
            return node?.ToString();
        }

        public static int GetInt(JsonNode? node)
        {
            if (node is not JsonValue value) return 0;
            if (value.TryGetValue<int>(out var i)) return i;
            if (value.TryGetValue<string>(out var s) &&
                int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }
            return 0;
        }

        public static ushort GetUInt16(JsonNode? node) => (ushort)GetInt(node);

        public static byte GetByte(JsonNode? node)
        {
            if (node is not JsonValue value) return 0;
            if (value.TryGetValue<int>(out var i)) return (byte)i;
            if (value.TryGetValue<string>(out var s) &&
                byte.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }
            return 0;
        }

        public static bool GetBool(JsonNode? node) =>
            node is JsonValue value && value.TryGetValue<bool>(out var b) && b;

        public static Guid GetGuid(JsonNode? node)
        {
            var s = node is JsonValue value && value.TryGetValue<string>(out var g) ? g : null;
            return Guid.TryParse(s, out var guid) ? guid : Guid.Empty;
        }

        /// <summary>
        /// Returns the underlying CLR scalar (string/bool/long/double) or null.
        /// </summary>
        public static object? GetScalar(JsonNode? node)
        {
            if (node is not JsonValue value) return null;
            if (value.TryGetValue<string>(out var s)) return s;
            if (value.TryGetValue<bool>(out var b)) return b;
            if (value.TryGetValue<long>(out var l)) return l;
            if (value.TryGetValue<double>(out var d)) return d;
            return value.ToString();
        }
    }
}
