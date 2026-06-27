using System;

namespace IctBaden.RevolutionPi
{
    /// <summary>
    /// Bit flags of the system status byte 'RevPiStatus' (process-image offset 0),
    /// mirroring the driver's <c>PICONTROL_STATUS_*</c> (picontrol_intern.h).
    /// </summary>
    [Flags]
    public enum RevPiStatus
    {
        Running = 0x01,
        ExtraModule = 0x02,
        MissingModule = 0x04,
        SizeMismatch = 0x08,
        LeftGateway = 0x10,
        RightGateway = 0x20,
        X2DigitalIn = 0x40   // RevPi Connect only (PICONTROL_STATUS_X2_DIN)
    }
}
