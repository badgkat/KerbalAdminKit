using HarmonyLib;

namespace KerbalAdminKit.Admin
{
    /// <summary>
    /// Harmony prefix on AdministrationFacility.OnClicked. AdministrationFacility
    /// overrides SpaceCenterBuilding.OnClicked, so a patch on the base class is
    /// never invoked (virtual dispatch routes straight to the derived method).
    /// Patch the derived type directly.
    /// </summary>
    [HarmonyPatch(typeof(AdministrationFacility), "OnClicked")]
    public static class AdminClickInterceptor
    {
        public static bool Enabled;
        public static AdminBuildingUI UI;

        static bool Prefix()
        {
            if (!Enabled || UI == null) return true;
            UI.Show();
            return false;
        }
    }
}
