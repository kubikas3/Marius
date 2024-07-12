using Marius.Engine;
using System.Drawing;

namespace Marius.Entities
{
    class HitParticle : Entity
    {
        Animator Animator;

        public override void Initialize()
        {
            Layer = 2;
            Scale = new SizeF(3, 3);

            AddComponent<SpriteRenderer>();
            Animator = AddComponent<Animator>();
            Animator.Play(Sprites.HitAnim);
        }

        public override void Update()
        {
            if (Animator.Time >= Animator.Animation.Length)
            {
                Scene.DestroyEntity(this);
            }
        }

        public override void OnCollision(Collision collision)
        {

        }
    }
}
