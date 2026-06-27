namespace IctBaden.RevolutionPi.Model
{
    /// <summary>
    /// RevPi module / product type ids, mirroring the driver headers
    /// (<c>common_define.h</c> <c>KUNBUS_FW_DESCR_TYP_*</c> for hardware modules and gateways,
    /// <c>piControl.h</c> <c>PICONTROL_SW_*</c> for software adapters). The driver only carries the
    /// numeric ids; the human-readable names live in <see cref="RevPiProductNames"/> (piCtory catalog).
    /// Keep this in sync with the driver sources at https://github.com/RevolutionPi/piControl.
    /// </summary>
    public static class RevPiModuleTypes
    {
        // piControl.h - module id flags
        public const int NotConnectedFlag = 0x8000;   // PICONTROL_NOT_CONNECTED
        public const int NotConnectedMask = 0x7fff;   // PICONTROL_NOT_CONNECTED_MASK
        public const int SoftwareOffset = 0x6001;     // PICONTROL_SW_OFFSET

        // common_define.h - KUNBUS_FW_DESCR_TYP_* (hardware modules & gateways)
        public const int GatewayCanOpen = 71;             // MG_CAN_OPEN
        public const int GatewayCcLink = 72;              // MG_CCLINK
        public const int GatewayDeviceNet = 73;           // MG_DEV_NET
        public const int GatewayEtherCat = 74;            // MG_ETHERCAT
        public const int GatewayEthernetIp = 75;          // MG_ETHERNET_IP
        public const int GatewayPowerlink = 76;           // MG_POWERLINK
        public const int GatewayProfibus = 77;            // MG_PROFIBUS
        public const int GatewayProfinetRt = 78;          // MG_PROFINET_RT
        public const int GatewayProfinetIrt = 79;         // MG_PROFINET_IRT
        public const int GatewayCanOpenMaster = 80;       // MG_CAN_OPEN_MASTER
        public const int GatewaySercos3 = 81;             // MG_SERCOS3
        public const int GatewaySerial = 82;              // MG_SERIAL
        public const int GatewayProfinetSitara = 83;      // MG_PROFINET_SITARA
        public const int GatewayProfinetIrtMaster = 84;   // MG_PROFINET_IRT_MASTER
        public const int GatewayEtherCatMaster = 85;      // MG_ETHERCAT_MASTER
        public const int GatewayModbusRtu = 92;           // MG_MODBUS_RTU
        public const int GatewayModbusTcp = 93;           // MG_MODBUS_TCP
        public const int PiCore = 95;                     // PI_CORE
        public const int PiDio14 = 96;                    // PI_DIO_14
        public const int PiDi16 = 97;                     // PI_DI_16
        public const int PiDo16 = 98;                     // PI_DO_16
        public const int GatewayDmx = 100;                // MG_DMX
        public const int PiAio = 103;                     // PI_AIO
        public const int PiCompact = 104;                 // PI_COMPACT
        public const int PiConnect = 105;                 // PI_CONNECT
        public const int PiConnectCan = 109;              // PI_CON_CAN
        public const int PiConnectMBus = 110;             // PI_CON_MBUS
        public const int PiConnectBt = 111;               // PI_CON_BT
        public const int PiMio = 118;                     // PI_MIO
        public const int PiFlat = 135;                    // PI_FLAT
        public const int PiConnect4 = 136;                // PI_CONNECT_4
        public const int PiRo = 137;                      // PI_RO
        public const int PiConnect5 = 138;                // PI_CONNECT_5
        public const int GenericProfibus = 0xfffe;        // PI_REVPI_GENERIC_PB
        public const int Undefined = 0xffff;              // INTERN / UNDEFINED

        // piControl.h - PICONTROL_SW_* (software adapters)
        public const int SwModbusTcpSlave = 0x6001;       // SW_MODBUS_TCP_SLAVE
        public const int SwModbusRtuSlave = 0x6002;       // SW_MODBUS_RTU_SLAVE
        public const int SwModbusTcpMaster = 0x6003;      // SW_MODBUS_TCP_MASTER
        public const int SwModbusRtuMaster = 0x6004;      // SW_MODBUS_RTU_MASTER
        public const int SwProfinetController = 0x6005;   // SW_PROFINET_CONTROLLER
        public const int SwProfinetDevice = 0x6006;       // SW_PROFINET_DEVICE
        public const int SwRevPiSeven = 0x6007;           // SW_REVPI_SEVEN
        public const int SwRevPiCloud = 0x6008;           // SW_REVPI_CLOUD
        public const int SwOpcUaServer = 0x6009;          // SW_OPCUA_REVPI_SERVER
        public const int SwMqttClient = 0x600a;           // SW_MQTT_REVPI_CLIENT
    }
}
