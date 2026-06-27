using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using IctBaden.RevolutionPi.Model;

namespace IctBaden.RevolutionPi.Configuration
{
    public class PiConfiguration
    {
        public string RevPiConfigFileName = "/etc/revpi/config.rsc";

        private JsonObject? _config;

        /// <summary>
        /// Opens the configuration file (RevPiConfigFileName)
        /// </summary>
        /// <returns>True if file could be opened and parsed</returns>
        public bool Open()
        {
            if (IsOpen) return true;
            if (!File.Exists(RevPiConfigFileName)) return false;

            try
            {
                var json = File.ReadAllText(RevPiConfigFileName);
                if (string.IsNullOrWhiteSpace(json))
                {
                    Trace.TraceError($"PiConfiguration.Open failed: {RevPiConfigFileName} is empty.");
                    return false;
                }

                _config = JsonNode.Parse(json) as JsonObject;
                return _config != null;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"PiConfiguration.Open failed: {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// Configuration is loaded.
        /// </summary>
        public bool IsOpen => _config != null;


        private List<DeviceInfo> _devices = new List<DeviceInfo>();

        /// <summary>
        /// List of loaded device informations.
        /// </summary>
        public List<DeviceInfo> Devices
        {
            get
            {
                if (_devices.Count == 0)
                {
                    Open();
                    try
                    {
                        if (_config?["Devices"] is JsonArray devices)
                        {
                            _devices = devices
                                .OfType<JsonObject>()
                                .Select(DeviceInfo.FromJson)
                                .ToList();
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError($"RevolutionPi.Configuration failed to parse devices: {ex.Message}");
                    }
                }
                return _devices;
            }
        }

        /// <summary>
        /// Retrieve information about a configured variable by its name.
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <returns>Variable info for the given variable or null if not found.</returns>
        public VariableInfo? GetVariable(string name)
        {
            return Devices.SelectMany(d => d.Inputs).FirstOrDefault(v => v.Name == name) ??
                   Devices.SelectMany(d => d.Outputs).FirstOrDefault(v => v.Name == name);
        }
    }
}
