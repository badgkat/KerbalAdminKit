using System.Linq;
using Xunit;
using KerbalAdminKit.Focuses;

namespace KerbalAdminKit.Tests
{
    public class FocusRegistryTests
    {
        [Fact]
        public void GetForCharacter_ReturnsMatches_IgnoresOthers()
        {
            var reg = new FocusRegistry();
            reg.Add(new FocusOption { CharacterId = "wernher", Id = "a" });
            reg.Add(new FocusOption { CharacterId = "wernher", Id = "b" });
            reg.Add(new FocusOption { CharacterId = "gene",    Id = "c" });

            var werns = reg.GetForCharacter("wernher").ToList();
            Assert.Equal(2, werns.Count);
            Assert.Contains(werns, f => f.Id == "a");
            Assert.Contains(werns, f => f.Id == "b");
        }

        [Fact]
        public void HasAnyFor_FalseWhenNone()
        {
            var reg = new FocusRegistry();
            Assert.False(reg.HasAnyFor("wernher"));
        }

        [Fact]
        public void HasAnyFor_TrueWhenPresent()
        {
            var reg = new FocusRegistry();
            reg.Add(new FocusOption { CharacterId = "wernher", Id = "a" });
            Assert.True(reg.HasAnyFor("wernher"));
        }
    }
}
