using UnityEngine;
using KerbalDialogueKit.Core;
using KSP.UI.Screens;

namespace KerbalAdminKit.Memos
{
    /// <summary>
    /// Polls memo conditions while at KSC and flips IsActive accordingly.
    /// Fires stock-mail delivery on first-time activation when PostToStockMail.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public sealed class MemoTicker : MonoBehaviour
    {
        private const double SecondsPerDay = 21600;

        private MemoRegistry registry;
        private double pollSeconds = 5.0;
        private double nextPoll;

        public void Initialize(MemoRegistry reg, double pollInterval)
        {
            registry = reg;
            pollSeconds = pollInterval;
        }

        private void FixedUpdate()
        {
            if (registry == null) return;
            var now = Planetarium.GetUniversalTime();
            if (now < nextPoll) return;
            nextPoll = now + pollSeconds;

            var ctx = BuildContext(now);
            foreach (var memo in registry.All) EvaluateMemo(memo, ctx, now);
        }

        private MemoContext BuildContext(double now)
        {
            return new MemoContext
            {
                NowSeconds = now,
                Funds = Funding.Instance != null ? Funding.Instance.Funds : 0,
                Reputation = global::Reputation.Instance != null ? global::Reputation.Instance.reputation : 0,
                Science = ResearchAndDevelopment.Instance != null ? ResearchAndDevelopment.Instance.Science : 0,
                CurrentChapter = KerbalCampaignKit.Core.CampaignKit.Chapters?.Current,
                Flags = DialogueKit.Flags,
            };
        }

        private void EvaluateMemo(Memo memo, MemoContext ctx, double now)
        {
            // Permanently dismissed this save
            if (memo.SuppressAfterDismiss &&
                AdminKitScenario.Instance != null &&
                AdminKitScenario.Instance.DismissedMemos.ContainsKey(memo.Id))
            {
                memo.IsActive = false;
                return;
            }

            // Expiry: was active, ExpireAfterDays elapsed since activation
            if (memo.IsActive && memo.ExpireAfterDays.HasValue)
            {
                var activeDays = (now - memo.ActivatedAtSeconds) / SecondsPerDay;
                if (activeDays > memo.ExpireAfterDays.Value)
                {
                    memo.IsActive = false;
                    return;
                }
            }

            // AND of conditions; empty list = always true
            bool allPass = memo.Conditions.Count == 0;
            if (!allPass)
            {
                allPass = true;
                foreach (var cond in memo.Conditions)
                {
                    if (!cond.Evaluate(ctx)) { allPass = false; break; }
                }
            }

            if (allPass && !memo.IsActive)
            {
                memo.IsActive = true;
                memo.ActivatedAtSeconds = now;
                PostToStockMailIfNeeded(memo);
            }
            else if (!allPass && memo.IsActive)
            {
                memo.IsActive = false;
            }
        }

        private void PostToStockMailIfNeeded(Memo memo)
        {
            if (!memo.PostToStockMail) return;
            if (AdminKitScenario.Instance == null) return;
            if (AdminKitScenario.Instance.StockMailedMemos.Contains(memo.Id)) return;

            if (MessageSystem.Instance != null)
            {
                var title = !string.IsNullOrEmpty(memo.CharacterId)
                    ? $"Memo from {memo.CharacterId}"
                    : "Program Memo";
                var msg = new MessageSystem.Message(
                    title,
                    memo.Text ?? "",
                    MessageSystemButton.MessageButtonColor.GREEN,
                    MessageSystemButton.ButtonIcons.MESSAGE);
                MessageSystem.Instance.AddMessage(msg);
            }
            AdminKitScenario.Instance.StockMailedMemos.Add(memo.Id);
        }
    }
}
