using System.Collections.Generic;
using System.Linq;

namespace KerbalAdminKit.Memos
{
    public sealed class MemoRegistry
    {
        private readonly List<Memo> all = new List<Memo>();

        public void Add(Memo m) { if (m != null) all.Add(m); }

        public IEnumerable<Memo> All => all;

        public IEnumerable<Memo> Active =>
            all.Where(m => m.IsActive)
               .OrderByDescending(m => (int)m.Priority)
               .ThenBy(m => m.ActivatedAtSeconds);

        public IEnumerable<Memo> Pinned =>
            all.Where(m => m.IsActive && m.Priority == MemoPriority.High);

        public Memo Get(string id) => all.FirstOrDefault(m => m.Id == id);

        /// <summary>
        /// Marks a memo dismissed and records dismissal time in AdminKitScenario.
        /// SuppressAfterDismiss memos won't re-activate this save. Otherwise the
        /// memo enters rearm-pending: it stays silent until its condition goes
        /// false at least once, then may fire again on the next true edge.
        /// </summary>
        public void Dismiss(string id, double nowSeconds)
        {
            var m = Get(id);
            if (m == null) return;
            m.IsActive = false;
            if (AdminKitScenario.Instance == null) return;
            AdminKitScenario.Instance.DismissedMemos[id] = nowSeconds;
            if (!m.SuppressAfterDismiss)
                AdminKitScenario.Instance.RearmPendingMemos.Add(id);
        }
    }
}
