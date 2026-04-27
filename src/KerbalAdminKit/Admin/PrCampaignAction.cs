using System.Globalization;
using KerbalCampaignKit.Config;
using KerbalCampaignKit.Core;

namespace KerbalAdminKit.Admin
{
    public sealed class PrCampaignConfig
    {
        public double BaseCost = 50000;
        public double TierCostMultiplier = 25000;
        public double HaltDecayDays = 60;
        public double RepBonus = 10;

        public static PrCampaignConfig Load(ISceneNode node)
        {
            var cfg = new PrCampaignConfig();
            if (node == null) return cfg;
            if (node.HasValue("baseCost")) cfg.BaseCost = D(node, "baseCost", cfg.BaseCost);
            if (node.HasValue("tierCostMultiplier")) cfg.TierCostMultiplier = D(node, "tierCostMultiplier", cfg.TierCostMultiplier);
            if (node.HasValue("haltDecayDays")) cfg.HaltDecayDays = D(node, "haltDecayDays", cfg.HaltDecayDays);
            if (node.HasValue("repBonus")) cfg.RepBonus = D(node, "repBonus", cfg.RepBonus);
            return cfg;
        }

        private static double D(ISceneNode n, string k, double fallback) =>
            double.TryParse(n.GetValue(k), NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : fallback;

        public double ComputeCost(int currentTierIndex) =>
            BaseCost + (currentTierIndex * TierCostMultiplier);
    }

    public static class PrCampaignAction
    {
        /// <summary>
        /// Applies the PR campaign: deducts funds, adds rep, halts decay,
        /// records lastUsedTime in the AdminKit scenario.
        /// </summary>
        public static void Execute(PrCampaignConfig cfg, int currentTierIndex)
        {
            if (cfg == null) return;
            var cost = cfg.ComputeCost(currentTierIndex);
            Funding.Instance?.AddFunds(-cost, TransactionReasons.Strategies);
            global::Reputation.Instance?.AddReputation((float)cfg.RepBonus, TransactionReasons.Strategies);
            CampaignKit.Reputation?.HaltDecay(Planetarium.GetUniversalTime(), cfg.HaltDecayDays);

            if (AdminKitScenario.Instance != null)
                AdminKitScenario.Instance.PrCampaignLastUsedTime = Planetarium.GetUniversalTime();
        }
    }
}
