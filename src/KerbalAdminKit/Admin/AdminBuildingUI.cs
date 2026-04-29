using UnityEngine;
using ClickThroughFix;
using KerbalAdminKit.Characters;
using KerbalAdminKit.Focuses;
using KerbalAdminKit.Memos;
using KerbalAdminKit.Settings;

namespace KerbalAdminKit.Admin
{
    /// <summary>
    /// Fullscreen IMGUI admin screen. Three panels side-by-side.
    /// Opened by AdminClickInterceptor when the administration building is
    /// clicked (and replaceAdminBuilding = true).
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public sealed class AdminBuildingUI : MonoBehaviour
    {
        public static AdminBuildingUI Instance;

        private bool visible;
        private Rect window = new Rect(40, 40, 1200, 700);
        private const int WindowId = 0x4B41_4B00;

        private CharacterPanel characterPanel;
        private DashboardPanel dashboardPanel;
        private DeskPanel deskPanel;
        private FocusPicker focusPicker;

        private void Awake() { Instance = this; }

        public void Initialize(
            CharacterRegistry characters,
            FocusRegistry focuses,
            MemoRegistry memos,
            AdminKitSettings settings,
            PrCampaignConfig prConfig)
        {
            focusPicker = new FocusPicker(focuses, characters);
            characterPanel = new CharacterPanel(characters, focuses, focusPicker);
            dashboardPanel = new DashboardPanel(characters, focuses);
            deskPanel = new DeskPanel(memos, settings, prConfig);

            AdminClickInterceptor.UI = this;
        }

        public void Show()
        {
            visible = true;
            var w = Mathf.Min(1200, Screen.width - 80);
            var h = Mathf.Min(700, Screen.height - 80);
            window = new Rect((Screen.width - w) / 2, (Screen.height - h) / 2, w, h);
        }

        public void Hide() => visible = false;

        private void OnGUI()
        {
            if (!visible) return;
            if (characterPanel == null) return;

            // Use GUIWindow (not GUILayoutWindow): DrawWindow positions panels via
            // absolute Rects + BeginArea, which doesn't propagate height back to
            // the outer GUILayout flow — GUILayoutWindow would collapse to a tiny
            // square. GUIWindow honors the explicit rect we pass.
            window = ClickThruBlocker.GUIWindow(
                WindowId, window, DrawWindow, "Administration", GUI.skin.window);

            // Modals must be opened from OnGUI, not from inside another window's
            // draw callback. Render any open modals here, after the main window.
            focusPicker?.OnGUI();
            deskPanel?.OnGUI();
        }

        private void DrawWindow(int id)
        {
            var padding = 8f;
            var innerW = window.width - padding * 2;
            var innerH = window.height - 44f;
            var colW = (innerW - padding * 2) / 3f;

            var charRect = new Rect(padding, 30, colW, innerH);
            var dashRect = new Rect(padding + colW + padding, 30, colW, innerH);
            var deskRect = new Rect(padding + (colW + padding) * 2, 30, colW, innerH);

            characterPanel?.Draw(charRect);
            dashboardPanel?.Draw(dashRect);
            deskPanel?.Draw(deskRect);

            var closeRect = new Rect(window.width - 80, 4, 70, 20);
            if (GUI.Button(closeRect, "Close")) Hide();

            GUI.DragWindow(new Rect(0, 0, window.width - 90, 24));
        }
    }
}
