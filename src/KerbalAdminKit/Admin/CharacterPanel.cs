using UnityEngine;
using KerbalAdminKit.Characters;
using KerbalAdminKit.Focuses;
using KerbalDialogueKit.Core;

namespace KerbalAdminKit.Admin
{
    /// <summary>
    /// Left-column character chips + text block. Click opens the focus picker,
    /// or fires onClickScene if no focuses exist for the character.
    /// </summary>
    public sealed class CharacterPanel
    {
        private readonly CharacterRegistry characters;
        private readonly FocusRegistry focuses;
        private readonly FocusPicker picker;
        private Vector2 scroll;

        public CharacterPanel(CharacterRegistry characters, FocusRegistry focuses, FocusPicker picker)
        {
            this.characters = characters;
            this.focuses = focuses;
            this.picker = picker;
        }

        public void Draw(Rect area)
        {
            GUILayout.BeginArea(area, GUI.skin.box);
            scroll = GUILayout.BeginScrollView(scroll);

            foreach (var c in characters.All)
            {
                DrawChip(c);
                GUILayout.Space(8);
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawChip(CharacterInfo c)
        {
            GUILayout.BeginVertical(GUI.skin.box);

            var chipHeight = 64f;
            var chipRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(chipHeight));
            var disposition = characters.GetDisposition(c.Id);
            ChipRenderer.Draw(chipRect, c, disposition);

            if (GUI.Button(chipRect, GUIContent.none, GUIStyle.none))
                OnChipClicked(c);

            GUILayout.Label($"<b>{c.DisplayName}</b>");
            if (!string.IsNullOrEmpty(c.Role)) GUILayout.Label(c.Role);
            GUILayout.Label($"— {DispositionColors.ResolveLabel(c, disposition) ?? "—"}");

            var focusFlag = c.DispositionFlag?.Replace("_disposition", "_focus");
            if (!string.IsNullOrEmpty(focusFlag))
            {
                var focusValue = DialogueKit.Flags?.Get(focusFlag);
                if (!string.IsNullOrEmpty(focusValue))
                {
                    var focus = FindFocus(c.Id, focusValue);
                    if (focus != null) GUILayout.Label($"Focus: {focus.Title}");
                }
            }

            GUILayout.EndVertical();
        }

        private FocusOption FindFocus(string characterId, string flagValue)
        {
            foreach (var f in focuses.GetForCharacter(characterId))
                if (f.FlagValue == flagValue) return f;
            return null;
        }

        private void OnChipClicked(CharacterInfo c)
        {
            if (focuses.HasAnyFor(c.Id)) { picker.Show(c); return; }
            if (!string.IsNullOrEmpty(c.OnClickScene))
                DialogueKit.EnqueueById(c.OnClickScene);
        }
    }
}
