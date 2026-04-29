using System.Globalization;
using KerbalCampaignKit.Config;

namespace KerbalAdminKit.Memos.Conditions
{
    /// <summary>
    /// Builds an IMemoCondition from a CONDITION cfg node. Returns null on
    /// unknown type or missing fields (loader logs and skips).
    /// </summary>
    public static class MemoConditionFactory
    {
        public static IMemoCondition Build(ISceneNode node)
        {
            if (node == null) return null;
            var type = node.GetValue("type");
            if (string.IsNullOrEmpty(type)) return null;

            switch (type)
            {
                case "FundsBelow":
                    return new FundsBelowCondition { Threshold = D(node, "threshold") };
                case "ReputationAbove":
                    return new ReputationAboveCondition { Threshold = D(node, "threshold") };
                case "ReputationBelow":
                    return new ReputationBelowCondition { Threshold = D(node, "threshold") };
                case "InChapter":
                    return new InChapterCondition { Chapter = node.GetValue("chapter") };
                case "ChapterAtLeast":
                    return new ChapterAtLeastCondition { Chapter = (int)D(node, "chapter") };
                case "FlagExpression":
                    return new FlagExpressionCondition { Expression = node.GetValue("expression") };
                case "TimeSinceEvent":
                    return new TimeSinceEventCondition
                    {
                        EventName = node.GetValue("event"),
                        MinDays = D(node, "minDays"),
                    };
                case "VesselAge":
                    return new VesselAgeCondition
                    {
                        VesselType = node.GetValue("vesselType"),
                        MinDays = D(node, "minDays"),
                        MaxDays = OptD(node, "maxDays"),
                    };
                case "ContractAvailable":
                    return new ContractAvailableCondition
                    {
                        Group = node.GetValue("group"),
                        Count = (int)D(node, "count", 1),
                    };
                case "BodyDiscovered":
                    return new BodyDiscoveredCondition { BodyName = node.GetValue("body") };
                default:
                    return null;
            }
        }

        private static double D(ISceneNode n, string k, double fallback = 0)
            => double.TryParse(n.GetValue(k), NumberStyles.Float,
                CultureInfo.InvariantCulture, out var v) ? v : fallback;

        private static double? OptD(ISceneNode n, string k)
            => n.HasValue(k) && double.TryParse(n.GetValue(k), NumberStyles.Float,
                CultureInfo.InvariantCulture, out var v) ? (double?)v : null;
    }
}
