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
            AddOrReplace(new NotificationStyle
            {
                Severity = "Action",
                TextureUrl = "KerbalAdminKit/Art/exclamation",
                Color = HexColor.Parse("#FFFF4040"),
                Pulse = true,
            });
            AddOrReplace(new NotificationStyle
            {
                Severity = "Info",
                TextureUrl = "KerbalAdminKit/Art/dot",
                Color = HexColor.Parse("#FFFFE066"),
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
