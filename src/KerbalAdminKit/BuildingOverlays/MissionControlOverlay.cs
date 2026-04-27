using KSP.UI.Screens;

namespace KerbalAdminKit.BuildingOverlays
{
    /// <summary>
    /// MC has no dedicated scene in KSP 1.12 — its UI mounts inside SpaceCentre
    /// when the player clicks the Mission Control building. We register on
    /// SpaceCentre and gate render on MissionControl.Instance being active.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public sealed class MissionControlOverlay : BuildingOverlayBase
    {
        protected override string Facility => "MissionControl";
        protected override string NotificationPrefix => "mc";

        protected override bool IsBuildingOpen() =>
            MissionControl.Instance != null && MissionControl.Instance.gameObject.activeInHierarchy;
    }
}
