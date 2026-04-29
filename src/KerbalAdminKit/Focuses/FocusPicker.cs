using UnityEngine;
using ClickThroughFix;
using KerbalDialogueKit.Core;
using KerbalAdminKit.Characters;

namespace KerbalAdminKit.Focuses
{
    /// <summary>
    /// IMGUI modal listing valid focuses for a character. Selecting writes
    /// the focus flag and closes. Caller calls Show(character) to open.
    /// </summary>
    public sealed class FocusPicker
    {
        private readonly FocusRegistry focuses;
        private readonly CharacterRegistry characters;
        private CharacterInfo target;
        private Rect window = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 200, 400, 400);
        private Vector2 scroll;
        private const int WindowId = 0x4B41_4B01;

        private static Texture2D opaqueBg;

        public bool IsOpen => target != null;

        public FocusPicker(FocusRegistry focuses, CharacterRegistry characters)
        {
            this.focuses = focuses;
            this.characters = characters;
        }

        public void Show(CharacterInfo character) => target = character;
        public void Close() => target = null;

        public void OnGUI()
        {
            if (target == null) return;
            window = ClickThruBlocker.GUILayoutWindow(
                WindowId, window, DrawWindow, $"Focus: {target.DisplayName}", GUI.skin.window);
            // Keep the modal above the parent admin window every frame, otherwise
            // KSP's window stack can bury it once focus shifts to the parent.
            GUI.BringWindowToFront(WindowId);
        }

        private void DrawWindow(int id)
        {
            // Paint an opaque background so labels don't bleed through the
            // semi-transparent skin window onto the admin panel beneath.
            var fill = new Rect(0, 0, window.width, window.height);
            GUI.DrawTexture(fill, OpaqueBackground());

            scroll = GUILayout.BeginScrollView(scroll);
            foreach (var focus in focuses.GetValidFor(target.Id))
            {
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label($"<b>{focus.Title}</b>");
                if (!string.IsNullOrEmpty(focus.Description))
                    GUILayout.Label(focus.Description);
                if (GUILayout.Button("Select"))
                {
                    DialogueKit.Flags?.Set(focus.Flag ?? "", focus.FlagValue ?? "");
                    Close();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();

            if (GUILayout.Button("Cancel")) Close();
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
    }
}
