using System.Collections.Generic;
using System.Globalization;
using KerbalCampaignKit.Config;

namespace KerbalAdminKit.DecayCompiler
{
    public sealed class DispositionDecay
    {
        public string CharacterId;
        public double StepDays;
        public string TowardValue;
        public List<DispositionDecayStep> Steps = new List<DispositionDecayStep>();
    }

    public sealed class DispositionDecayStep
    {
        public string From;
        public string To;
    }

    public static class DispositionDecayLoader
    {
        public static DispositionDecay Load(ISceneNode node)
        {
            if (node == null) return null;
            var character = node.GetValue("character");
            if (string.IsNullOrEmpty(character)) return null;

            var d = new DispositionDecay { CharacterId = character };
            if (node.HasValue("stepDays") &&
                double.TryParse(node.GetValue("stepDays"),
                    NumberStyles.Float, CultureInfo.InvariantCulture, out var days))
                d.StepDays = days;
            d.TowardValue = node.GetValue("towardValue") ?? "Neutral";

            foreach (var stepNode in node.GetNodes("STEP"))
            {
                d.Steps.Add(new DispositionDecayStep
                {
                    From = stepNode.GetValue("from"),
                    To = stepNode.GetValue("to"),
                });
            }

            return d;
        }
    }
}
