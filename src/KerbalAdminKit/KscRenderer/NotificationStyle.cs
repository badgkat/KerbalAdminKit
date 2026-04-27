using KerbalAdminKit.Util;

namespace KerbalAdminKit.KscRenderer
{
    public sealed class NotificationStyle
    {
        public string Severity;
        public string TextureUrl;
        public ColorValue Color = ColorValue.White;
        public bool Pulse;
    }
}
