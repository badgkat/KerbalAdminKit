using System.Collections.Generic;
using KerbalDialogueKit.Core;

namespace KerbalAdminKit.Characters
{
    public sealed class CharacterRegistry
    {
        private readonly Dictionary<string, CharacterInfo> byId =
            new Dictionary<string, CharacterInfo>();

        public void Add(CharacterInfo c)
        {
            if (c == null || string.IsNullOrEmpty(c.Id)) return;
            byId[c.Id] = c;
        }

        public CharacterInfo Get(string id) =>
            (id != null && byId.TryGetValue(id, out var c)) ? c : null;

        public IEnumerable<CharacterInfo> All => byId.Values;
        public int Count => byId.Count;

        /// <summary>
        /// Reads disposition flag via KDK, falling back to DefaultDisposition.
        /// </summary>
        public string GetDisposition(string id)
        {
            var info = Get(id);
            if (info == null) return null;
            if (string.IsNullOrEmpty(info.DispositionFlag)) return info.DefaultDisposition;
            var value = DialogueKit.Flags?.Get(info.DispositionFlag);
            return string.IsNullOrEmpty(value) ? info.DefaultDisposition : value;
        }
    }
}
