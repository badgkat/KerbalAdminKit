namespace KerbalAdminKit.Memos.Conditions
{
    public sealed class FundsBelowCondition : IMemoCondition
    {
        public double Threshold;
        public bool Evaluate(MemoContext ctx) => ctx != null && ctx.Funds < Threshold;
    }
}
