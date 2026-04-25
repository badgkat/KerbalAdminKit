using System.Collections.Generic;
using KerbalAdminKit.Util;

namespace KerbalAdminKit.Characters
{
    public sealed class CharacterInfo
    {
        public string Id;
        public string DisplayName;
        public string Role;
        public ColorValue BaseColor;
        public string ChipShape = "square";   // reserved for v0.2

        public List<ChipLayerSpec> ChipLayers = new List<ChipLayerSpec>();

        public string OnClickScene;           // optional KDK scene id

        public string DispositionFlag;
        public string DefaultDisposition = "Neutral";

        // Per-disposition overrides
        public Dictionary<string, ColorValue> DispositionTints = new Dictionary<string, ColorValue>();
        public Dictionary<string, string> DispositionLabels = new Dictionary<string, string>();
    }
}
