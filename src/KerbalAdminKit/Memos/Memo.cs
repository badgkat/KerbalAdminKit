using System.Collections.Generic;
using KerbalAdminKit.Memos.Conditions;

namespace KerbalAdminKit.Memos
{
    public enum MemoPriority { Low, Normal, High }

    public sealed class Memo
    {
        public string Id;
        public string CharacterId;
        public MemoPriority Priority = MemoPriority.Normal;
        public string Text;

        public List<IMemoCondition> Conditions = new List<IMemoCondition>();

        public double? ExpireAfterDays;   // null = never
        public bool SuppressAfterDismiss;
        public bool PostToStockMail;

        // Runtime state (not persisted):
        public bool IsActive;
        public double ActivatedAtSeconds;
    }
}
