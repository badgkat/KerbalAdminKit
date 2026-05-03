using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KerbalAdminKit.Characters
{
    public static class InstructorPortraitRenderer
    {
        private static readonly Dictionary<string, Texture2D> cache =
            new Dictionary<string, Texture2D>();

        private static bool rendered;

        public static Texture2D Get(string characterId)
        {
            if (string.IsNullOrEmpty(characterId)) return null;
            cache.TryGetValue(characterId, out var tex);
            return tex;
        }

        public static IEnumerator RenderAll(IEnumerable<CharacterInfo> characters)
        {
            if (rendered) yield break;
            rendered = true;

            foreach (var c in characters)
            {
                if (string.IsNullOrEmpty(c.InstructorModel)) continue;
                if (cache.ContainsKey(c.Id)) continue;
                yield return RenderOne(c);
            }

            Debug.Log($"[KerbalAdminKit] Rendered {cache.Count} instructor portrait(s).");
        }

        private static IEnumerator RenderOne(CharacterInfo c)
        {
            var prefab = AssetBase.GetPrefab(c.InstructorModel);
            if (prefab == null)
            {
                Debug.LogWarning($"[KerbalAdminKit] Instructor prefab not found: {c.InstructorModel}");
                yield break;
            }

            var go = Object.Instantiate(prefab);
            go.transform.position = new Vector3(0, -500, 0);
            go.SetActive(true);

            var instructor = go.GetComponent<KerbalInstructor>();
            if (instructor == null || instructor.instructorCamera == null)
            {
                Debug.LogWarning($"[KerbalAdminKit] No KerbalInstructor/camera on prefab: {c.InstructorModel}");
                Object.Destroy(go);
                yield break;
            }

            yield return null;
            yield return null;

            const int size = 128;
            var rt = new RenderTexture(size, size, 24);
            var cam = instructor.instructorCamera;
            cam.targetTexture = rt;
            cam.aspect = 1f;
            cam.Render();

            var prev = RenderTexture.active;
            RenderTexture.active = rt;
            var tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
            tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
            tex.Apply();
            RenderTexture.active = prev;

            cache[c.Id] = tex;

            cam.targetTexture = null;
            rt.Release();
            Object.Destroy(rt);
            Object.Destroy(go);
        }
    }
}
