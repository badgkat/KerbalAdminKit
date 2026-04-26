using KerbalDialogueKit.Flags;

namespace KerbalAdminKit.Memos.Conditions
{
    public sealed class FlagExpressionCondition : IMemoCondition
    {
        public string Expression;

        public bool Evaluate(MemoContext ctx)
        {
            if (string.IsNullOrEmpty(Expression) || ctx?.Flags == null) return false;
            try
            {
                var expr = FlagExpressionParser.Parse(Expression);
                return expr.Evaluate(ctx.Flags);
            }
            catch { return false; }
        }
    }
}
