using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using KerbalAdminKit.Admin;
using KerbalAdminKit.BuildingOverlays;
using KerbalAdminKit.Characters;
using KerbalAdminKit.DecayCompiler;
using KerbalAdminKit.Focuses;
using KerbalAdminKit.KscRenderer;
using KerbalAdminKit.Memos;
using KerbalAdminKit.Settings;
using KerbalCampaignKit.Config;

namespace KerbalAdminKit
{
    /// <summary>
    /// Top-level boot addon. Loads cfg, builds registries, wires the public
    /// AdminKit facade, installs the Harmony admin-click interceptor (if
    /// enabled), and re-initializes scene-targeted MonoBehaviours each time
    /// SpaceCenter or Tracking Station is loaded.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public sealed class AdminKitAddon : MonoBehaviour
    {
        private AdminKitSettings settings;
        private CharacterRegistry characters;
        private FocusRegistry focuses;
        private MemoRegistry memos;
        private NotificationStyleRegistry styles;
        private KscMarkerOffsetRegistry offsets;
        private BuildingSceneRegistry buildingScenes;
        private PrCampaignConfig prConfig;
        private InMemoryDecaySink decaySink;

        private bool anyCfg;

        private void Awake()
        {
            DontDestroyOnLoad(this);

            LoadSettings();
            LoadCharacters();
            LoadFocuses();
            LoadMemos();
            LoadNotificationStyles();
            LoadMarkerOffsets();
            LoadBuildingScenes();
            LoadPrCampaign();
            CompileDispositionDecay();

            AdminKit.Characters = characters;
            AdminKit.Focuses = focuses;
            AdminKit.Memos = memos;
            AdminKit.BuildingScenes = buildingScenes;

            if (!anyCfg)
            {
                Debug.Log("[KerbalAdminKit] No DIRECTOR_* cfg found — remaining inert.");
                return;
            }

            if (settings.ReplaceAdminBuilding)
                InstallAdminInterceptor();

            GameEvents.onLevelWasLoaded.Add(OnLevelLoaded);
        }

        private void LoadSettings()
        {
            var nodes = GameDatabase.Instance.GetConfigNodes("KERBAL_ADMIN_KIT");
            settings = nodes.Length > 0
                ? AdminKitSettingsLoader.Load(new ConfigNodeAdapter(nodes[0]))
                : AdminKitSettings.Defaults();
            if (nodes.Length > 0) anyCfg = true;
        }

        private void LoadCharacters()
        {
            characters = new CharacterRegistry();
            var nodes = GameDatabase.Instance.GetConfigNodes("DIRECTOR_CHARACTER");
            foreach (var n in nodes)
            {
                var c = CharacterLoader.Load(new ConfigNodeAdapter(n));
                if (c != null) characters.Add(c);
            }
            if (nodes.Length > 0) anyCfg = true;
        }

        private void LoadFocuses()
        {
            focuses = new FocusRegistry();
            var nodes = GameDatabase.Instance.GetConfigNodes("DIRECTOR_FOCUS");
            foreach (var n in nodes)
            {
                var f = FocusLoader.Load(new ConfigNodeAdapter(n));
                if (f != null) focuses.Add(f);
            }
            if (nodes.Length > 0) anyCfg = true;
        }

        private void LoadMemos()
        {
            memos = new MemoRegistry();
            var nodes = GameDatabase.Instance.GetConfigNodes("DIRECTOR_MEMO");
            foreach (var n in nodes)
            {
                var m = MemoLoader.Load(new ConfigNodeAdapter(n));
                if (m != null) memos.Add(m);
            }
            if (nodes.Length > 0) anyCfg = true;
        }

        private void LoadNotificationStyles()
        {
            styles = new NotificationStyleRegistry();
            var nodes = GameDatabase.Instance.GetConfigNodes("DIRECTOR_NOTIFICATION_STYLE");
            foreach (var n in nodes)
            {
                var s = NotificationStyleLoader.Load(new ConfigNodeAdapter(n));
                if (s != null) styles.AddOrReplace(s);
            }
            if (nodes.Length > 0) anyCfg = true;
        }

        private void LoadMarkerOffsets()
        {
            offsets = new KscMarkerOffsetRegistry();
            var nodes = GameDatabase.Instance.GetConfigNodes("DIRECTOR_KSC_MARKER_OFFSET");
            foreach (var n in nodes) offsets.Load(new ConfigNodeAdapter(n));
            if (nodes.Length > 0) anyCfg = true;
        }

        private void LoadBuildingScenes()
        {
            buildingScenes = new BuildingSceneRegistry();
            var nodes = GameDatabase.Instance.GetConfigNodes("DIRECTOR_BUILDING_SCENE");
            foreach (var n in nodes)
            {
                var bs = BuildingSceneLoader.Load(new ConfigNodeAdapter(n));
                if (bs != null) buildingScenes.Add(bs);
            }
            if (nodes.Length > 0) anyCfg = true;
        }

        private void LoadPrCampaign()
        {
            var nodes = GameDatabase.Instance.GetConfigNodes("DIRECTOR_PR_CAMPAIGN");
            if (nodes.Length > 0)
            {
                prConfig = PrCampaignConfig.Load(new ConfigNodeAdapter(nodes[0]));
                anyCfg = true;
            }
        }

        private void CompileDispositionDecay()
        {
            var nodes = GameDatabase.Instance.GetConfigNodes("DISPOSITION_DECAY");
            if (nodes.Length == 0) return;
            anyCfg = true;

            decaySink = new InMemoryDecaySink();
            foreach (var n in nodes)
            {
                var decay = DispositionDecayLoader.Load(new ConfigNodeAdapter(n));
                DispositionDecayCompiler.Compile(decay, decaySink);
            }
        }

        private void InstallAdminInterceptor()
        {
            var harmony = new Harmony("badgkat.KerbalAdminKit");
            harmony.PatchAll(typeof(AdminClickInterceptor).Assembly);
            AdminClickInterceptor.Enabled = true;
        }

        private void OnLevelLoaded(GameScenes scene)
        {
            if (scene == GameScenes.SPACECENTER)
            {
                WireAtSpaceCentre();
            }
            else if (scene == GameScenes.TRACKSTATION)
            {
                var tso = FindObjectOfType<TrackingStationOverlay>();
                if (tso != null) tso.Initialize(buildingScenes, characters);
            }
            // MissionControl is a popup inside SpaceCentre — its overlay is wired
            // alongside SpaceCentre's other addons via WireAtSpaceCentre below.
        }

        private void WireAtSpaceCentre()
        {
            var ui = AdminBuildingUI.Instance;
            if (ui != null) ui.Initialize(characters, focuses, memos, settings, prConfig);

            var memoTicker = FindObjectOfType<MemoTicker>();
            if (memoTicker != null) memoTicker.Initialize(memos, settings.MemoPollSeconds);

            var renderer = FindObjectOfType<KscNotificationRenderer>();
            if (renderer != null) renderer.Initialize(styles, offsets);

            var decayTicker = FindObjectOfType<DispositionDecayTicker>();
            if (decayTicker != null && decaySink != null)
                decayTicker.Initialize(decaySink.Triggers);

            var mcOverlay = FindObjectOfType<MissionControlOverlay>();
            if (mcOverlay != null) mcOverlay.Initialize(buildingScenes, characters);
        }
    }

    /// <summary>
    /// Collects compiled decay triggers in memory. DispositionDecayTicker
    /// reads from this list on a polling cadence and applies transitions by
    /// reading and updating KDK flags directly. We skip KCK's trigger engine
    /// for decay because KCK's TimeElapsed events require an external source
    /// to fire them — this ticker is that source.
    /// </summary>
    internal sealed class InMemoryDecaySink : DecayCompiler.ITriggerSink
    {
        public List<DecayCompiler.CompiledDecayTrigger> Triggers =
            new List<DecayCompiler.CompiledDecayTrigger>();
        public void Register(DecayCompiler.CompiledDecayTrigger t) => Triggers.Add(t);
    }
}
