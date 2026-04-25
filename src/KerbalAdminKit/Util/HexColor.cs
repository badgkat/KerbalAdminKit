using System;

namespace KerbalAdminKit.Util
{
    /// <summary>
    /// Framework-neutral color struct so we can unit-test without UnityEngine.
    /// Converted to UnityEngine.Color at render time via ChipRenderer.ToUnity().
    /// </summary>
    public struct ColorValue
    {
        public byte A, R, G, B;

        public static ColorValue White => new ColorValue { A = 255, R = 255, G = 255, B = 255 };

        public override bool Equals(object obj) =>
            obj is ColorValue c && c.A == A && c.R == R && c.G == G && c.B == B;
        public override int GetHashCode() => (A << 24) | (R << 16) | (G << 8) | B;
        public static bool operator ==(ColorValue a, ColorValue b) => a.Equals(b);
        public static bool operator !=(ColorValue a, ColorValue b) => !a.Equals(b);
    }

    public static class HexColor
    {
        public static ColorValue Parse(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return ColorValue.White;
            hex = hex.TrimStart('#').Trim();
            try
            {
                if (hex.Length == 8)
                {
                    return new ColorValue
                    {
                        A = Convert.ToByte(hex.Substring(0, 2), 16),
                        R = Convert.ToByte(hex.Substring(2, 2), 16),
                        G = Convert.ToByte(hex.Substring(4, 2), 16),
                        B = Convert.ToByte(hex.Substring(6, 2), 16),
                    };
                }
                if (hex.Length == 6)
                {
                    return new ColorValue
                    {
                        A = 0xFF,
                        R = Convert.ToByte(hex.Substring(0, 2), 16),
                        G = Convert.ToByte(hex.Substring(2, 2), 16),
                        B = Convert.ToByte(hex.Substring(4, 2), 16),
                    };
                }
            }
            catch (FormatException) { }
            catch (ArgumentOutOfRangeException) { }
            catch (OverflowException) { }
            return ColorValue.White;
        }
    }
}
