using System.Collections.Generic;
using UnityEngine;
using KerbalAdminKit.Util;
using KerbalCampaignKit.Core;

namespace KerbalAdminKit.KscRenderer
{
    /// <summary>
    /// Draws notification markers over KSC buildings. Pulls textures and
    /// colors from NotificationStyleRegistry and per-building offsets from
    /// KscMarkerOffsetRegistry. Building transforms are resolved lazily by
    /// scanning SpaceCenterBuilding objects in the scene (KSP 1.12 has no
    /// SpaceCenter property accessor for individual facilities).
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public sealed class KscNotificationRenderer : MonoBehaviour
    {
        private NotificationStyleRegistry styles;
        private KscMarkerOffsetRegistry offsets;

        // Each entry: (logical facility key, notification prefix, name fragments to match SpaceCenterBuilding by)
        private static readonly (string facility, string prefix, string[] nameFragments)[] Buildings = new[]
        {
            ("Administration",          "admin",     new[] { "Administration", "admin" }),
            ("MissionControl",          "mc",        new[] { "MissionControl", "Mission Control" }),
            ("TrackingStation",         "tracking",  new[] { "TrackingStation", "Tracking Station" }),
            ("ResearchAndDevelopment",  "rd",        new[] { "ResearchAndDevelopment", "Research" }),
            ("VehicleAssemblyBuilding", "vab",       new[] { "VehicleAssemblyBuilding", "VAB" }),
            ("SpaceplaneHangar",        "sph",       new[] { "SpaceplaneHangar", "SPH" }),
            ("AstronautComplex",        "astronaut", new[] { "AstronautComplex", "Astronaut" }),
        };

        private Dictionary<string, Transform> facilityCache;

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

            foreach (var entry in Buildings)
            {
                var severity = CampaignKit.Notifications?.Highest(entry.prefix);
                if (!severity.HasValue) continue;

                var style = styles.Get(severity.Value.ToString());
                if (style == null || string.IsNullOrEmpty(style.TextureUrl)) continue;
                var tex = TextureLoader.Get(style.TextureUrl);
                if (tex == null) continue;

                if (!facilityCache.TryGetValue(entry.facility, out var transform) || transform == null) continue;

                var screen = Camera.main.WorldToScreenPoint(transform.position);
                if (screen.z < 0) continue;

                var offset = offsets.Get(entry.facility);
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
            // Refresh when null OR when any cached transform was destroyed (scene reload).
            if (facilityCache != null)
            {
                bool stale = false;
                foreach (var kv in facilityCache) if (kv.Value == null) { stale = true; break; }
                if (!stale) return;
            }

            facilityCache = new Dictionary<string, Transform>();
            var allBuildings = Object.FindObjectsOfType<SpaceCenterBuilding>();
            foreach (var b in allBuildings)
            {
                if (b == null) continue;
                var name = b.facilityName ?? b.gameObject?.name ?? "";

                foreach (var entry in Buildings)
                {
                    if (facilityCache.ContainsKey(entry.facility)) continue;
                    foreach (var frag in entry.nameFragments)
                    {
                        if (name.IndexOf(frag, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            facilityCache[entry.facility] = b.transform;
                            break;
                        }
                    }
                }
            }
        }
    }
}
