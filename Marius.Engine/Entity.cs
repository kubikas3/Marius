using System.Collections.Generic;
using System.Drawing;

namespace Marius.Engine
{
    public abstract class Entity
    {
        internal HashSet<Component> Components { get; } = new HashSet<Component>();

        internal LayeredForm Form;

        public int Layer { get; set; }

        public PointF Position { get; set; }

        public PointF Pivot { get; set; } = new PointF(0.5f, 0.5f);

        public SizeF Scale { get; set; } = new SizeF(1, 1);

        public abstract void Initialize();

        public abstract void Update();

        public abstract void OnCollision(Collision collision);

        public T AddComponent<T>() where T : Component, new()
        {
            T component = new T
            {
                Entity = this
            };
            Components.Add(component);
            return component;
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (var component in Components)
            {
                if (component is T t)
                {
                    return t;
                }
            }

            return null;
        }
    }
}
