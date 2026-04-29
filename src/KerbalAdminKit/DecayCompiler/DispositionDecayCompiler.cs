namespace KerbalAdminKit.DecayCompiler
{
    /// <summary>
    /// Pure data: a single "if flag X has value Y for N days, write Z to flag X" rule.
    /// Consumed at runtime by DispositionDecayTicker.
    /// </summary>
    public sealed class CompiledDecayTrigger
    {
        public string Id;
        public double StepDays;
        public string FlagToMatch;
        public string ExpectedFlagValue;
        public string TargetFlagValue;
    }

    public interface ITriggerSink
    {
        void Register(CompiledDecayTrigger trigger);
    }

    public static class DispositionDecayCompiler
    {
        public static void Compile(DispositionDecay decay, ITriggerSink sink)
        {
            if (decay == null || sink == null) return;
            if (string.IsNullOrEmpty(decay.CharacterId)) return;

            var flagName = decay.CharacterId + "_disposition";

            foreach (var step in decay.Steps)
            {
                if (string.IsNullOrEmpty(step?.From) || string.IsNullOrEmpty(step.To)) continue;
                sink.Register(new CompiledDecayTrigger
                {
                    Id = $"disposition_decay_{decay.CharacterId}_{step.From}_to_{step.To}",
                    StepDays = decay.StepDays,
                    FlagToMatch = flagName,
                    ExpectedFlagValue = step.From,
                    TargetFlagValue = step.To,
                });
            }
        }
    }
}
