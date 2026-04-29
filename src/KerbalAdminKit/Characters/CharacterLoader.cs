using System.Globalization;
using KerbalAdminKit.Util;
using KerbalCampaignKit.Config;

namespace KerbalAdminKit.Characters
{
    public static class CharacterLoader
    {
        public static CharacterInfo Load(ISceneNode node)
        {
            if (node == null) return null;
            var id = node.GetValue("id");
            if (string.IsNullOrEmpty(id)) return null;

            var c = new CharacterInfo
            {
                Id = id,
                DisplayName = node.GetValue("displayName") ?? id,
                Role = node.GetValue("role") ?? "",
                BaseColor = HexColor.Parse(node.GetValue("baseColor")),
                DispositionFlag = node.GetValue("dispositionFlag"),
                DefaultDisposition = node.GetValue("defaultDisposition") ?? "Neutral",
                OnClickScene = node.GetValue("onClickScene"),
            };

            foreach (var layerNode in node.GetNodes("CHIP_LAYER"))
            {
                var layer = new ChipLayerSpec
                {
                    TextureUrl = layerNode.GetValue("textureUrl"),
                };
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
                    c.ChipLayers.Add(layer);
            }

            foreach (var tintNode in node.GetNodes("DISPOSITION_TINT"))
            {
                var value = tintNode.GetValue("value");
                var color = tintNode.GetValue("color");
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(color))
                    c.DispositionTints[value] = HexColor.Parse(color);
            }

            foreach (var labelNode in node.GetNodes("DISPOSITION_LABEL"))
            {
                var value = labelNode.GetValue("value");
                var text = labelNode.GetValue("text");
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(text))
                    c.DispositionLabels[value] = text;
            }

            return c;
        }
    }
}
