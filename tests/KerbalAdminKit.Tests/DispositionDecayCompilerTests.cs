using Xunit;
using KerbalAdminKit.DecayCompiler;
using KerbalAdminKit.Tests.TestHelpers;

namespace KerbalAdminKit.Tests
{
    public class DispositionDecayCompilerTests
    {
        [Fact]
        public void Compile_ProducesOneTriggerPerStep()
        {
            var node = new FakeSceneNode()
                .Set("character", "wernher")
                .Set("stepDays", "180")
                .Set("towardValue", "Neutral");
            node.AddChild("STEP", new FakeSceneNode().Set("from", "Enthusiastic").Set("to", "Supportive"));
            node.AddChild("STEP", new FakeSceneNode().Set("from", "Supportive").Set("to", "Neutral"));

            var decay = DispositionDecayLoader.Load(node);
            var sink = new FakeTriggerRegistry();
            DispositionDecayCompiler.Compile(decay, sink);

            Assert.Equal(2, sink.Registered.Count);

            var t0 = sink.Registered[0];
            Assert.Equal("disposition_decay_wernher_Enthusiastic_to_Supportive", t0.Id);
            Assert.Equal("wernher_disposition", t0.FlagToMatch);
            Assert.Equal("Enthusiastic", t0.ExpectedFlagValue);
            Assert.Equal("Supportive", t0.TargetFlagValue);
            Assert.Equal(180.0, t0.StepDays);
        }

        [Fact]
        public void Compile_SkipsIncompleteSteps()
        {
            var node = new FakeSceneNode()
                .Set("character", "gus")
                .Set("stepDays", "120");
            node.AddChild("STEP", new FakeSceneNode().Set("from", "A"));
            node.AddChild("STEP", new FakeSceneNode().Set("to", "B"));
            node.AddChild("STEP", new FakeSceneNode().Set("from", "Frustrated").Set("to", "Skeptical"));

            var decay = DispositionDecayLoader.Load(node);
            var sink = new FakeTriggerRegistry();
            DispositionDecayCompiler.Compile(decay, sink);

            Assert.Single(sink.Registered);
            Assert.Equal("disposition_decay_gus_Frustrated_to_Skeptical", sink.Registered[0].Id);
        }

        [Fact]
        public void Load_ReturnsNullWhenCharacterMissing()
        {
            var node = new FakeSceneNode().Set("stepDays", "180");
            node.AddChild("STEP", new FakeSceneNode().Set("from", "X").Set("to", "Y"));
            Assert.Null(DispositionDecayLoader.Load(node));
        }
    }
}
