namespace KerbalAdminKit.Memos.Conditions
{
    public sealed class InChapterCondition : IMemoCondition
    {
        public string Chapter;

        public bool Evaluate(MemoContext ctx) =>
            ctx != null && Chapter != null && Chapter == ctx.CurrentChapter;
    }
}
