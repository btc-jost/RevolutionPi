using IctBaden.RevolutionPi;
using IctBaden.RevolutionPi.Configuration;
using NUnit.Framework;

namespace IctBaden.RevolutionPi.Test
{
    [TestFixture]
    public class RevPiLedsTests
    {
        /// <summary>
        /// In-memory <see cref="IPiControl"/> backed by a flat byte buffer, so the
        /// LED read-modify-write bit packing can be verified without hardware.
        /// </summary>
        private sealed class FakePiControl : IPiControl
        {
            private readonly byte[] _buffer = new byte[256];
            public bool ReturnNullOnRead { get; set; }

            public byte this[int index] => _buffer[index];

            public byte[] Read(int offset, int length)
            {
                if (ReturnNullOnRead) return null;
                var data = new byte[length];
                System.Array.Copy(_buffer, offset, data, 0, length);
                return data;
            }

            public int Write(int offset, byte[] data)
            {
                System.Array.Copy(data, 0, _buffer, offset, data.Length);
                return data.Length;
            }
        }

        // No /etc/revpi/config.rsc in the test environment, so RevPiLeds falls back
        // to the default LED address 0x06.
        private const int DefaultLedAddress = 0x06;

        private static RevPiLeds CreateLeds(out FakePiControl fake)
        {
            fake = new FakePiControl();
            return new RevPiLeds(fake, new PiConfiguration());
        }

        [Test]
        public void EachLedOccupiesItsOwnBitsWithoutBleeding()
        {
            var leds = CreateLeds(out var fake);

            leds.SystemLedA1 = LedColor.Green;   // bits 0-1 = 01
            leds.SystemLedA2 = LedColor.Red;     // bits 2-3 = 10
            leds.SystemLedA3 = LedColor.Orange;  // bits 4-5 = 11
            leds.Watchdog = true;                // bit 7

            Assert.That(fake[DefaultLedAddress], Is.EqualTo(0x01 | (0x02 << 2) | (0x03 << 4) | 0x80));

            Assert.That(leds.SystemLedA1, Is.EqualTo(LedColor.Green));
            Assert.That(leds.SystemLedA2, Is.EqualTo(LedColor.Red));
            Assert.That(leds.SystemLedA3, Is.EqualTo(LedColor.Orange));
            Assert.That(leds.Watchdog, Is.True);
        }

        [Test]
        public void ClearingWatchdogLeavesLedColorsIntact()
        {
            var leds = CreateLeds(out _);

            leds.SystemLedA2 = LedColor.Green;
            leds.Watchdog = true;
            leds.Watchdog = false;

            Assert.That(leds.SystemLedA2, Is.EqualTo(LedColor.Green));
            Assert.That(leds.Watchdog, Is.False);
        }

        [Test]
        public void NullReadIsTreatedAsZeroByteWithoutThrowing()
        {
            var leds = CreateLeds(out var fake);
            fake.ReturnNullOnRead = true;

            Assert.That(leds.SystemLedA1, Is.EqualTo(LedColor.Off));
            Assert.That(leds.Watchdog, Is.False);
        }
    }
}
