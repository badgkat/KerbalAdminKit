using Xunit;
using KerbalAdminKit.Settings;
using KerbalAdminKit.Tests.TestHelpers;

namespace KerbalAdminKit.Tests
{
    public class AdminKitSettingsLoaderTests
    {
        [Fact]
        public void Load_Defaults_WhenNoFieldsSet()
        {
            var node = new FakeSceneNode();
            var s = AdminKitSettingsLoader.Load(node);

            Assert.False(s.ReplaceAdminBuilding);
            Assert.Equal(5, s.DeskMemoCount);
            Assert.Equal(5.0, s.MemoPollSeconds);
        }

        [Fact]
        public void Load_ParsesReplaceAdminBuilding()
        {
            var node = new FakeSceneNode().Set("replaceAdminBuilding", "true");
            var s = AdminKitSettingsLoader.Load(node);
            Assert.True(s.ReplaceAdminBuilding);
        }

        [Fact]
        public void Load_ParsesDeskMemoCount()
        {
            var node = new FakeSceneNode().Set("deskMemoCount", "10");
            var s = AdminKitSettingsLoader.Load(node);
            Assert.Equal(10, s.DeskMemoCount);
        }

        [Fact]
        public void Load_ParsesMemoPollSeconds()
        {
            var node = new FakeSceneNode().Set("memoPollSeconds", "2.5");
            var s = AdminKitSettingsLoader.Load(node);
            Assert.Equal(2.5, s.MemoPollSeconds);
        }

        [Fact]
        public void Load_IgnoresMalformedValuesAndKeepsDefaults()
        {
            var node = new FakeSceneNode()
                .Set("deskMemoCount", "not-a-number")
                .Set("memoPollSeconds", "garbage");
            var s = AdminKitSettingsLoader.Load(node);
            Assert.Equal(5, s.DeskMemoCount);
            Assert.Equal(5.0, s.MemoPollSeconds);
        }
    }
}
