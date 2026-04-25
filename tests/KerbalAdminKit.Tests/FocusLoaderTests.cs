using Xunit;
using KerbalAdminKit.Focuses;
using KerbalAdminKit.Tests.TestHelpers;

namespace KerbalAdminKit.Tests
{
    public class FocusLoaderTests
    {
        [Fact]
        public void Load_ParsesAllFields()
        {
            var n = new FakeSceneNode()
                .Set("character", "wernher")
                .Set("id", "deep_space_signals")
                .Set("title", "Deep Space Signals")
                .Set("description", "Wernher focuses on anomaly signals")
                .Set("requirement", "chapter >= 3")
                .Set("flag", "wernher_focus")
                .Set("flagValue", "deep_space_signals");

            var f = FocusLoader.Load(n);

            Assert.Equal("wernher", f.CharacterId);
            Assert.Equal("deep_space_signals", f.Id);
            Assert.Equal("Deep Space Signals", f.Title);
            Assert.Equal("Wernher focuses on anomaly signals", f.Description);
            Assert.Equal("chapter >= 3", f.Requirement);
            Assert.Equal("wernher_focus", f.Flag);
            Assert.Equal("deep_space_signals", f.FlagValue);
        }

        [Fact]
        public void Load_ReturnsNullWhenCharacterMissing()
        {
            var n = new FakeSceneNode().Set("id", "no_char");
            Assert.Null(FocusLoader.Load(n));
        }

        [Fact]
        public void Load_ReturnsNullWhenIdMissing()
        {
            var n = new FakeSceneNode().Set("character", "x");
            Assert.Null(FocusLoader.Load(n));
        }
    }
}
