using System.Drawing;

namespace Marius.Engine
{
    public enum CollisionType
    {
        Window,
        Entity,
    }

    public struct Collision
    {
        public CollisionType Type { get; set; }
        public Collider Collider { get; set; }
        public Rectangle WindowBounds { get; set; }
    }
}
