using System.Collections.Generic;

namespace IctBaden.RevolutionPi.Model
{
    /// <summary>
    /// Human-readable names for the RevPi module/product type ids in <see cref="RevPiModuleTypes"/>.
    /// The driver headers carry no display strings (those come from the piCtory catalog), so the names
    /// are maintained here while the numeric ids are sourced from <see cref="RevPiModuleTypes"/>.
    /// </summary>
    public class RevPiProductNames
    {
        public static readonly Dictionary<int, string> KnownProducts = new()
        {
            // Hardware modules (common_define.h)
            { RevPiModuleTypes.PiCore, "RevPi Core" },
            { RevPiModuleTypes.PiDio14, "RevPi DIO" },
            { RevPiModuleTypes.PiDi16, "RevPi DI" },
            { RevPiModuleTypes.PiDo16, "RevPi DO" },
            { RevPiModuleTypes.PiAio, "RevPi AIO" },
            { RevPiModuleTypes.PiCompact, "RevPi Compact" },
            { RevPiModuleTypes.PiConnect, "RevPi Connect" },
            { RevPiModuleTypes.PiConnectCan, "RevPi Connect CAN" },
            { RevPiModuleTypes.PiConnectMBus, "RevPi Connect MBUS" },
            { RevPiModuleTypes.PiConnectBt, "RevPi Connect BT" },
            { RevPiModuleTypes.PiMio, "RevPi MIO" },
            { RevPiModuleTypes.PiFlat, "RevPi Flat" },
            { RevPiModuleTypes.PiConnect4, "RevPi Connect 4" },
            { RevPiModuleTypes.PiRo, "RevPi RO" },
            { RevPiModuleTypes.PiConnect5, "RevPi Connect 5" },

            // Gateways (common_define.h)
            { RevPiModuleTypes.GatewayCanOpen, "Gateway CANopen" },
            { RevPiModuleTypes.GatewayCcLink, "Gateway CC-Link" },
            { RevPiModuleTypes.GatewayDeviceNet, "Gateway DeviceNet" },
            { RevPiModuleTypes.GatewayEtherCat, "Gateway EtherCAT" },
            { RevPiModuleTypes.GatewayEthernetIp, "Gateway EtherNet/IP" },
            { RevPiModuleTypes.GatewayPowerlink, "Gateway Powerlink" },
            { RevPiModuleTypes.GatewayProfibus, "Gateway Profibus" },
            { RevPiModuleTypes.GatewayProfinetRt, "Gateway Profinet RT" },
            { RevPiModuleTypes.GatewayProfinetIrt, "Gateway Profinet IRT" },
            { RevPiModuleTypes.GatewayCanOpenMaster, "Gateway CANopen Master" },
            { RevPiModuleTypes.GatewaySercos3, "Gateway SercosIII" },
            { RevPiModuleTypes.GatewaySerial, "Gateway Serial" },
            { RevPiModuleTypes.GatewayProfinetSitara, "Gateway Profinet Sitara" },
            { RevPiModuleTypes.GatewayProfinetIrtMaster, "Gateway Profinet IRT Master" },
            { RevPiModuleTypes.GatewayEtherCatMaster, "Gateway EtherCAT Master" },
            { RevPiModuleTypes.GatewayModbusRtu, "Gateway ModbusRTU" },
            { RevPiModuleTypes.GatewayModbusTcp, "Gateway ModbusTCP" },
            { RevPiModuleTypes.GatewayDmx, "Gateway DMX" },

            // Software adapters (piControl.h)
            { RevPiModuleTypes.SwModbusTcpSlave, "ModbusTCP Slave Adapter" },
            { RevPiModuleTypes.SwModbusRtuSlave, "ModbusRTU Slave Adapter" },
            { RevPiModuleTypes.SwModbusTcpMaster, "ModbusTCP Master Adapter" },
            { RevPiModuleTypes.SwModbusRtuMaster, "ModbusRTU Master Adapter" },
            { RevPiModuleTypes.SwProfinetController, "Profinet Controller" },
            { RevPiModuleTypes.SwProfinetDevice, "Profinet Device" },
            { RevPiModuleTypes.SwRevPiSeven, "RevPi Seven Adapter" },
            { RevPiModuleTypes.SwRevPiCloud, "RevPi Cloud Adapter" },
            { RevPiModuleTypes.SwOpcUaServer, "OPC UA Server Adapter" },
            { RevPiModuleTypes.SwMqttClient, "MQTT Client Adapter" }
        };

        /// <summary>
        /// Returns name of the product given its product type.
        /// </summary>
        /// <param name="productType">Product type number</param>
        /// <returns>Name of the product.</returns>
        public static string GetProductName(int productType)
        {
            // A not-connected module reports its base id with bit 0x8000 set; look up the base type.
            var baseType = productType & RevPiModuleTypes.NotConnectedMask;
            if (KnownProducts.TryGetValue(baseType, out var name))
            {
                return name;
            }

            return $"Unknown product type ({productType})";
        }
    }
}
