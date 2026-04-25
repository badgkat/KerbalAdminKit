using KerbalDialogueKit.Flags;

namespace KerbalAdminKit.Memos
{
    /// <summary>
    /// Snapshot of game state at evaluation time. Passed to conditions.
    /// Built by MemoTicker before each poll; pure-logic conditions read from it.
    /// </summary>
    public sealed class MemoContext
    {
        public double NowSeconds;
        public double Funds;
        public double Reputation;
        public double Science;
        public string CurrentChapter;
        public FlagStore Flags;
    }
}
