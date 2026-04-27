using System.Collections.Generic;
using UnityEngine;
using ClickThroughFix;
using KerbalAdminKit.Admin;
using KerbalAdminKit.Characters;
using KerbalCampaignKit.Core;
using KerbalDialogueKit.Core;

namespace KerbalAdminKit.BuildingOverlays
{
    /// <summary>
    /// Top-right corner overlay listing one chip per BuildingScene matching
    /// this facility. Click → KDK scene. Renders a yellow outline if the
    /// character has a notification on this facility.
    /// </summary>
    public abstract class BuildingOverlayBase : MonoBehaviour
    {
        protected abstract string Facility { get; }
        protected abstract string NotificationPrefix { get; }
        private const int WindowId = 0x4B41_4B10;

        protected BuildingSceneRegistry SceneRegistry;
        protected CharacterRegistry Characters;

        private Rect window;
        private List<BuildingScene> myScenes;

        public void Initialize(BuildingSceneRegistry scenes, CharacterRegistry characters)
        {
            SceneRegistry = scenes;
            Characters = characters;
            myScenes = new List<BuildingScene>();
            foreach (var s in scenes.GetForFacility(Facility)) myScenes.Add(s);

            var w = 220f;
            var h = 80f + myScenes.Count * 80f;
            window = new Rect(Screen.width - w - 20, 60, w, h);
        }

        /// <summary>
        /// Override to gate rendering on whether the stock building UI is open.
        /// Default returns true (always render in this scene).
        /// </summary>
        protected virtual bool IsBuildingOpen() => true;

        private void OnGUI()
        {
            if (myScenes == null || myScenes.Count == 0) return;
            if (!IsBuildingOpen()) return;
            window = ClickThruBlocker.GUILayoutWindow(
                WindowId + GetInstanceID(), window, DrawWindow, Facility, GUI.skin.window);
        }

        private void DrawWindow(int id)
        {
            foreach (var scene in myScenes)
            {
                var character = Characters.Get(scene.CharacterId);
                if (character == null) continue;

                var chipHeight = 56f;
                var chipRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(chipHeight));
                var disposition = Characters.GetDisposition(scene.CharacterId);

                var overrideLayers = scene.ChipLayerOverrides.Count > 0
                    ? scene.ChipLayerOverrides
                    : null;

                ChipRenderer.Draw(chipRect, character, disposition, overrideLayers);

                if (HasNotification(scene.CharacterId)) DrawOutline(chipRect);

                if (GUI.Button(chipRect, GUIContent.none, GUIStyle.none))
                {
                    if (!string.IsNullOrEmpty(scene.OnClickScene))
                        DialogueKit.EnqueueById(scene.OnClickScene);
                }

                GUILayout.Label(character.DisplayName);
                GUILayout.Space(4);
            }

            GUI.DragWindow();
        }

        private bool HasNotification(string characterId)
        {
            var target = NotificationPrefix + ".character." + characterId;
            return CampaignKit.Notifications?.Highest(target).HasValue == true;
        }

        private void DrawOutline(Rect r)
        {
            var original = GUI.color;
            GUI.color = Color.yellow;
            GUI.Box(new Rect(r.x - 2, r.y - 2, r.width + 4, r.height + 4), GUIContent.none);
            GUI.color = original;
        }
    }
}
