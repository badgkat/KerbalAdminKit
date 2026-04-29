using System.Collections.Generic;
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

        // High-priority memos require a second click to confirm dismissal so
        // the player doesn't blow past something the program lead flagged.
        private readonly HashSet<string> dismissArmed = new HashSet<string>();

        private static Texture2D opaqueBg;

        /// <summary>
        /// Called when the admin window closes so confirmation arming
        /// doesn't persist across separate visits.
        /// </summary>
        public void Reset()
        {
            dismissArmed.Clear();
            confirmingPr = false;
        }

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
            if (!confirmingPr) return;

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                confirmingPr = false;
                Event.current.Use();
                return;
            }

            DrawPrConfirm();
        }

        private void DrawMemos()
        {
            GUILayout.Label("<b>Memos</b>");
            int shown = 0;
            foreach (var m in memos.Active)
            {
                if (shown >= settings.DeskMemoCount) break;
                GUILayout.BeginVertical(GUI.skin.box);

                var prefix = m.Priority == MemoPriority.High ? "<color=#ff8080>!</color> " : "";
                if (!string.IsNullOrEmpty(m.CharacterId))
                    GUILayout.Label($"{prefix}<b>{m.CharacterId}:</b>");
                else if (!string.IsNullOrEmpty(prefix))
                    GUILayout.Label(prefix.TrimEnd());
                GUILayout.Label(m.Text ?? "");

                var isHigh = m.Priority == MemoPriority.High;
                var armed = dismissArmed.Contains(m.Id);
                string label;
                if (isHigh && !armed) label = "Dismiss…";
                else if (isHigh && armed) label = "Confirm dismiss";
                else label = "Dismiss";

                if (GUILayout.Button(label))
                {
                    if (isHigh && !armed)
                    {
                        dismissArmed.Add(m.Id);
                    }
                    else
                    {
                        memos.Dismiss(m.Id, Planetarium.GetUniversalTime());
                        dismissArmed.Remove(m.Id);
                    }
                }

                GUILayout.EndVertical();
                GUILayout.Space(4);
                shown++;
            }
            if (shown == 0)
                GUILayout.Label("<i>Inbox is clear.</i>");
        }

        private void DrawPrButton()
        {
            if (prConfig == null) return;
            GUILayout.Space(12);
            var tier = CurrentTierIndex();
            var cost = prConfig.ComputeCost(tier);
            var canAfford = PrCampaignAction.CanAfford(prConfig, tier);
            var cooldownDays = PrCampaignAction.CooldownRemainingDays(prConfig);
            var available = canAfford && cooldownDays <= 0;

            GUI.enabled = available;
            if (GUILayout.Button($"PR Campaign ({(int)cost} funds)"))
                confirmingPr = true;
            GUI.enabled = true;

            if (!canAfford)
                GUILayout.Label("<i>Not enough funds.</i>");
            else if (cooldownDays > 0)
                GUILayout.Label($"<i>On cooldown for {(int)System.Math.Ceiling(cooldownDays)} more days.</i>");
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

            var tier = CurrentTierIndex();
            var cost = prConfig.ComputeCost(tier);
            var canAfford = PrCampaignAction.CanAfford(prConfig, tier);
            var cooldownDays = PrCampaignAction.CooldownRemainingDays(prConfig);
            var available = canAfford && cooldownDays <= 0;

            GUILayout.Label($"Run a PR campaign for {(int)cost} funds?");
            GUILayout.Label($"+{(int)prConfig.RepBonus} reputation, halt decay {(int)prConfig.HaltDecayDays} days.");
            if (prConfig.CooldownDays > 0)
                GUILayout.Label($"Cooldown: {(int)prConfig.CooldownDays} days before another campaign.");

            if (!canAfford)
                GUILayout.Label("<color=#ff8080><i>Not enough funds.</i></color>");
            else if (cooldownDays > 0)
                GUILayout.Label($"<color=#ff8080><i>On cooldown for {(int)System.Math.Ceiling(cooldownDays)} more days.</i></color>");

            GUILayout.BeginHorizontal();
            GUI.enabled = available;
            if (GUILayout.Button("Confirm"))
            {
                if (PrCampaignAction.Execute(prConfig, tier))
                    confirmingPr = false;
            }
            GUI.enabled = true;
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
