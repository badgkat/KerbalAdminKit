using KerbalAdminKit.Util;
using KerbalCampaignKit.Config;

namespace KerbalAdminKit.KscRenderer
{
    public static class NotificationStyleLoader
    {
        public static NotificationStyle Load(ISceneNode node)
        {
            if (node == null) return null;
            var severity = node.GetValue("severity");
            if (string.IsNullOrEmpty(severity)) return null;

            var s = new NotificationStyle { Severity = severity };
            if (node.HasValue("textureUrl")) s.TextureUrl = node.GetValue("textureUrl");
            if (node.HasValue("color")) s.Color = HexColor.Parse(node.GetValue("color"));
            if (node.HasValue("pulse") && bool.TryParse(node.GetValue("pulse"), out var p))
                s.Pulse = p;
            return s;
        }
    }
}
