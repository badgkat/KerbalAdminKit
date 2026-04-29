using Xunit;
using KerbalAdminKit.Memos;
using KerbalAdminKit.Memos.Conditions;
using KerbalAdminKit.Tests.TestHelpers;

namespace KerbalAdminKit.Tests
{
    public class MemoLoaderTests
    {
        [Fact]
        public void Load_ParsesCoreFields()
        {
            var n = new FakeSceneNode()
                .Set("id", "relay_aging")
                .Set("character", "gus")
                .Set("priority", "high")
                .Set("text", "That relay is getting old.")
                .Set("expireAfterDays", "60")
                .Set("suppressAfterDismiss", "true")
                .Set("postToStockMail", "true");

            var m = MemoLoader.Load(n);

            Assert.Equal("relay_aging", m.Id);
            Assert.Equal("gus", m.CharacterId);
            Assert.Equal(MemoPriority.High, m.Priority);
            Assert.Equal("That relay is getting old.", m.Text);
            Assert.Equal(60.0, m.ExpireAfterDays);
            Assert.True(m.SuppressAfterDismiss);
            Assert.True(m.PostToStockMail);
        }

        [Fact]
        public void Load_ReturnsNullWhenIdMissing()
        {
            var n = new FakeSceneNode().Set("text", "no id");
            Assert.Null(MemoLoader.Load(n));
        }

        [Fact]
        public void Load_DefaultsPriorityToNormal()
        {
            var n = new FakeSceneNode().Set("id", "x").Set("text", "y");
            Assert.Equal(MemoPriority.Normal, MemoLoader.Load(n).Priority);
        }

        [Fact]
        public void Load_BuildsConditionsFromChildNodes()
        {
            var n = new FakeSceneNode().Set("id", "x").Set("text", "y");
            n.AddChild("CONDITION", new FakeSceneNode()
                .Set("type", "FundsBelow")
                .Set("threshold", "10000"));
            n.AddChild("CONDITION", new FakeSceneNode()
                .Set("type", "InChapter")
                .Set("chapter", "3"));

            var m = MemoLoader.Load(n);
            Assert.Equal(2, m.Conditions.Count);
            Assert.IsType<FundsBelowCondition>(m.Conditions[0]);
            Assert.Equal(10000.0, ((FundsBelowCondition)m.Conditions[0]).Threshold);
            Assert.IsType<InChapterCondition>(m.Conditions[1]);
        }

        [Fact]
        public void Load_SkipsUnknownConditionTypes()
        {
            var n = new FakeSceneNode().Set("id", "x").Set("text", "y");
            n.AddChild("CONDITION", new FakeSceneNode().Set("type", "NotAThing"));
            n.AddChild("CONDITION", new FakeSceneNode().Set("type", "InChapter").Set("chapter", "1"));

            var m = MemoLoader.Load(n);
            Assert.Single(m.Conditions);
            Assert.IsType<InChapterCondition>(m.Conditions[0]);
        }

        [Fact]
        public void Load_ParsesAllBlock()
        {
            var n = new FakeSceneNode().Set("id", "x").Set("text", "y");
            var all = new FakeSceneNode();
            all.AddChild("CONDITION", new FakeSceneNode()
                .Set("type", "FundsBelow").Set("threshold", "10000"));
            all.AddChild("CONDITION", new FakeSceneNode()
                .Set("type", "ChapterAtLeast").Set("chapter", "1"));
            n.AddChild("ALL", all);

            var m = MemoLoader.Load(n);
            Assert.Single(m.Conditions);
            var composite = Assert.IsType<CompositeCondition>(m.Conditions[0]);
            Assert.Equal(CompositeMode.All, composite.Mode);
            Assert.Equal(2, composite.Children.Count);
        }

        [Fact]
        public void Load_ParsesAnyBlock()
        {
            var n = new FakeSceneNode().Set("id", "x").Set("text", "y");
            var any = new FakeSceneNode();
            any.AddChild("CONDITION", new FakeSceneNode()
                .Set("type", "FundsBelow").Set("threshold", "10000"));
            any.AddChild("CONDITION", new FakeSceneNode()
                .Set("type", "ReputationAbove").Set("threshold", "50"));
            n.AddChild("ANY", any);

            var m = MemoLoader.Load(n);
            Assert.Single(m.Conditions);
            var composite = Assert.IsType<CompositeCondition>(m.Conditions[0]);
            Assert.Equal(CompositeMode.Any, composite.Mode);
            Assert.Equal(2, composite.Children.Count);
        }

        [Fact]
        public void Load_NestsCompositesRecursively()
        {
            var n = new FakeSceneNode().Set("id", "x").Set("text", "y");
            var outer = new FakeSceneNode();
            var inner = new FakeSceneNode();
            inner.AddChild("CONDITION", new FakeSceneNode()
                .Set("type", "FundsBelow").Set("threshold", "10000"));
            outer.AddChild("ANY", inner);
            outer.AddChild("CONDITION", new FakeSceneNode()
                .Set("type", "ChapterAtLeast").Set("chapter", "1"));
            n.AddChild("ALL", outer);

            var m = MemoLoader.Load(n);
            Assert.Single(m.Conditions);
            var topAll = Assert.IsType<CompositeCondition>(m.Conditions[0]);
            Assert.Equal(CompositeMode.All, topAll.Mode);
            Assert.Equal(2, topAll.Children.Count);

            var nestedAny = Assert.IsType<CompositeCondition>(topAll.Children[0]);
            Assert.Equal(CompositeMode.Any, nestedAny.Mode);
        }
    }
}
