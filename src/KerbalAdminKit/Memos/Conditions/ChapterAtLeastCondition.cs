using System.Globalization;

namespace KerbalAdminKit.Memos.Conditions
{
    /// <summary>
    /// True when the current chapter id is parseable as an integer
    /// and >= the configured threshold.
    /// </summary>
    public sealed class ChapterAtLeastCondition : IMemoCondition
    {
        public int Chapter;

        public bool Evaluate(MemoContext ctx)
        {
            if (string.IsNullOrEmpty(ctx.CurrentChapter)) return false;
            if (!int.TryParse(ctx.CurrentChapter, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var current)) return false;
            return current >= Chapter;
        }
    }
}
