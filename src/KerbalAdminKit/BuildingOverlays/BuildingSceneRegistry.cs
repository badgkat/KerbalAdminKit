using System.Collections.Generic;
using System.Linq;

namespace KerbalAdminKit.BuildingOverlays
{
    public sealed class BuildingSceneRegistry
    {
        private readonly List<BuildingScene> scenes = new List<BuildingScene>();

        public void Add(BuildingScene s) { if (s != null) scenes.Add(s); }

        public IEnumerable<BuildingScene> All => scenes;

        public IEnumerable<BuildingScene> GetForFacility(string facility) =>
            scenes.Where(s => s.Facility == facility);
    }
}
