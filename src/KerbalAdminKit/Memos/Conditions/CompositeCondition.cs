using System.Collections.Generic;

namespace KerbalAdminKit.Memos.Conditions
{
    public enum CompositeMode { All, Any }

    /// <summary>
    /// Combines child conditions with All (AND) or Any (OR) semantics.
    /// Empty All evaluates true (vacuous truth); empty Any evaluates false.
    /// Nestable.
    /// </summary>
    public sealed class CompositeCondition : IMemoCondition
    {
        public CompositeMode Mode = CompositeMode.All;
        public List<IMemoCondition> Children = new List<IMemoCondition>();

        public bool Evaluate(MemoContext ctx)
        {
            if (Mode == CompositeMode.All)
            {
                foreach (var c in Children)
                    if (!c.Evaluate(ctx)) return false;
                return true;
            }
            else
            {
                foreach (var c in Children)
                    if (c.Evaluate(ctx)) return true;
                return false;
            }
        }
    }
}
