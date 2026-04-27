using System.Collections.Generic;
using System.Globalization;
using KerbalCampaignKit.Config;
using UnityEngine;

namespace KerbalAdminKit.KscRenderer
{
    public sealed class KscMarkerOffsetRegistry
    {
        private readonly Dictionary<string, Vector2> offsets =
            new Dictionary<string, Vector2>();

        public Vector2 Get(string building) =>
            offsets.TryGetValue(building ?? "", out var v) ? v : new Vector2(0, 20);

        public void Load(ISceneNode node)
        {
            if (node == null) return;
            var building = node.GetValue("building");
            if (string.IsNullOrEmpty(building)) return;
            float x = 0, y = 20;
            float.TryParse(node.GetValue("x"), NumberStyles.Float, CultureInfo.InvariantCulture, out x);
            float.TryParse(node.GetValue("y"), NumberStyles.Float, CultureInfo.InvariantCulture, out y);
            offsets[building] = new Vector2(x, y);
        }
    }
}
