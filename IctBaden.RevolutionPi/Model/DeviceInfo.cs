using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Nodes;

namespace IctBaden.RevolutionPi.Model
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class DeviceInfo
    {
        public string? CatalogNr { get; set; }
        public Guid Guid { get; set; }
        public string? Id { get; set; }
        public string? Type { get; set; }
        public int ProductType { get; set; }
        public int Position { get; set; }
        public string? Name { get; set; }
        public string? Bmk { get; set; }
        public int InpVariant { get; set; }
        public int OutVariant { get; set; }
        public string? Comment { get; set; }
        public ushort Offset { get; set; }

        public VariableInfo[] Inputs { get; private set; } = Array.Empty<VariableInfo>();
        public VariableInfo[] Outputs { get; private set; } = Array.Empty<VariableInfo>();
        public VariableInfo[] Mems { get; private set; } = Array.Empty<VariableInfo>();
        public VariableInfo[] Extends { get; private set; } = Array.Empty<VariableInfo>();

        public IEnumerable<VariableInfo> Variables =>
            Inputs.Concat(Outputs).Concat(Mems).Concat(Extends);

        /// <summary>
        /// Builds a device (incl. its input/output/mem/extend variables) from a
        /// config.rsc "Devices" entry.
        /// </summary>
        public static DeviceInfo FromJson(JsonObject node)
        {
            var device = new DeviceInfo
            {
                CatalogNr = JsonScalar.GetString(node["catalogNr"]),
                Guid = JsonScalar.GetGuid(node["GUID"]),
                Id = JsonScalar.GetString(node["id"]),
                Type = JsonScalar.GetString(node["type"]),
                ProductType = JsonScalar.GetInt(node["productType"]),
                Position = JsonScalar.GetInt(node["position"]),
                Name = JsonScalar.GetString(node["name"]),
                Bmk = JsonScalar.GetString(node["bmk"]),
                InpVariant = JsonScalar.GetInt(node["inpVariant"]),
                OutVariant = JsonScalar.GetInt(node["outVariant"]),
                Comment = JsonScalar.GetString(node["comment"]),
                Offset = JsonScalar.GetUInt16(node["offset"])
            };

            device.Inputs = device.ParseVariables(node["inp"] as JsonObject, VariableType.Input);
            device.Outputs = device.ParseVariables(node["out"] as JsonObject, VariableType.Output);
            device.Mems = device.ParseVariables(node["mem"] as JsonObject, VariableType.Memory);
            device.Extends = device.ParseVariables(node["extend"] as JsonObject, VariableType.Extend);
            return device;
        }

        private VariableInfo[] ParseVariables(JsonObject? obj, VariableType type)
        {
            if (obj == null) return Array.Empty<VariableInfo>();

            var variables = new List<VariableInfo>();
            foreach (var entry in obj)
            {
                if (entry.Value is JsonArray fields && int.TryParse(entry.Key, out var index))
                {
                    variables.Add(new VariableInfo(this, type, index, fields));
                }
            }
            return variables.ToArray();
        }
    }
}
