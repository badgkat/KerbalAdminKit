using HarmonyLib;
using UnityEngine;

namespace KerbalAdminKit.Admin
{
    /// <summary>
    /// Harmony prefix on SpaceCenterBuilding.OnClicked. When the clicked
    /// building is the Administration facility, returns false to suppress
    /// the stock admin scene and instead opens the AdminBuildingUI window.
    /// Installed only when replaceAdminBuilding = true.
    /// </summary>
    [HarmonyPatch(typeof(SpaceCenterBuilding), "OnClicked")]
    public static class AdminClickInterceptor
    {
        public static bool Enabled;
        public static AdminBuildingUI UI;

        static bool Prefix(SpaceCenterBuilding __instance)
        {
            if (!Enabled || UI == null) return true;
            if (__instance == null) return true;
            if (!IsAdministration(__instance)) return true;

            UI.Show();
            return false;
        }

        private static bool IsAdministration(SpaceCenterBuilding b)
        {
            var name = b.facilityName ?? b.gameObject?.name ?? "";
            return name.Contains("Administration") || name.Contains("admin");
        }
    }
}
