using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Marius.Engine
{
    public class Settings
    {
        public CompositingQuality CompositingQuality { get; set; }
        public InterpolationMode InterpolationMode { get; set; }
        public PixelOffsetMode PixelOffsetMode { get; set; }
        public TextRenderingHint TextRenderingHint { get; set; }
    }
}
