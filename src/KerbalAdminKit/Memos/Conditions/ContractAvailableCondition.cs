using System.Linq;
using Contracts;

namespace KerbalAdminKit.Memos.Conditions
{
    /// <summary>
    /// True when ContractSystem has at least Count contracts in Offered state whose
    /// type-name matches Group. Group null/empty = any.
    /// </summary>
    public sealed class ContractAvailableCondition : IMemoCondition
    {
        public string Group;
        public int Count = 1;

        public bool Evaluate(MemoContext ctx)
        {
            if (ContractSystem.Instance?.Contracts == null) return false;
            var matching = ContractSystem.Instance.Contracts
                .Where(c => c.ContractState == Contract.State.Offered);

            if (!string.IsNullOrEmpty(Group))
                matching = matching.Where(c => c.GetType().Name == Group);

            return matching.Count() >= Count;
        }
    }
}
