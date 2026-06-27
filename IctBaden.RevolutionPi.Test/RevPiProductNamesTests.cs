using IctBaden.RevolutionPi.Model;
using NUnit.Framework;

namespace IctBaden.RevolutionPi.Test
{
    [TestFixture]
    public class RevPiProductNamesTests
    {
        [TestCase(95, "RevPi Core")]
        [TestCase(136, "RevPi Connect 4")]
        [TestCase(0x6001, "ModbusTCP Slave Adapter")]
        [TestCase(92, "Gateway ModbusRTU")]      // common_define.h MG_MODBUS_RTU
        [TestCase(93, "Gateway ModbusTCP")]      // common_define.h MG_MODBUS_TCP
        [TestCase(0x600a, "MQTT Client Adapter")] // piControl.h SW_MQTT_REVPI_CLIENT
        public void KnownProductTypesResolveToName(int productType, string expected)
        {
            Assert.That(RevPiProductNames.GetProductName(productType), Is.EqualTo(expected));
        }

        [Test]
        public void NotConnectedIdResolvesToBaseName()
        {
            // base id with the 0x8000 not-connected flag still resolves via masking
            Assert.That(RevPiProductNames.GetProductName(96 | RevPiModuleTypes.NotConnectedFlag),
                Is.EqualTo("RevPi DIO"));
        }

        [Test]
        public void UnknownProductTypeFallsBackToPlaceholder()
        {
            Assert.That(RevPiProductNames.GetProductName(99999), Is.EqualTo("Unknown product type (99999)"));
        }
    }
}
