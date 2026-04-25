using Xunit;
using KerbalAdminKit.Characters;
using KerbalAdminKit.Util;

namespace KerbalAdminKit.Tests
{
    public class DispositionColorsTests
    {
        [Fact]
        public void ResolveTint_ReturnsBaseWhenNoOverrideAndUnknownDisposition()
        {
            var info = new CharacterInfo
            {
                Id = "gus",
                BaseColor = HexColor.Parse("#FFFFC078"),
            };
            var tint = DispositionColors.ResolveTint(info, "TotallyUnknownMood");
            Assert.Equal(info.BaseColor, tint);
        }

        [Fact]
        public void ResolveTint_UsesOverrideWhenMatched()
        {
            var info = new CharacterInfo
            {
                Id = "gus",
                BaseColor = HexColor.Parse("#FFFFC078"),
            };
            info.DispositionTints["Enthusiastic"] = HexColor.Parse("#FFA0D4FF");
            var tint = DispositionColors.ResolveTint(info, "Enthusiastic");
            Assert.Equal(HexColor.Parse("#FFA0D4FF"), tint);
        }

        [Fact]
        public void ResolveTint_Enthusiastic_BrightensBaseColor()
        {
            var info = new CharacterInfo
            {
                Id = "x",
                BaseColor = new ColorValue { A = 255, R = 100, G = 100, B = 100 },
            };
            var tint = DispositionColors.ResolveTint(info, "Enthusiastic");
            Assert.True(tint.R > 100);
            Assert.True(tint.G > 100);
            Assert.True(tint.B > 100);
        }

        [Fact]
        public void ResolveTint_Neutral_ReturnsBaseColor()
        {
            var info = new CharacterInfo
            {
                Id = "x",
                BaseColor = new ColorValue { A = 255, R = 128, G = 128, B = 128 },
            };
            var tint = DispositionColors.ResolveTint(info, "Neutral");
            Assert.Equal(info.BaseColor, tint);
        }

        [Fact]
        public void ResolveLabel_ReturnsOverrideIfPresent()
        {
            var info = new CharacterInfo { Id = "x" };
            info.DispositionLabels["Enthusiastic"] = "Thrilled";
            Assert.Equal("Thrilled", DispositionColors.ResolveLabel(info, "Enthusiastic"));
        }

        [Fact]
        public void ResolveLabel_ReturnsRawValueWhenNoOverride()
        {
            var info = new CharacterInfo { Id = "x" };
            Assert.Equal("Supportive", DispositionColors.ResolveLabel(info, "Supportive"));
        }
    }
}
