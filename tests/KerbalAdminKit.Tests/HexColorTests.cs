using Xunit;
using KerbalAdminKit.Util;

namespace KerbalAdminKit.Tests
{
    public class HexColorTests
    {
        [Fact]
        public void Parse_EightDigitHex_IncludesAlpha()
        {
            // #FF82B4E8 → A=FF R=82 G=B4 B=E8
            var c = HexColor.Parse("#FF82B4E8");
            Assert.Equal(0xFF, c.A);
            Assert.Equal(0x82, c.R);
            Assert.Equal(0xB4, c.G);
            Assert.Equal(0xE8, c.B);
        }

        [Fact]
        public void Parse_SixDigitHex_DefaultsAlphaToFF()
        {
            var c = HexColor.Parse("#82B4E8");
            Assert.Equal(0xFF, c.A);
            Assert.Equal(0x82, c.R);
        }

        [Fact]
        public void Parse_NoHash_Works()
        {
            var c = HexColor.Parse("82B4E8");
            Assert.Equal(0x82, c.R);
        }

        [Fact]
        public void Parse_Invalid_ReturnsWhite()
        {
            var c = HexColor.Parse("garbage");
            Assert.Equal(0xFF, c.R);
            Assert.Equal(0xFF, c.G);
            Assert.Equal(0xFF, c.B);
            Assert.Equal(0xFF, c.A);
        }
    }
}
