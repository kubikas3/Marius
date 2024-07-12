using System;
using System.Drawing;

namespace Marius.Engine
{
    public class Collider : Component
    {
        public PointF Offset { get; set; }
        public SizeF Size { get; set; }

        public RectangleF Bounds
        {
            get
            {
                var scaleX = Entity.Scale.Width;
                var scaleY = Entity.Scale.Height;
                var width = Size.Width * Math.Abs(scaleX);
                var height = Size.Height * Math.Abs(scaleY);

                return new RectangleF(
                    Entity.Position.X + Offset.X * scaleX - width / 2f,
                    Entity.Position.Y + Offset.Y * scaleY - height / 2f,
                    width, height);
            }
        }
    }
}
