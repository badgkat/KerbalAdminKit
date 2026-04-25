namespace KerbalAdminKit.Settings
{
    public sealed class AdminKitSettings
    {
        public bool ReplaceAdminBuilding;
        public int DeskMemoCount = 5;
        public double MemoPollSeconds = 5.0;

        public static AdminKitSettings Defaults() => new AdminKitSettings();
    }
}
