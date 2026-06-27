using System;

namespace IctBaden.RevolutionPi.Model
{
    /// <summary>
    /// A RevPi module/product type id with the semantics defined in piControl.h:
    /// the high bit (<c>0x8000</c>) flags a module that is configured but not connected,
    /// and ids at/above <c>0x6001</c> are software adapters (e.g. Modbus/Profinet).
    /// </summary>
    public readonly struct ProductType : IEquatable<ProductType>
    {
        /// <summary>Raw product type id as reported in the configuration.</summary>
        public int Value { get; }

        public ProductType(int value)
        {
            Value = value;
        }

        /// <summary>The module is physically connected (the not-connected flag is clear).</summary>
        public bool IsConnected => (Value & RevPiModuleTypes.NotConnectedFlag) == 0;

        /// <summary>The type is a software adapter rather than a hardware module.</summary>
        public bool IsSoftwareAdapter => (Value & RevPiModuleTypes.NotConnectedMask) >= RevPiModuleTypes.SoftwareOffset;

        /// <summary>Human-readable name; disconnected modules get a "(not connected)" suffix.</summary>
        public string Name => IsConnected
            ? RevPiProductNames.GetProductName(Value)
            : $"{RevPiProductNames.GetProductName(Value)} (not connected)";

        public static implicit operator ProductType(int value) => new ProductType(value);

        public override string ToString() => Name;

        public bool Equals(ProductType other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is ProductType other && Equals(other);
        public override int GetHashCode() => Value;
        public static bool operator ==(ProductType left, ProductType right) => left.Equals(right);
        public static bool operator !=(ProductType left, ProductType right) => !left.Equals(right);
    }
}
