using System.Collections.Generic;
using UnityEngine;
using KerbalAdminKit.Characters;
using KerbalAdminKit.Util;

namespace KerbalAdminKit.Admin
{
    /// <summary>
    /// Draws a composed chip: layered textures (or baseColor fallback) +
    /// disposition tint overlay on opt-in layers. Caller supplies the
    /// pixel rect and the chip spec.
    /// </summary>
    public static class ChipRenderer
    {
        public static void Draw(Rect chipRect, CharacterInfo info, string disposition, List<ChipLayerSpec> layerOverride = null)
        {
            if (info == null) return;

            var portrait = InstructorPortraitRenderer.Get(info.Id);
            if (portrait != null && (layerOverride == null || layerOverride.Count == 0))
            {
                GUI.DrawTexture(chipRect, portrait, ScaleMode.ScaleToFit);
                return;
            }

            var layers = layerOverride != null && layerOverride.Count > 0
                ? layerOverride
                : info.ChipLayers;

            if (layers == null || layers.Count == 0)
            {
                var tint = DispositionColors.ResolveTint(info, disposition);
                DrawFilledRect(chipRect, ToUnity(tint));
                return;
            }

            foreach (var layer in layers)
            {
                var tex = TextureLoader.Get(layer.TextureUrl);
                if (tex == null) continue;
                var layerRect = new Rect(
                    chipRect.x + layer.RectX * chipRect.width,
                    chipRect.y + (1 - layer.RectY - layer.RectH) * chipRect.height,
                    layer.RectW * chipRect.width,
                    layer.RectH * chipRect.height);

                if (layer.TintWithDisposition)
                {
                    var tint = DispositionColors.ResolveTint(info, disposition);
                    var original = GUI.color;
                    GUI.color = ToUnity(tint);
                    GUI.DrawTexture(layerRect, tex, ScaleMode.ScaleToFit);
                    GUI.color = original;
                }
                else
                {
                    GUI.DrawTexture(layerRect, tex, ScaleMode.ScaleToFit);
                }
            }
        }

        private static Texture2D solidPixel;
        private static Texture2D SolidPixel()
        {
            if (solidPixel == null)
            {
                solidPixel = new Texture2D(1, 1);
                solidPixel.SetPixel(0, 0, Color.white);
                solidPixel.Apply();
            }
            return solidPixel;
        }

        private static void DrawFilledRect(Rect r, Color c)
        {
            var original = GUI.color;
            GUI.color = c;
            GUI.DrawTexture(r, SolidPixel());
            GUI.color = original;
        }

        public static Color ToUnity(ColorValue c) =>
            new Color(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
    }
}
