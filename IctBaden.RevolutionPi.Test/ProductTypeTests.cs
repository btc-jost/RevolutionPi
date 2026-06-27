using IctBaden.RevolutionPi.Model;
using NUnit.Framework;

namespace IctBaden.RevolutionPi.Test
{
    [TestFixture]
    public class ProductTypeTests
    {
        [Test]
        public void ConnectedHardwareModule()
        {
            var type = new ProductType(96); // RevPi DIO

            Assert.That(type.IsConnected, Is.True);
            Assert.That(type.IsSoftwareAdapter, Is.False);
            Assert.That(type.Name, Is.EqualTo("RevPi DIO"));
        }

        [Test]
        public void NotConnectedModuleKeepsBaseNameWithSuffix()
        {
            // configured but not connected -> base id | 0x8000
            var type = new ProductType(96 | RevPiModuleTypes.NotConnectedFlag);

            Assert.That(type.IsConnected, Is.False);
            Assert.That(type.Name, Is.EqualTo("RevPi DIO (not connected)"));
        }

        [Test]
        public void SoftwareAdapterIsRecognised()
        {
            var type = new ProductType(0x6001); // ModbusTCP Slave Adapter

            Assert.That(type.IsSoftwareAdapter, Is.True);
            Assert.That(type.IsConnected, Is.True);
            Assert.That(type.Name, Is.EqualTo("ModbusTCP Slave Adapter"));
        }

        [Test]
        public void ImplicitConversionFromIntPreservesValue()
        {
            ProductType type = 95;
            Assert.That(type.Value, Is.EqualTo(95));
            Assert.That(type, Is.EqualTo(new ProductType(95)));
        }

        [Test]
        public void EqualityOperatorsAndToString()
        {
            var core = new ProductType(95);
            var sameCore = new ProductType(95);
            var dio = new ProductType(96);

            Assert.That(core == sameCore, Is.True);
            Assert.That(core != dio, Is.True);
            Assert.That(core.Equals((object)sameCore), Is.True);
            Assert.That(core.GetHashCode(), Is.EqualTo(sameCore.GetHashCode()));
            Assert.That(core.ToString(), Is.EqualTo("RevPi Core"));
        }
    }
}
