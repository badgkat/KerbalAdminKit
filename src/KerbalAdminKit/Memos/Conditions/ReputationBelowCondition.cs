namespace KerbalAdminKit.Memos.Conditions
{
    public sealed class ReputationBelowCondition : IMemoCondition
    {
        public double Threshold;
        public bool Evaluate(MemoContext ctx) => ctx != null && ctx.Reputation < Threshold;
    }
}
