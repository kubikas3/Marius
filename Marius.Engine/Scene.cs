using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Marius.Engine
{
    public static class Scene
    {
        internal static List<Entity> Entities = new List<Entity>();

        internal static List<Entity> AddedEntities = new List<Entity>();

        internal static List<Entity> RemovedEntities = new List<Entity>();

        public static RectangleF Bounds { get => SystemInformation.VirtualScreen; }

        public static float Gravity { get; set; } = 981;

        public static float Time { get; internal set; }

        public static void AddEntity(Entity entity)
        {
            AddedEntities.Add(entity);
        }

        public static void DestroyEntity(Entity entity)
        {
            RemovedEntities.Add(entity);
        }

        public static T FindEntity<T>() where T : Entity
        {
            return (T)Entities.Find((entity) => entity is T);
        }
    }
}
