using KerbalCampaignKit.Config;

namespace KerbalAdminKit.Settings
{
    public static class AdminKitSettingsLoader
    {
        public static AdminKitSettings Load(ISceneNode node)
        {
            var s = AdminKitSettings.Defaults();
            if (node == null) return s;

            if (node.HasValue("replaceAdminBuilding") &&
                bool.TryParse(node.GetValue("replaceAdminBuilding"), out var rab))
                s.ReplaceAdminBuilding = rab;

            if (node.HasValue("deskMemoCount") &&
                int.TryParse(node.GetValue("deskMemoCount"), out var dmc))
                s.DeskMemoCount = dmc;

            if (node.HasValue("memoPollSeconds") &&
                double.TryParse(node.GetValue("memoPollSeconds"), out var mps))
                s.MemoPollSeconds = mps;

            return s;
        }
    }
}
