namespace IctBaden.RevolutionPi.Model
{
    /// <summary>
    /// Bit masks of the system-LED process-image field, mirroring <c>picontrol_intern.h</c>.
    /// <para>
    /// Classic single-byte <c>leds</c> layout (RevPi Core / Core S / SE / Connect, and Flat for A1-A3):
    /// A1=bits0-1, A2=bits2-3, A3=bits4-5. <b>RevPi Core S/SE only wire A1 and A2</b> (no A3, no watchdog).
    /// Bit 6/7 are <c>X2_DOUT</c> / <c>WD_TRIGGER</c> on RevPi Connect; on RevPi Flat bits 6-9 carry A4/A5.
    /// </para>
    /// <para>
    /// RevPi Connect 4/5 do not use this byte for status LEDs - they have a separate 16-bit
    /// <c>rgb_leds</c> field (3 bits per LED); see the <c>Rgb*</c> masks.
    /// </para>
    /// </summary>
    public static class RevPiLedBits
    {
        // Classic single-byte 'leds' field
        public const int A1Green = 0x0001;
        public const int A1Red = 0x0002;
        public const int A2Green = 0x0004;
        public const int A2Red = 0x0008;
        public const int A3Green = 0x0010;          // RevPi Connect / Flat
        public const int A3Red = 0x0020;            // RevPi Connect / Flat
        public const int X2DigitalOut = 0x0040;     // RevPi Connect (PICONTROL_X2_DOUT)
        public const int WatchdogTrigger = 0x0080;  // RevPi Connect (PICONTROL_WD_TRIGGER)
        // RevPi Flat reuses bits 6-7 for A4 (green/red) and bits 8-9 for A5 (green/red).

        // RevPi Connect 4/5 - separate 16-bit 'rgb_leds' field, 3 bits per LED
        public const int RgbA1Red = 0x0001;
        public const int RgbA1Green = 0x0002;
        public const int RgbA1Blue = 0x0004;
        public const int RgbA2Red = 0x0008;
        public const int RgbA2Green = 0x0010;
        public const int RgbA2Blue = 0x0020;
        public const int RgbA3Red = 0x0040;
        public const int RgbA3Green = 0x0080;
        public const int RgbA3Blue = 0x0100;
        public const int RgbA4Red = 0x0200;
        public const int RgbA4Green = 0x0400;
        public const int RgbA4Blue = 0x0800;
        public const int RgbA5Red = 0x1000;
        public const int RgbA5Green = 0x2000;
        public const int RgbA5Blue = 0x4000;
    }

    /// <summary>
    /// Default byte offsets of the RevPi Core-class process image (<c>revpi_core.h</c>
    /// <c>SRevPiProcessImage</c>). <b>Reference only</b> - actual addresses are read from
    /// <c>config.rsc</c> (e.g. the LED byte via the <c>RevPiLED</c> variable). The <c>rgb_leds</c>
    /// field is present only on RGB models.
    /// </summary>
    public static class RevPiCoreImageOffsets
    {
        public const int Status = 0;
        public const int IoCycle = 1;
        public const int RS485ErrorCount = 2;       // u16
        public const int CpuTemperature = 4;
        public const int CpuFrequency = 5;
        public const int Leds = 6;                  // union { outputs; leds }
        public const int RS485ErrorLimit1 = 7;      // u16
        public const int RS485ErrorLimit2 = 9;      // u16
        public const int RgbLeds = 11;              // u16, RGB models only
    }
}
