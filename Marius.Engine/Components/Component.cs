namespace Marius.Engine
{
    public abstract class Component
    {
        public bool IsEnabled { get; set; } = true;

        public Entity Entity { get; internal set; }
    }
}
