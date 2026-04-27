using Xunit;
using KerbalAdminKit.KscRenderer;
using KerbalAdminKit.Util;
using KerbalAdminKit.Tests.TestHelpers;

namespace KerbalAdminKit.Tests
{
    public class NotificationStyleLoaderTests
    {
        [Fact]
        public void Load_ParsesSeverityStyle()
        {
            var n = new FakeSceneNode()
                .Set("severity", "Action")
                .Set("textureUrl", "KerbalAdminKit/Art/exclamation")
                .Set("color", "#FFFF4040")
                .Set("pulse", "true");

            var style = NotificationStyleLoader.Load(n);

            Assert.Equal("Action", style.Severity);
            Assert.Equal("KerbalAdminKit/Art/exclamation", style.TextureUrl);
            Assert.Equal(HexColor.Parse("#FFFF4040"), style.Color);
            Assert.True(style.Pulse);
        }

        [Fact]
        public void Load_ReturnsNullWhenSeverityMissing()
        {
            var n = new FakeSceneNode().Set("textureUrl", "x");
            Assert.Null(NotificationStyleLoader.Load(n));
        }

        [Fact]
        public void Registry_OverridesDefaults()
        {
            var reg = new NotificationStyleRegistry();
            reg.AddOrReplace(new NotificationStyle { Severity = "Action", TextureUrl = "custom" });
            Assert.Equal("custom", reg.Get("Action").TextureUrl);
        }
    }
}
