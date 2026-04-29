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

        private static Texture2D alertBg;
        private static GUIStyle alertStyle;

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

            if (HasUnselectedFocus(c.Id)) DrawAlertBadge(chipRect);

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

        private bool HasUnselectedFocus(string characterId)
        {
            if (!focuses.HasAnyFor(characterId)) return false;
            if (DialogueKit.Flags == null) return true;
            foreach (var f in focuses.GetForCharacter(characterId))
            {
                if (string.IsNullOrEmpty(f.Flag)) continue;
                if (!string.IsNullOrEmpty(DialogueKit.Flags.Get(f.Flag))) return false;
            }
            return true;
        }

        private static void DrawAlertBadge(Rect chipRect)
        {
            const float size = 20f;
            const float pad = 3f;
            var badge = new Rect(chipRect.xMax - size - pad, chipRect.y + pad, size, size);
            GUI.DrawTexture(badge, AlertBackground());
            GUI.Label(badge, "!", AlertStyle());
        }

        private static Texture2D AlertBackground()
        {
            if (alertBg == null)
            {
                alertBg = new Texture2D(1, 1);
                alertBg.SetPixel(0, 0, new Color(0.95f, 0.55f, 0.10f, 1f));
                alertBg.Apply();
            }
            return alertBg;
        }

        private static GUIStyle AlertStyle()
        {
            // Copy from skin.button to inherit center alignment and bold weight
            // without referencing TextAnchor/FontStyle directly, which would
            // require UnityEngine.TextRenderingModule and pull in
            // UnityEngine.CharacterInfo (collides with our CharacterInfo type).
            // Safe to call here: lazy init happens during OnGUI.
            if (alertStyle == null)
            {
                alertStyle = new GUIStyle(GUI.skin.button) { fontSize = 14 };
                alertStyle.normal.textColor = Color.white;
            }
            return alertStyle;
        }
    }
}
