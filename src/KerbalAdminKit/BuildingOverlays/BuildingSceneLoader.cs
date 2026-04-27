using System.Globalization;
using KerbalAdminKit.Characters;
using KerbalCampaignKit.Config;

namespace KerbalAdminKit.BuildingOverlays
{
    public static class BuildingSceneLoader
    {
        public static BuildingScene Load(ISceneNode node)
        {
            if (node == null) return null;
            var facility = node.GetValue("facility");
            var character = node.GetValue("character");
            if (string.IsNullOrEmpty(facility) || string.IsNullOrEmpty(character)) return null;

            var bs = new BuildingScene
            {
                Facility = facility,
                CharacterId = character,
                OnClickScene = node.GetValue("onClickScene"),
            };

            foreach (var layerNode in node.GetNodes("CHIP_LAYER"))
            {
                var layer = new ChipLayerSpec { TextureUrl = layerNode.GetValue("textureUrl") };
                if (layerNode.HasValue("rect"))
                {
                    var parts = (layerNode.GetValue("rect") ?? "").Split(',');
                    if (parts.Length == 4 &&
                        float.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var x) &&
                        float.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var y) &&
                        float.TryParse(parts[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var w) &&
                        float.TryParse(parts[3].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var h))
                    {
                        layer.RectX = x; layer.RectY = y; layer.RectW = w; layer.RectH = h;
                    }
                }
                if (layerNode.HasValue("tintWithDisposition") &&
                    bool.TryParse(layerNode.GetValue("tintWithDisposition"), out var tint))
                    layer.TintWithDisposition = tint;

                if (!string.IsNullOrEmpty(layer.TextureUrl))
                    bs.ChipLayerOverrides.Add(layer);
            }

            return bs;
        }
    }
}
