using System.Runtime.InteropServices;
using IctBaden.RevolutionPi.Model;
using NUnit.Framework;

namespace IctBaden.RevolutionPi.Test
{
    /// <summary>
    /// Locks the marshaled sizes of the driver-facing structs against the
    /// piControl.h ABI (SPIValue = 4 bytes, SPIVariable = 38 bytes).
    /// </summary>
    [TestFixture]
    public class StructLayoutTests
    {
        [Test]
        public void SpiValueMatchesDriverAbi()
        {
            Assert.That(Marshal.SizeOf<SpiValue>(), Is.EqualTo(4));
        }

        [Test]
        public void SpiVariableMatchesDriverAbi()
        {
            // char[32] + __u16 + __u8 + __u8 pad + __u16 = 38 bytes
            Assert.That(Marshal.SizeOf<SpiVariable>(), Is.EqualTo(38));
        }
    }
}
