using NUnit.Framework;

namespace IctBaden.RevolutionPi.Test
{
    [TestFixture]
    public class ConvertDataToValueTests
    {
        private readonly PiControl _control = new PiControl();

        [Test]
        public void SingleByteIsReturnedAsByte()
        {
            var value = _control.ConvertDataToValue(new byte[] { 0x42 });
            Assert.That(value, Is.EqualTo((byte)0x42));
        }

        [Test]
        public void TwoBytesAreReturnedAsLittleEndianUshort()
        {
            var value = _control.ConvertDataToValue(new byte[] { 0x34, 0x12 });
            Assert.That(value, Is.EqualTo((ushort)0x1234));
        }

        [Test]
        public void FourBytesAreReturnedAsLittleEndianNumber()
        {
            // Regression for the case 3 -> case 4 fix: 4-byte values must be
            // decoded numerically, not fall through to the ASCII-string default.
            var value = _control.ConvertDataToValue(new byte[] { 0x01, 0x02, 0x03, 0x04 });

            Assert.That(value, Is.Not.InstanceOf<string>());
            Assert.That(value, Is.EqualTo(0x04030201UL));
        }

        [Test]
        public void OtherLengthsFallBackToAsciiString()
        {
            var value = _control.ConvertDataToValue(new byte[] { (byte)'1', (byte)'.', (byte)'2' });
            Assert.That(value, Is.EqualTo("1.2"));
        }
    }
}
