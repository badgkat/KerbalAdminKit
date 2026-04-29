using System.Collections.Generic;
using KerbalAdminKit.Util;

namespace KerbalAdminKit.KscRenderer
{
    public sealed class NotificationStyleRegistry
    {
        private readonly Dictionary<string, NotificationStyle> bySeverity =
            new Dictionary<string, NotificationStyle>();

        public NotificationStyleRegistry()
        {
            // Default colors are white because the shipped glyphs are already
            // colored (red exclamation, yellow dot). Content mods that ship
            // their own greyscale glyphs can override Color via cfg.
            AddOrReplace(new NotificationStyle
            {
                Severity = "Action",
                TextureUrl = "KerbalAdminKit/Art/exclamation",
                Color = HexColor.Parse("#FFFFFFFF"),
                Pulse = true,
            });
            AddOrReplace(new NotificationStyle
            {
                Severity = "Info",
                TextureUrl = "KerbalAdminKit/Art/dot",
                Color = HexColor.Parse("#FFFFFFFF"),
                Pulse = false,
            });
        }

        public void AddOrReplace(NotificationStyle s)
        {
            if (s == null || string.IsNullOrEmpty(s.Severity)) return;
            bySeverity[s.Severity] = s;
        }

        public NotificationStyle Get(string severity) =>
            (severity != null && bySeverity.TryGetValue(severity, out var s)) ? s : null;
    }
}
