using System.Collections.Generic;
using UnityEngine;
using KerbalAdminKit.Util;
using KerbalCampaignKit.Core;

namespace KerbalAdminKit.KscRenderer
{
    /// <summary>
    /// Draws notification markers over KSC buildings in OnGUI. Markers pull
    /// their textures/colors from NotificationStyleRegistry; positions from
    /// each SpaceCenterBuilding transform via Camera.main.WorldToScreenPoint.
    /// SpaceCenterBuilding lookup is cached on first scene tick; FindObjectsOfType
    /// is too expensive to run in OnGUI per frame.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public sealed class KscNotificationRenderer : MonoBehaviour
    {
        // (canonical facility key, notification target prefix, fragment that should appear in
        // the SpaceCenterBuilding name to match it).
        private static readonly (string facility, string prefix, string nameFragment)[] Buildings =
        {
            ("Administration",          "admin",     "Administration"),
            ("MissionControl",          "mc",        "MissionControl"),
            ("TrackingStation",         "tracking",  "TrackingStation"),
            ("ResearchAndDevelopment",  "rd",        "ResearchAndDevelopment"),
            ("VehicleAssemblyBuilding", "vab",       "VehicleAssemblyBuilding"),
            ("SpaceplaneHangar",        "sph",       "SpaceplaneHangar"),
            ("AstronautComplex",        "astronaut", "AstronautComplex"),
        };

        private NotificationStyleRegistry styles;
        private KscMarkerOffsetRegistry offsets;
        private Dictionary<string, Transform> facilityTransforms;

        public void Initialize(NotificationStyleRegistry styles, KscMarkerOffsetRegistry offsets)
        {
            this.styles = styles;
            this.offsets = offsets;
        }

        private void OnGUI()
        {
            if (styles == null || offsets == null) return;
            if (Camera.main == null) return;

            EnsureFacilityCache();

            foreach (var b in Buildings)
            {
                var severity = CampaignKit.Notifications?.Highest(b.prefix);
                if (!severity.HasValue) continue;

                var style = styles.Get(severity.Value.ToString());
                if (style == null || string.IsNullOrEmpty(style.TextureUrl)) continue;
                var tex = TextureLoader.Get(style.TextureUrl);
                if (tex == null) continue;

                if (!facilityTransforms.TryGetValue(b.facility, out var t) || t == null) continue;

                var screen = Camera.main.WorldToScreenPoint(t.position);
                if (screen.z < 0) continue;

                var offset = offsets.Get(b.facility);
                var pos = new Vector2(screen.x + offset.x, Screen.height - screen.y - offset.y);

                var size = 32f * (style.Pulse ? (1f + 0.1f * Mathf.Sin(Time.time * 3f)) : 1f);
                var rect = new Rect(pos.x - size / 2f, pos.y - size / 2f, size, size);

                var original = GUI.color;
                GUI.color = KerbalAdminKit.Admin.ChipRenderer.ToUnity(style.Color);
                GUI.DrawTexture(rect, tex, ScaleMode.ScaleToFit);
                GUI.color = original;
            }
        }

        private void EnsureFacilityCache()
        {
            if (facilityTransforms != null) return;
            facilityTransforms = new Dictionary<string, Transform>();

            var allBuildings = Object.FindObjectsOfType<SpaceCenterBuilding>();
            foreach (var b in allBuildings)
            {
                if (b == null) continue;
                var name = b.facilityName ?? b.gameObject?.name ?? "";
                foreach (var entry in Buildings)
                {
                    if (facilityTransforms.ContainsKey(entry.facility)) continue;
                    if (name.IndexOf(entry.nameFragment, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        facilityTransforms[entry.facility] = b.transform;
                        break;
                    }
                }
            }
        }
    }
}
