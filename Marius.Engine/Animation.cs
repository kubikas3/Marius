using System.Drawing;

namespace Marius.Engine
{
    public class Animation
    {
        public Image[] Frames { get; set; }
        public float Length { get; set; } = 1;
        public bool Loop { get; set; } = true;
    }
}
