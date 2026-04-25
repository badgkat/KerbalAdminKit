using System.Linq;
using Xunit;
using KerbalAdminKit.Characters;
using KerbalAdminKit.Util;
using KerbalAdminKit.Tests.TestHelpers;

namespace KerbalAdminKit.Tests
{
    public class CharacterLoaderTests
    {
        [Fact]
        public void Load_ParsesCoreFields()
        {
            var n = new FakeSceneNode()
                .Set("id", "wernher")
                .Set("displayName", "Wernher von Kerman")
                .Set("role", "Chief Scientist")
                .Set("baseColor", "#FF82B4E8")
                .Set("dispositionFlag", "wernher_disposition")
                .Set("defaultDisposition", "Supportive")
                .Set("onClickScene", "wernher_intro");

            var c = CharacterLoader.Load(n);

            Assert.Equal("wernher", c.Id);
            Assert.Equal("Wernher von Kerman", c.DisplayName);
            Assert.Equal("Chief Scientist", c.Role);
            Assert.Equal(HexColor.Parse("#FF82B4E8"), c.BaseColor);
            Assert.Equal("wernher_disposition", c.DispositionFlag);
            Assert.Equal("Supportive", c.DefaultDisposition);
            Assert.Equal("wernher_intro", c.OnClickScene);
        }

        [Fact]
        public void Load_ReturnsNull_WhenIdMissing()
        {
            var n = new FakeSceneNode().Set("displayName", "no id");
            Assert.Null(CharacterLoader.Load(n));
        }

        [Fact]
        public void Load_ParsesChipLayers_InOrder()
        {
            var n = new FakeSceneNode().Set("id", "gus").Set("baseColor", "#FFFFC078");
            n.AddChild("CHIP_LAYER", new FakeSceneNode().Set("textureUrl", "A/bg"));
            n.AddChild("CHIP_LAYER", new FakeSceneNode()
                .Set("textureUrl", "A/portrait")
                .Set("tintWithDisposition", "true"));
            n.AddChild("CHIP_LAYER", new FakeSceneNode()
                .Set("textureUrl", "A/badge")
                .Set("rect", "0.7, 0.0, 0.3, 0.3"));

            var c = CharacterLoader.Load(n);

            Assert.Equal(3, c.ChipLayers.Count);
            Assert.Equal("A/bg", c.ChipLayers[0].TextureUrl);
            Assert.False(c.ChipLayers[0].TintWithDisposition);

            Assert.Equal("A/portrait", c.ChipLayers[1].TextureUrl);
            Assert.True(c.ChipLayers[1].TintWithDisposition);

            Assert.Equal("A/badge", c.ChipLayers[2].TextureUrl);
            Assert.Equal(0.7f, c.ChipLayers[2].RectX);
            Assert.Equal(0.0f, c.ChipLayers[2].RectY);
            Assert.Equal(0.3f, c.ChipLayers[2].RectW);
            Assert.Equal(0.3f, c.ChipLayers[2].RectH);
        }

        [Fact]
        public void Load_ParsesDispositionTintOverrides()
        {
            var n = new FakeSceneNode().Set("id", "x").Set("baseColor", "#FF808080");
            n.AddChild("DISPOSITION_TINT", new FakeSceneNode()
                .Set("value", "Enthusiastic")
                .Set("color", "#FFA0D4FF"));

            var c = CharacterLoader.Load(n);
            Assert.Equal(HexColor.Parse("#FFA0D4FF"), c.DispositionTints["Enthusiastic"]);
        }

        [Fact]
        public void Load_ParsesDispositionLabelOverrides()
        {
            var n = new FakeSceneNode().Set("id", "x").Set("baseColor", "#FF808080");
            n.AddChild("DISPOSITION_LABEL", new FakeSceneNode()
                .Set("value", "Enthusiastic")
                .Set("text", "Thrilled"));

            var c = CharacterLoader.Load(n);
            Assert.Equal("Thrilled", c.DispositionLabels["Enthusiastic"]);
        }

        [Fact]
        public void Registry_GetAndAll_Works()
        {
            var reg = new CharacterRegistry();
            reg.Add(new CharacterInfo { Id = "gene" });
            reg.Add(new CharacterInfo { Id = "gus"  });

            Assert.Equal(2, reg.All.Count());
            Assert.Equal("gus", reg.Get("gus").Id);
            Assert.Null(reg.Get("missing"));
        }
    }
}
