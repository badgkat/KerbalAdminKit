using UnityEngine;
using KerbalAdminKit.Characters;
using KerbalAdminKit.Focuses;
using KerbalCampaignKit.Core;
using KerbalDialogueKit.Core;

namespace KerbalAdminKit.Admin
{
    /// <summary>
    /// Center panel: program status (chapter, rep tier, income, next gate,
    /// active focuses).
    /// </summary>
    public sealed class DashboardPanel
    {
        private readonly CharacterRegistry characters;
        private readonly FocusRegistry focuses;
        private Vector2 scroll;

        public DashboardPanel(CharacterRegistry characters, FocusRegistry focuses)
        {
            this.characters = characters;
            this.focuses = focuses;
        }

        public void Draw(Rect area)
        {
            GUILayout.BeginArea(area, GUI.skin.box);
            scroll = GUILayout.BeginScrollView(scroll);

            GUILayout.Label("<b>Program Status</b>");
            DrawChapter();
            DrawReputation();
            DrawActiveFocuses();

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawChapter()
        {
            var chapter = CampaignKit.Chapters?.Current;
            GUILayout.Label($"Current Chapter: {chapter ?? "—"}");
        }

        private void DrawReputation()
        {
            var rep = global::Reputation.Instance != null ? global::Reputation.Instance.reputation : 0f;
            GUILayout.Label($"Reputation: {(int)rep}");

            var income = CampaignKit.Reputation?.Income;
            if (income != null && income.Enabled)
            {
                var monthly = income.IncomeForRep(rep);
                if (monthly > 0) GUILayout.Label($"Monthly Income: {(int)monthly}");

                var tierLabel = income.TierLabelForRep(rep);
                if (!string.IsNullOrEmpty(tierLabel)) GUILayout.Label($"Tier: {tierLabel}");
            }

            var nextGate = CampaignKit.Reputation?.NextGate(rep);
            if (nextGate.HasValue)
                GUILayout.Label($"Next: {nextGate.Value.label} at {(int)nextGate.Value.value}");
        }

        private void DrawActiveFocuses()
        {
            GUILayout.Space(8);
            GUILayout.Label("<b>Active Focuses</b>");

            bool any = false;
            foreach (var c in characters.All)
            {
                if (string.IsNullOrEmpty(c.DispositionFlag)) continue;
                var focusFlag = c.DispositionFlag.Replace("_disposition", "_focus");
                var value = DialogueKit.Flags?.Get(focusFlag);
                if (string.IsNullOrEmpty(value)) continue;

                FocusOption match = null;
                foreach (var f in focuses.GetForCharacter(c.Id))
                    if (f.FlagValue == value) { match = f; break; }
                if (match != null)
                {
                    GUILayout.Label($"{c.DisplayName}: {match.Title}");
                    any = true;
                }
            }
            if (!any) GUILayout.Label("<i>No focuses set. Click a character chip to assign one.</i>");
        }
    }
}
