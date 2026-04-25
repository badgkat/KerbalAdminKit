using KerbalAdminKit.Util;

namespace KerbalAdminKit.Characters
{
    public static class DispositionColors
    {
        public static ColorValue ResolveTint(CharacterInfo info, string disposition)
        {
            if (info == null) return ColorValue.White;
            if (info.DispositionTints != null &&
                info.DispositionTints.TryGetValue(disposition ?? "", out var overrideColor))
                return overrideColor;

            return Derive(info.BaseColor, disposition);
        }

        public static string ResolveLabel(CharacterInfo info, string disposition)
        {
            if (info?.DispositionLabels != null &&
                info.DispositionLabels.TryGetValue(disposition ?? "", out var label))
                return label;
            return disposition;
        }

        private static ColorValue Derive(ColorValue baseColor, string disposition)
        {
            switch (disposition)
            {
                case "Enthusiastic": return Brighten(baseColor, 0.25f);
                case "Supportive":   return Brighten(baseColor, 0.10f);
                case "Neutral":      return baseColor;
                case "Skeptical":    return Darken(baseColor, 0.15f);
                case "Frustrated":   return Desaturate(Darken(baseColor, 0.25f), 0.4f);
                default:             return baseColor;
            }
        }

        private static ColorValue Brighten(ColorValue c, float amount)
        {
            return new ColorValue
            {
                A = c.A,
                R = ToByte(c.R + (255 - c.R) * amount),
                G = ToByte(c.G + (255 - c.G) * amount),
                B = ToByte(c.B + (255 - c.B) * amount),
            };
        }

        private static ColorValue Darken(ColorValue c, float amount)
        {
            return new ColorValue
            {
                A = c.A,
                R = ToByte(c.R * (1 - amount)),
                G = ToByte(c.G * (1 - amount)),
                B = ToByte(c.B * (1 - amount)),
            };
        }

        private static ColorValue Desaturate(ColorValue c, float amount)
        {
            int avg = (c.R + c.G + c.B) / 3;
            return new ColorValue
            {
                A = c.A,
                R = ToByte(c.R + (avg - c.R) * amount),
                G = ToByte(c.G + (avg - c.G) * amount),
                B = ToByte(c.B + (avg - c.B) * amount),
            };
        }

        private static byte ToByte(float v)
        {
            if (v < 0) return 0;
            if (v > 255) return 255;
            return (byte)v;
        }
    }
}
