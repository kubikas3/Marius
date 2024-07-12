using System.Drawing;

namespace Marius.Engine
{
    public enum SpriteRenderMode
    {
        Simple,
        NineSliced
    }

    public class SpriteRenderer : Component
    {
        public Image Sprite { get; set; }

        public SizeF Size { get; set; }

        public SpriteRenderMode RenderMode { get; set; }
    }
}
