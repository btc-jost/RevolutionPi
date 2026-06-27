using IctBaden.RevolutionPi.Configuration;
using System;
using System.Diagnostics;

namespace IctBaden.RevolutionPi
{
    /// <summary>
    /// Setting and querying the system LEDs A1, A2, A3 and Watchdog.
    /// </summary>
    public class RevPiLeds
    {
        private readonly PiControl _control;
        private readonly int _ledAddress;

        public RevPiLeds(PiControl control, PiConfiguration config)
        {
            _control = control ?? throw new ArgumentException("RevPiLeds cannot be used without PiControl");

            var info = config.GetVariable("RevPiLED");
            _ledAddress = info?.Address ?? 0x06;
            Trace.TraceInformation($"RevPiLeds: Using address 0x{_ledAddress:X2}");
        }

        private byte LedByte
        {
            get => _control.Read(_ledAddress, 1)[0];
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
        /// Current color of system LED A3
        /// </summary>
        public LedColor SystemLedA3
        {
            get => (LedColor)((LedByte & 0x30) >> 4);
            set => LedByte = (byte)((LedByte & ~0x30) | ((byte)value << 4));
        }

        /// <summary>
        /// Watchdog
        /// </summary>
        public bool Watchdog
        {
            get => (LedByte & 0x80) == 0x80;
            set => LedByte = (byte)((LedByte & ~0x80) | (value ? 0x80 : 0x00));
        }
    }
}