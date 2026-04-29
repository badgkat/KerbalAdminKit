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
        public double CooldownDays = 30;

        public static PrCampaignConfig Load(ISceneNode node)
        {
            var cfg = new PrCampaignConfig();
            if (node == null) return cfg;
            if (node.HasValue("baseCost")) cfg.BaseCost = D(node, "baseCost", cfg.BaseCost);
            if (node.HasValue("tierCostMultiplier")) cfg.TierCostMultiplier = D(node, "tierCostMultiplier", cfg.TierCostMultiplier);
            if (node.HasValue("haltDecayDays")) cfg.HaltDecayDays = D(node, "haltDecayDays", cfg.HaltDecayDays);
            if (node.HasValue("repBonus")) cfg.RepBonus = D(node, "repBonus", cfg.RepBonus);
            if (node.HasValue("cooldownDays")) cfg.CooldownDays = D(node, "cooldownDays", cfg.CooldownDays);
            return cfg;
        }

        private static double D(ISceneNode n, string k, double fallback) =>
            double.TryParse(n.GetValue(k), NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : fallback;

        public double ComputeCost(int currentTierIndex) =>
            BaseCost + (currentTierIndex * TierCostMultiplier);
    }

    public static class PrCampaignAction
    {
        private const double SecondsPerDay = 21600;

        public static bool CanAfford(PrCampaignConfig cfg, int currentTierIndex)
        {
            if (cfg == null) return false;
            var cost = cfg.ComputeCost(currentTierIndex);
            var funds = Funding.Instance != null ? Funding.Instance.Funds : 0;
            return funds >= cost;
        }

        /// <summary>
        /// Days remaining on the cooldown, or 0 if available now.
        /// </summary>
        public static double CooldownRemainingDays(PrCampaignConfig cfg)
        {
            if (cfg == null || cfg.CooldownDays <= 0) return 0;
            if (AdminKitScenario.Instance == null) return 0;
            var lastUsed = AdminKitScenario.Instance.PrCampaignLastUsedTime;
            if (lastUsed <= 0) return 0;
            var now = Planetarium.GetUniversalTime();
            var elapsedDays = (now - lastUsed) / SecondsPerDay;
            var remaining = cfg.CooldownDays - elapsedDays;
            return remaining > 0 ? remaining : 0;
        }

        public static bool IsAvailable(PrCampaignConfig cfg, int currentTierIndex) =>
            CanAfford(cfg, currentTierIndex) && CooldownRemainingDays(cfg) <= 0;

        /// <summary>
        /// Applies the PR campaign: deducts funds, adds rep, halts decay, records
        /// lastUsedTime. Returns false (and does nothing) if funds are insufficient
        /// or the cooldown is still active.
        /// </summary>
        public static bool Execute(PrCampaignConfig cfg, int currentTierIndex)
        {
            if (cfg == null) return false;
            if (!IsAvailable(cfg, currentTierIndex)) return false;

            var cost = cfg.ComputeCost(currentTierIndex);
            Funding.Instance?.AddFunds(-cost, TransactionReasons.Strategies);
            global::Reputation.Instance?.AddReputation((float)cfg.RepBonus, TransactionReasons.Strategies);
            CampaignKit.Reputation?.HaltDecay(Planetarium.GetUniversalTime(), cfg.HaltDecayDays);

            if (AdminKitScenario.Instance != null)
                AdminKitScenario.Instance.PrCampaignLastUsedTime = Planetarium.GetUniversalTime();

            return true;
        }
    }
}
