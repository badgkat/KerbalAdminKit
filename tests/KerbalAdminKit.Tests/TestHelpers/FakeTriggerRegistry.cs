using System.Collections.Generic;
using KerbalAdminKit.DecayCompiler;

namespace KerbalAdminKit.Tests.TestHelpers
{
    /// <summary>
    /// Captures compiled triggers for assertions. Production wiring adapts
    /// the same ITriggerSink to a real KAK-owned ticker.
    /// </summary>
    public sealed class FakeTriggerRegistry : ITriggerSink
    {
        public List<CompiledDecayTrigger> Registered = new List<CompiledDecayTrigger>();
        public void Register(CompiledDecayTrigger t) => Registered.Add(t);
    }
}
