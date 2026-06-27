using System;
using System.Diagnostics;
using IctBaden.RevolutionPi.Configuration;

namespace IctBaden.RevolutionPi
{
    /// <summary>
    /// Setting and querying the system LEDs A1/A2/A3 and the watchdog trigger in the single-byte
    /// <c>leds</c> field (see <see cref="Model.RevPiLedBits"/>). The byte address comes from the
    /// <c>RevPiLED</c> variable in <c>config.rsc</c> (default process-image offset 0x06).
    /// <para>
    /// Layout applies to RevPi Core/Core S/SE and Connect. <b>On Core S/SE only A1 and A2 exist</b>
    /// (no A3 LED, no watchdog) - <see cref="SystemLedA3"/>/<see cref="Watchdog"/> write unused bits
    /// there. RevPi Connect 4/5 drive their RGB LEDs through a separate field and are not supported here.
    /// </para>
    /// </summary>
    public class RevPiLeds
    {
        private readonly IPiControl _control;
        private readonly int _ledAddress;

        public RevPiLeds(IPiControl control, PiConfiguration config)
        {
            _control = control ?? throw new ArgumentException("RevPiLeds cannot be used without PiControl");

            var info = config.GetVariable("RevPiLED");
            _ledAddress = info?.Address ?? 0x06;
            Trace.TraceInformation($"RevPiLeds: Using address 0x{_ledAddress:X2}");
        }

        private byte LedByte
        {
            get
            {
                var data = _control.Read(_ledAddress, 1);
                return data?[0] ?? 0;
            }
            set => _control.Write(_ledAddress, new[] { value });
        }

        /// <summary>
        /// Current color of system LED A1
        /// </summary>
        public LedColor SystemLedA1
        {
            get => (LedColor)(LedByte & 0x03);
            set
            {
                var oldValue = LedByte;
                LedByte = (byte)((oldValue & ~0x03) | (byte)value);
            }
        }

        /// <summary>
        /// Current color of system LED A2
        /// </summary>
        public LedColor SystemLedA2
        {
            get => (LedColor)((LedByte & 0x0C) >> 2);
            set => LedByte = (byte)((LedByte & ~0x0C) | ((byte)value << 2));
        }

        /// <summary>
        /// Current color of system LED A3 (RevPi Connect / Flat; not present on Core S/SE).
        /// </summary>
        public LedColor SystemLedA3
        {
            get => (LedColor)((LedByte & 0x30) >> 4);
            set => LedByte = (byte)((LedByte & ~0x30) | ((byte)value << 4));
        }

        /// <summary>
        /// Watchdog trigger bit (RevPi Connect, <c>PICONTROL_WD_TRIGGER</c>; not present on Core S/SE).
        /// </summary>
        public bool Watchdog
        {
            get => (LedByte & 0x80) == 0x80;
            set => LedByte = (byte)((LedByte & ~0x80) | (value ? 0x80 : 0x00));
        }
    }
}
