using System.Collections.Generic;
using UnityEngine;
using KerbalDialogueKit.Core;

namespace KerbalAdminKit.DecayCompiler
{
    /// <summary>
    /// Polls compiled decay triggers at KSC. For each trigger whose flag
    /// matches ExpectedFlagValue and whose flag has been set at least
    /// StepDays ago, sets the flag to TargetFlagValue. Tracks the last-set
    /// time via a companion flag "&lt;flag&gt;_last_set_at". If no timestamp
    /// is present, stamps now and waits for the next cycle.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public sealed class DispositionDecayTicker : MonoBehaviour
    {
        private const double SecondsPerDay = 21600;
        private const double PollSeconds = 10.0;

        private List<CompiledDecayTrigger> triggers = new List<CompiledDecayTrigger>();
        private double nextPoll;

        public void Initialize(IEnumerable<CompiledDecayTrigger> compiled)
        {
            triggers.Clear();
            if (compiled == null) return;
            foreach (var t in compiled) triggers.Add(t);
        }

        private void FixedUpdate()
        {
            if (triggers.Count == 0) return;
            var now = Planetarium.GetUniversalTime();
            if (now < nextPoll) return;
            nextPoll = now + PollSeconds;

            var flags = DialogueKit.Flags;
            if (flags == null) return;

            foreach (var t in triggers)
            {
                var current = flags.Get(t.FlagToMatch);
                if (current != t.ExpectedFlagValue) continue;

                var lastSetKey = t.FlagToMatch + "_last_set_at";
                var lastSetStr = flags.Get(lastSetKey);
                if (!double.TryParse(lastSetStr, out var lastSet))
                {
                    flags.Set(lastSetKey, now.ToString("F1"));
                    continue;
                }

                var elapsedDays = (now - lastSet) / SecondsPerDay;
                if (elapsedDays < t.StepDays) continue;

                flags.Set(t.FlagToMatch, t.TargetFlagValue);
                flags.Set(lastSetKey, now.ToString("F1"));
                Debug.Log($"[KerbalAdminKit] Decay {t.Id}: {t.ExpectedFlagValue} → {t.TargetFlagValue}");
            }
        }
    }
}
