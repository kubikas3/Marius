using Marius.Engine;
using System;
using System.Drawing;

namespace Marius.Entities
{
    class GoombaSpawner : Entity
    {
        float SpawnInterval = 10;
        float NextSpawnTime;
        Random Random;

        public override void Initialize()
        {
            NextSpawnTime = Scene.Time + SpawnInterval;
            Random = new Random(Guid.NewGuid().GetHashCode());
        }

        public override void OnCollision(Collision collision)
        {

        }

        public override void Update()
        {
            if (Scene.Time >= NextSpawnTime)
            {
                NextSpawnTime = Scene.Time + SpawnInterval;
                var worldBounds = Scene.Bounds;
                var positionX = Random.Next((int)worldBounds.Left + 50, (int)worldBounds.Right - 50);
                Scene.AddEntity(new Goomba()
                {
                    Position = new PointF(positionX, worldBounds.Top - 100),
                });
            }
        }
    }
}
