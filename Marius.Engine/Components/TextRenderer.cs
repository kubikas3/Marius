using System.Drawing;

namespace Marius.Engine
{
    public class TextRenderer : Component
    {
        public string Text { get; set; }

        public Font Font { get; set; }

        public Color Color { get; set; }

        public RectangleF Bounds { get; set; }

        public StringFormat Format { get; set; }
    }
}
