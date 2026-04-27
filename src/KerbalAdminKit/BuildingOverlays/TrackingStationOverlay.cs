namespace KerbalAdminKit.BuildingOverlays
{
    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public sealed class TrackingStationOverlay : BuildingOverlayBase
    {
        protected override string Facility => "TrackingStation";
        protected override string NotificationPrefix => "tracking";
    }
}
