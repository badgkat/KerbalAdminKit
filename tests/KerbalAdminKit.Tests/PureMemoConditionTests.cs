using Xunit;
using KerbalAdminKit.Memos;
using KerbalAdminKit.Memos.Conditions;
using KerbalDialogueKit.Flags;

namespace KerbalAdminKit.Tests
{
    public class PureMemoConditionTests
    {
        private MemoContext Ctx(double funds = 0, double rep = 0, string chapter = null, FlagStore flags = null, double now = 0)
            => new MemoContext { Funds = funds, Reputation = rep, CurrentChapter = chapter, Flags = flags ?? new FlagStore(), NowSeconds = now };

        [Fact]
        public void FundsBelow_True_WhenFundsLessThanThreshold()
        {
            var c = new FundsBelowCondition { Threshold = 10000 };
            Assert.True(c.Evaluate(Ctx(funds: 9999)));
        }

        [Fact]
        public void FundsBelow_False_WhenFundsEqualOrAbove()
        {
            var c = new FundsBelowCondition { Threshold = 10000 };
            Assert.False(c.Evaluate(Ctx(funds: 10000)));
            Assert.False(c.Evaluate(Ctx(funds: 10001)));
        }

        [Fact]
        public void ReputationAbove_True_WhenRepExceedsThreshold()
        {
            var c = new ReputationAboveCondition { Threshold = 250 };
            Assert.True(c.Evaluate(Ctx(rep: 251)));
            Assert.False(c.Evaluate(Ctx(rep: 250)));
        }

        [Fact]
        public void ReputationBelow_True_WhenRepUnderThreshold()
        {
            var c = new ReputationBelowCondition { Threshold = 100 };
            Assert.True(c.Evaluate(Ctx(rep: 99)));
            Assert.False(c.Evaluate(Ctx(rep: 100)));
        }

        [Fact]
        public void InChapter_True_WhenCurrentMatches()
        {
            var c = new InChapterCondition { Chapter = "3" };
            Assert.True(c.Evaluate(Ctx(chapter: "3")));
            Assert.False(c.Evaluate(Ctx(chapter: "2")));
            Assert.False(c.Evaluate(Ctx(chapter: null)));
        }

        [Fact]
        public void ChapterAtLeast_True_WhenCurrentMatches()
        {
            var c = new ChapterAtLeastCondition { Chapter = 1 };
            Assert.True(c.Evaluate(Ctx(chapter: "1")));
        }

        [Fact]
        public void ChapterAtLeast_True_WhenCurrentExceeds()
        {
            var c = new ChapterAtLeastCondition { Chapter = 1 };
            Assert.True(c.Evaluate(Ctx(chapter: "2")));
            Assert.True(c.Evaluate(Ctx(chapter: "5")));
        }

        [Fact]
        public void ChapterAtLeast_False_WhenCurrentBelow()
        {
            var c = new ChapterAtLeastCondition { Chapter = 2 };
            Assert.False(c.Evaluate(Ctx(chapter: "1")));
            Assert.False(c.Evaluate(Ctx(chapter: "0")));
        }

        [Fact]
        public void ChapterAtLeast_False_WhenCurrentNullOrUnparseable()
        {
            var c = new ChapterAtLeastCondition { Chapter = 1 };
            Assert.False(c.Evaluate(Ctx(chapter: null)));
            Assert.False(c.Evaluate(Ctx(chapter: "not-a-number")));
        }

        [Fact]
        public void FlagExpression_ParsesAndEvaluates()
        {
            var flags = new FlagStore();
            flags.Set("completed_first_anomaly", "true");
            flags.Set("chapter", "3");

            var c = new FlagExpressionCondition { Expression = "completed_first_anomaly == true" };
            Assert.True(c.Evaluate(Ctx(flags: flags)));

            var c2 = new FlagExpressionCondition { Expression = "chapter >= 4" };
            Assert.False(c2.Evaluate(Ctx(flags: flags)));
        }

        [Fact]
        public void TimeSinceEvent_True_WhenEventFlagOlderThanMinDays()
        {
            var flags = new FlagStore();
            // event time stored as seconds in a flag named "<event>_at"
            flags.Set("first_orbit_at", "0");           // elapsed: now - 0

            double secondsPerDay = 21600;               // KSP Kerbin day
            double now = secondsPerDay * 10;            // 10 days later

            var c = new TimeSinceEventCondition { EventName = "first_orbit", MinDays = 5 };
            Assert.True(c.Evaluate(Ctx(flags: flags, now: now)));

            var c2 = new TimeSinceEventCondition { EventName = "first_orbit", MinDays = 20 };
            Assert.False(c2.Evaluate(Ctx(flags: flags, now: now)));
        }

        [Fact]
        public void TimeSinceEvent_False_WhenEventMissing()
        {
            var flags = new FlagStore();
            var c = new TimeSinceEventCondition { EventName = "never_happened", MinDays = 1 };
            Assert.False(c.Evaluate(Ctx(flags: flags, now: 1e9)));
        }

        [Fact]
        public void All_True_WhenAllChildrenTrue()
        {
            var c = new CompositeCondition
            {
                Mode = CompositeMode.All,
                Children =
                {
                    new FundsBelowCondition { Threshold = 10000 },
                    new ReputationAboveCondition { Threshold = 50 },
                },
            };
            Assert.True(c.Evaluate(Ctx(funds: 5000, rep: 100)));
        }

        [Fact]
        public void All_False_WhenAnyChildFalse()
        {
            var c = new CompositeCondition
            {
                Mode = CompositeMode.All,
                Children =
                {
                    new FundsBelowCondition { Threshold = 10000 },
                    new ReputationAboveCondition { Threshold = 50 },
                },
            };
            Assert.False(c.Evaluate(Ctx(funds: 5000, rep: 10)));
            Assert.False(c.Evaluate(Ctx(funds: 50000, rep: 100)));
        }

        [Fact]
        public void All_True_WhenChildrenEmpty()
        {
            var c = new CompositeCondition { Mode = CompositeMode.All };
            Assert.True(c.Evaluate(Ctx()));
        }

        [Fact]
        public void Any_True_WhenAnyChildTrue()
        {
            var c = new CompositeCondition
            {
                Mode = CompositeMode.Any,
                Children =
                {
                    new FundsBelowCondition { Threshold = 10000 },
                    new ReputationAboveCondition { Threshold = 50 },
                },
            };
            Assert.True(c.Evaluate(Ctx(funds: 5000, rep: 0)));
            Assert.True(c.Evaluate(Ctx(funds: 50000, rep: 100)));
        }

        [Fact]
        public void Any_False_WhenAllChildrenFalse()
        {
            var c = new CompositeCondition
            {
                Mode = CompositeMode.Any,
                Children =
                {
                    new FundsBelowCondition { Threshold = 10000 },
                    new ReputationAboveCondition { Threshold = 50 },
                },
            };
            Assert.False(c.Evaluate(Ctx(funds: 50000, rep: 10)));
        }

        [Fact]
        public void Any_False_WhenChildrenEmpty()
        {
            var c = new CompositeCondition { Mode = CompositeMode.Any };
            Assert.False(c.Evaluate(Ctx()));
        }

        [Fact]
        public void Composite_Nests()
        {
            // ALL { ANY { funds<10k, rep>50 }, chapter>=1 }
            var c = new CompositeCondition
            {
                Mode = CompositeMode.All,
                Children =
                {
                    new CompositeCondition
                    {
                        Mode = CompositeMode.Any,
                        Children =
                        {
                            new FundsBelowCondition { Threshold = 10000 },
                            new ReputationAboveCondition { Threshold = 50 },
                        },
                    },
                    new ChapterAtLeastCondition { Chapter = 1 },
                },
            };
            Assert.True(c.Evaluate(Ctx(funds: 5000, rep: 0, chapter: "1")));
            Assert.False(c.Evaluate(Ctx(funds: 5000, rep: 0, chapter: "0")));
            Assert.False(c.Evaluate(Ctx(funds: 50000, rep: 10, chapter: "1")));
        }
    }
}
