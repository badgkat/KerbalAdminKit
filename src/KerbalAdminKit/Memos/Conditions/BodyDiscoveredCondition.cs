namespace KerbalAdminKit.Memos.Conditions
{
    /// <summary>
    /// True when the named body exists in FlightGlobals. ResearchBodies integration
    /// is optional and degrades to "exists" when the assembly is absent.
    /// </summary>
    public sealed class BodyDiscoveredCondition : IMemoCondition
    {
        public string BodyName;

        public bool Evaluate(MemoContext ctx)
        {
            if (string.IsNullOrEmpty(BodyName)) return false;
            var body = FlightGlobals.Bodies?.Find(b => b.bodyName == BodyName);
            return body != null;
        }
    }
}
