namespace KerbalAdminKit.Focuses
{
    public sealed class FocusOption
    {
        public string CharacterId;
        public string Id;
        public string Title;
        public string Description;
        public string Requirement;   // optional KDK flag expression
        public string Flag;          // the flag KAK writes on selection
        public string FlagValue;     // the value to write
    }
}
