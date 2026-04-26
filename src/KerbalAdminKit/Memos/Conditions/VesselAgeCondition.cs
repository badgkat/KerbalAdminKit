using System.Linq;

namespace KerbalAdminKit.Memos.Conditions
{
    /// <summary>
    /// True when the game contains at least one vessel whose age is in [MinDays, MaxDays].
    /// MaxDays = null means no upper bound. VesselType filter (e.g. "Relay", "Station") matches
    /// Vessel.vesselType.ToString(). Day = 21600s (Kerbin).
    /// </summary>
    public sealed class VesselAgeCondition : IMemoCondition
    {
        public string VesselType;
        public double MinDays;
        public double? MaxDays;

        private const double SecondsPerDay = 21600;

        public bool Evaluate(MemoContext ctx)
        {
            if (FlightGlobals.Vessels == null) return false;
            var now = ctx?.NowSeconds ?? Planetarium.GetUniversalTime();

            return FlightGlobals.Vessels.Any(v =>
            {
                if (v == null) return false;
                if (!string.IsNullOrEmpty(VesselType) &&
                    v.vesselType.ToString() != VesselType) return false;
                var ageDays = (now - v.launchTime) / SecondsPerDay;
                if (ageDays < MinDays) return false;
                if (MaxDays.HasValue && ageDays > MaxDays.Value) return false;
                return true;
            });
        }
    }
}
