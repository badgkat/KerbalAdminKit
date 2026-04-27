using System.Collections.Generic;
using KerbalAdminKit.Characters;

namespace KerbalAdminKit.BuildingOverlays
{
    public sealed class BuildingScene
    {
        public string Facility;
        public string CharacterId;
        public string OnClickScene;
        public List<ChipLayerSpec> ChipLayerOverrides = new List<ChipLayerSpec>();
    }
}
