using System.Globalization;
using KerbalAdminKit.Memos.Conditions;
using KerbalCampaignKit.Config;

namespace KerbalAdminKit.Memos
{
    public static class MemoLoader
    {
        public static Memo Load(ISceneNode node)
        {
            if (node == null) return null;
            var id = node.GetValue("id");
            if (string.IsNullOrEmpty(id)) return null;

            var m = new Memo
            {
                Id = id,
                CharacterId = node.GetValue("character") ?? "",
                Text = node.GetValue("text") ?? "",
                Priority = ParsePriority(node.GetValue("priority")),
            };

            if (node.HasValue("expireAfterDays") &&
                double.TryParse(node.GetValue("expireAfterDays"),
                    NumberStyles.Float, CultureInfo.InvariantCulture, out var exp))
                m.ExpireAfterDays = exp;

            if (node.HasValue("suppressAfterDismiss") &&
                bool.TryParse(node.GetValue("suppressAfterDismiss"), out var sup))
                m.SuppressAfterDismiss = sup;

            if (node.HasValue("postToStockMail") &&
                bool.TryParse(node.GetValue("postToStockMail"), out var psm))
                m.PostToStockMail = psm;

            foreach (var condNode in node.GetNodes("CONDITION"))
            {
                var cond = MemoConditionFactory.Build(condNode);
                if (cond != null) m.Conditions.Add(cond);
            }

            return m;
        }

        private static MemoPriority ParsePriority(string s)
        {
            if (string.IsNullOrEmpty(s)) return MemoPriority.Normal;
            switch (s.ToLowerInvariant())
            {
                case "low":    return MemoPriority.Low;
                case "high":   return MemoPriority.High;
                default:       return MemoPriority.Normal;
            }
        }
    }
}
