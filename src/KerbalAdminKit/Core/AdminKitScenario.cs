using System.Collections.Generic;

namespace KerbalAdminKit
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames,
        GameScenes.SPACECENTER, GameScenes.FLIGHT,
        GameScenes.TRACKSTATION, GameScenes.EDITOR)]
    public class AdminKitScenario : ScenarioModule
    {
        public static AdminKitScenario Instance;

        public Dictionary<string, double> DismissedMemos = new Dictionary<string, double>();
        public HashSet<string> StockMailedMemos = new HashSet<string>();
        public double PrCampaignLastUsedTime;

        public override void OnAwake()
        {
            base.OnAwake();
            Instance = this;
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            var dismissed = node.AddNode("DISMISSED_MEMOS");
            foreach (var kv in DismissedMemos)
            {
                var m = dismissed.AddNode("MEMO");
                m.AddValue("id", kv.Key);
                m.AddValue("dismissedAt", kv.Value);
            }

            var mailed = node.AddNode("STOCK_MAILED_MEMOS");
            foreach (var id in StockMailedMemos)
            {
                var m = mailed.AddNode("MEMO");
                m.AddValue("id", id);
            }

            var pr = node.AddNode("PR_CAMPAIGN");
            pr.AddValue("lastUsedTime", PrCampaignLastUsedTime);
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            DismissedMemos.Clear();
            StockMailedMemos.Clear();
            PrCampaignLastUsedTime = 0;

            if (node.HasNode("DISMISSED_MEMOS"))
            {
                foreach (var m in node.GetNode("DISMISSED_MEMOS").GetNodes("MEMO"))
                {
                    var id = m.GetValue("id");
                    if (string.IsNullOrEmpty(id)) continue;
                    double.TryParse(m.GetValue("dismissedAt"), out var at);
                    DismissedMemos[id] = at;
                }
            }

            if (node.HasNode("STOCK_MAILED_MEMOS"))
            {
                foreach (var m in node.GetNode("STOCK_MAILED_MEMOS").GetNodes("MEMO"))
                {
                    var id = m.GetValue("id");
                    if (!string.IsNullOrEmpty(id)) StockMailedMemos.Add(id);
                }
            }

            if (node.HasNode("PR_CAMPAIGN"))
            {
                double.TryParse(node.GetNode("PR_CAMPAIGN").GetValue("lastUsedTime"),
                    out PrCampaignLastUsedTime);
            }
        }
    }
}
