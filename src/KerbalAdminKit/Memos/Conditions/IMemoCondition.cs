namespace KerbalAdminKit.Memos.Conditions
{
    public interface IMemoCondition
    {
        bool Evaluate(MemoContext ctx);
    }
}
