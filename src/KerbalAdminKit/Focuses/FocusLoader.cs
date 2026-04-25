using KerbalCampaignKit.Config;

namespace KerbalAdminKit.Focuses
{
    public static class FocusLoader
    {
        public static FocusOption Load(ISceneNode node)
        {
            if (node == null) return null;
            var character = node.GetValue("character");
            var id = node.GetValue("id");
            if (string.IsNullOrEmpty(character) || string.IsNullOrEmpty(id)) return null;

            return new FocusOption
            {
                CharacterId = character,
                Id = id,
                Title = node.GetValue("title") ?? id,
                Description = node.GetValue("description") ?? "",
                Requirement = node.GetValue("requirement"),
                Flag = node.GetValue("flag"),
                FlagValue = node.GetValue("flagValue"),
            };
        }
    }
}
