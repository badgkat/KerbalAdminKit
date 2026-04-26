namespace KerbalAdminKit.Memos.Conditions
{
    public sealed class ReputationAboveCondition : IMemoCondition
    {
        public double Threshold;
        public bool Evaluate(MemoContext ctx) => ctx != null && ctx.Reputation > Threshold;
    }
}
