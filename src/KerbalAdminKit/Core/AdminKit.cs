using KerbalAdminKit.BuildingOverlays;
using KerbalAdminKit.Characters;
using KerbalAdminKit.Focuses;
using KerbalAdminKit.Memos;

namespace KerbalAdminKit
{
    /// <summary>
    /// Public static facade. Wired by AdminKitAddon during startup.
    /// </summary>
    public static class AdminKit
    {
        public static CharacterRegistry Characters { get; internal set; }
        public static FocusRegistry Focuses { get; internal set; }
        public static MemoRegistry Memos { get; internal set; }
        public static BuildingSceneRegistry BuildingScenes { get; internal set; }
    }
}
