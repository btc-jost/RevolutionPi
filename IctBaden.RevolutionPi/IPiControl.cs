namespace IctBaden.RevolutionPi
{
    /// <summary>
    /// Process-image access surface used by higher-level helpers (e.g. <see cref="RevPiLeds"/>).
    /// Extracted as a seam so the read-modify-write logic can be unit-tested with a fake buffer.
    /// </summary>
    public interface IPiControl
    {
        /// <summary>
        /// Read data from the process image.
        /// </summary>
        /// <param name="offset">Position to read from</param>
        /// <param name="length">Byte count to read</param>
        /// <returns>Data read or null in case of failure</returns>
        byte[]? Read(int offset, int length);

        /// <summary>
        /// Write data to the process image.
        /// </summary>
        /// <param name="offset">Position to write to</param>
        /// <param name="data">Data to be written</param>
        /// <returns>Bytes written</returns>
        int Write(int offset, byte[] data);
    }
}
