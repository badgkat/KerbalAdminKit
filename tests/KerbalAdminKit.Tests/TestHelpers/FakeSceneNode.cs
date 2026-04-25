using System.Collections.Generic;
using KerbalCampaignKit.Config;

namespace KerbalAdminKit.Tests.TestHelpers
{
    public sealed class FakeSceneNode : ISceneNode
    {
        public Dictionary<string, List<string>> Values = new Dictionary<string, List<string>>();
        public Dictionary<string, List<FakeSceneNode>> Nodes = new Dictionary<string, List<FakeSceneNode>>();

        public string GetValue(string key) =>
            Values.TryGetValue(key, out var list) && list.Count > 0 ? list[0] : null;

        public IEnumerable<string> GetValues(string key) =>
            Values.TryGetValue(key, out var list) ? list : new List<string>();

        public IEnumerable<ISceneNode> GetNodes(string name)
        {
            if (!Nodes.TryGetValue(name, out var list)) yield break;
            foreach (var n in list) yield return n;
        }

        public bool HasValue(string key) => Values.ContainsKey(key);
        public bool HasNode(string name) => Nodes.ContainsKey(name);

        public FakeSceneNode Set(string key, string value)
        {
            if (!Values.TryGetValue(key, out var list))
            {
                list = new List<string>();
                Values[key] = list;
            }
            list.Clear();
            list.Add(value);
            return this;
        }

        public FakeSceneNode Add(string key, string value)
        {
            if (!Values.TryGetValue(key, out var list))
            {
                list = new List<string>();
                Values[key] = list;
            }
            list.Add(value);
            return this;
        }

        public FakeSceneNode AddChild(string name, FakeSceneNode child)
        {
            if (!Nodes.TryGetValue(name, out var list))
            {
                list = new List<FakeSceneNode>();
                Nodes[name] = list;
            }
            list.Add(child);
            return this;
        }
    }
}
