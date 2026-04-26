namespace KerbalAdminKit.Memos.Conditions
{
    /// <summary>
    /// Evaluates true when the current time is at least MinDays after the
    /// timestamp stored in flag "&lt;EventName&gt;_at". If the flag isn't set
    /// (event never happened), returns false. Day length = 21600s (Kerbin).
    /// </summary>
    public sealed class TimeSinceEventCondition : IMemoCondition
    {
        public string EventName;
        public double MinDays;

        private const double SecondsPerDay = 21600;

        public bool Evaluate(MemoContext ctx)
        {
            if (ctx?.Flags == null || string.IsNullOrEmpty(EventName)) return false;
            var flag = ctx.Flags.Get(EventName + "_at");
            if (string.IsNullOrEmpty(flag)) return false;
            if (!double.TryParse(flag, out var eventSeconds)) return false;
            var elapsedDays = (ctx.NowSeconds - eventSeconds) / SecondsPerDay;
            return elapsedDays >= MinDays;
        }
    }
}
