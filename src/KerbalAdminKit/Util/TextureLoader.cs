using System.Collections.Generic;
using UnityEngine;

namespace KerbalAdminKit.Util
{
    /// <summary>
    /// Caches GameDatabase texture lookups. Returns null if not found;
    /// callers render a fallback (color swatch) when null.
    /// </summary>
    public static class TextureLoader
    {
        private static readonly Dictionary<string, Texture2D> cache =
            new Dictionary<string, Texture2D>();

        public static Texture2D Get(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            if (cache.TryGetValue(url, out var tex)) return tex;

            var loaded = GameDatabase.Instance?.GetTexture(url, false);
            cache[url] = loaded;
            if (loaded == null)
                Debug.LogWarning($"[KerbalAdminKit] Texture not found: {url}");
            return loaded;
        }

        public static void ClearCache() => cache.Clear();
    }
}
