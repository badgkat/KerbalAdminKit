using System.Collections.Generic;
using System.Linq;
using KerbalDialogueKit.Core;
using KerbalDialogueKit.Flags;

namespace KerbalAdminKit.Focuses
{
    public sealed class FocusRegistry
    {
        private readonly List<FocusOption> options = new List<FocusOption>();

        public void Add(FocusOption option)
        {
            if (option != null) options.Add(option);
        }

        public IEnumerable<FocusOption> All => options;

        public IEnumerable<FocusOption> GetForCharacter(string characterId) =>
            options.Where(o => o.CharacterId == characterId);

        public bool HasAnyFor(string characterId) =>
            options.Any(o => o.CharacterId == characterId);

        /// <summary>
        /// Returns focuses for character whose Requirement expression evaluates
        /// to true against the current flag store (null requirement = always valid).
        /// Used by the picker UI.
        /// </summary>
        public IEnumerable<FocusOption> GetValidFor(string characterId)
        {
            var flags = DialogueKit.Flags;
            foreach (var opt in GetForCharacter(characterId))
            {
                if (string.IsNullOrEmpty(opt.Requirement)) { yield return opt; continue; }
                bool valid;
                try
                {
                    var expr = FlagExpressionParser.Parse(opt.Requirement);
                    valid = expr.Evaluate(flags);
                }
                catch
                {
                    UnityEngine.Debug.LogWarning(
                        $"[KerbalAdminKit] Invalid focus requirement on {opt.Id}: {opt.Requirement}");
                    valid = false;
                }
                if (valid) yield return opt;
            }
        }
    }
}
