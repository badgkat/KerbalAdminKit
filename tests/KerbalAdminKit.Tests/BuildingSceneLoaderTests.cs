using System.Linq;
using Xunit;
using KerbalAdminKit.BuildingOverlays;
using KerbalAdminKit.Tests.TestHelpers;

namespace KerbalAdminKit.Tests
{
    public class BuildingSceneLoaderTests
    {
        [Fact]
        public void Load_ParsesCoreFields()
        {
            var n = new FakeSceneNode()
                .Set("facility", "MissionControl")
                .Set("character", "gene")
                .Set("onClickScene", "gene_mc_status_briefing");

            var bs = BuildingSceneLoader.Load(n);

            Assert.Equal("MissionControl", bs.Facility);
            Assert.Equal("gene", bs.CharacterId);
            Assert.Equal("gene_mc_status_briefing", bs.OnClickScene);
            Assert.Empty(bs.ChipLayerOverrides);
        }

        [Fact]
        public void Load_ReturnsNullWhenFacilityOrCharacterMissing()
        {
            Assert.Null(BuildingSceneLoader.Load(new FakeSceneNode().Set("character", "gus")));
            Assert.Null(BuildingSceneLoader.Load(new FakeSceneNode().Set("facility", "RD")));
        }

        [Fact]
        public void Load_ParsesChipLayerOverrides()
        {
            var n = new FakeSceneNode()
                .Set("facility", "MissionControl")
                .Set("character", "gene")
                .Set("onClickScene", "scene");
            n.AddChild("CHIP_LAYER", new FakeSceneNode().Set("textureUrl", "Override/gene_mc"));

            var bs = BuildingSceneLoader.Load(n);
            Assert.Single(bs.ChipLayerOverrides);
            Assert.Equal("Override/gene_mc", bs.ChipLayerOverrides[0].TextureUrl);
        }

        [Fact]
        public void Registry_GroupsByFacility()
        {
            var reg = new BuildingSceneRegistry();
            reg.Add(new BuildingScene { Facility = "MissionControl", CharacterId = "gene" });
            reg.Add(new BuildingScene { Facility = "MissionControl", CharacterId = "walt" });
            reg.Add(new BuildingScene { Facility = "TrackingStation", CharacterId = "linus" });

            var mc = reg.GetForFacility("MissionControl").ToList();
            Assert.Equal(2, mc.Count);
            Assert.Single(reg.GetForFacility("TrackingStation"));
        }
    }
}
