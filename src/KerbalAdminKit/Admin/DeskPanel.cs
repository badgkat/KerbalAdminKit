using UnityEngine;
using ClickThroughFix;
using KerbalAdminKit.Memos;
using KerbalAdminKit.Settings;

namespace KerbalAdminKit.Admin
{
    public sealed class DeskPanel
    {
        private readonly MemoRegistry memos;
        private readonly AdminKitSettings settings;
        private readonly PrCampaignConfig prConfig;
        private Vector2 scroll;
        private bool confirmingPr;
        private Rect prConfirmRect = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 100, 400, 200);
        private const int PrConfirmWindowId = 0x4B41_4B02;

        private static Texture2D opaqueBg;

        public DeskPanel(MemoRegistry memos, AdminKitSettings settings, PrCampaignConfig prConfig)
        {
            this.memos = memos;
            this.settings = settings;
            this.prConfig = prConfig;
        }

        public void Draw(Rect area)
        {
            GUILayout.BeginArea(area, GUI.skin.box);
            scroll = GUILayout.BeginScrollView(scroll);

            DrawMemos();
            DrawPrButton();

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Renders the PR-campaign confirmation modal. Must be called from
        /// the host's OnGUI AFTER the main window draw — IMGUI doesn't allow
        /// top-level windows to be opened from inside another window's callback.
        /// </summary>
        public void OnGUI()
        {
            if (confirmingPr) DrawPrConfirm();
        }

        private void DrawMemos()
        {
            GUILayout.Label("<b>Memos</b>");
            int shown = 0;
            foreach (var m in memos.Active)
            {
                if (shown >= settings.DeskMemoCount) break;
                GUILayout.BeginVertical(GUI.skin.box);
                if (!string.IsNullOrEmpty(m.CharacterId))
                    GUILayout.Label($"<b>{m.CharacterId}:</b>");
                GUILayout.Label(m.Text ?? "");
                if (GUILayout.Button("Dismiss"))
                    memos.Dismiss(m.Id, Planetarium.GetUniversalTime());
                GUILayout.EndVertical();
                GUILayout.Space(4);
                shown++;
            }
            if (shown == 0) GUILayout.Label("— no memos —");
        }

        private void DrawPrButton()
        {
            if (prConfig == null) return;
            GUILayout.Space(12);
            var cost = prConfig.ComputeCost(CurrentTierIndex());
            if (GUILayout.Button($"PR Campaign ({(int)cost} funds)"))
                confirmingPr = true;
        }

        private void DrawPrConfirm()
        {
            prConfirmRect = ClickThruBlocker.GUILayoutWindow(
                PrConfirmWindowId, prConfirmRect, DrawPrConfirmContents, "PR Campaign", GUI.skin.window);
            GUI.BringWindowToFront(PrConfirmWindowId);
        }

        private void DrawPrConfirmContents(int id)
        {
            var fill = new Rect(0, 0, prConfirmRect.width, prConfirmRect.height);
            GUI.DrawTexture(fill, OpaqueBackground());

            var cost = prConfig.ComputeCost(CurrentTierIndex());
            GUILayout.Label($"Run a PR campaign for {(int)cost} funds?");
            GUILayout.Label($"+{(int)prConfig.RepBonus} reputation, halt decay {(int)prConfig.HaltDecayDays} days.");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirm"))
            {
                PrCampaignAction.Execute(prConfig, CurrentTierIndex());
                confirmingPr = false;
            }
            if (GUILayout.Button("Cancel")) confirmingPr = false;
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        private static Texture2D OpaqueBackground()
        {
            if (opaqueBg == null)
            {
                opaqueBg = new Texture2D(1, 1);
                opaqueBg.SetPixel(0, 0, new Color(0.10f, 0.10f, 0.12f, 1f));
                opaqueBg.Apply();
            }
            return opaqueBg;
        }

        private int CurrentTierIndex()
        {
            var income = KerbalCampaignKit.Core.CampaignKit.Reputation?.Income;
            if (income == null || income.Tiers == null) return 0;
            var rep = global::Reputation.Instance != null ? global::Reputation.Instance.reputation : 0f;
            for (int i = 0; i < income.Tiers.Count; i++)
            {
                var t = income.Tiers[i];
                if (rep >= t.Min && rep < t.Max) return i;
            }
            return 0;
        }
    }
}
