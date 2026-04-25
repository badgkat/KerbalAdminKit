using Xunit;
using KerbalAdminKit.Tests.TestHelpers;

namespace KerbalAdminKit.Tests
{
    public class SmokeTest
    {
        [Fact]
        public void AdminKit_Facade_IsReferenceable()
        {
            var t = typeof(AdminKit);
            Assert.NotNull(t);
        }

        [Fact]
        public void FakeSceneNode_RoundTripsValues()
        {
            var n = new FakeSceneNode().Set("key", "value");
            Assert.Equal("value", n.GetValue("key"));
            Assert.True(n.HasValue("key"));
            Assert.False(n.HasNode("whatever"));
        }
    }
}
