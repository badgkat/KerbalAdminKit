namespace KerbalAdminKit.Characters
{
    /// <summary>
    /// One layer in a chip's composed visual. Drawn in cfg order (first = back).
    /// rect uses normalized [x, y, width, height] in chip space (0..1).
    /// </summary>
    public sealed class ChipLayerSpec
    {
        public string TextureUrl;
        public float RectX = 0f;
        public float RectY = 0f;
        public float RectW = 1f;
        public float RectH = 1f;
        public bool TintWithDisposition = false;
    }
}
