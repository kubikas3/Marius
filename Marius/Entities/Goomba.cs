using Marius.Engine;
using System;
using System.Drawing;

namespace Marius.Entities
{
    class Goomba : Entity
    {
        Collider Collider;
        Rectangle Ground;
        Animator Animator;
        Rigidbody Rigidbody;

        bool IsGrounded;
        float DespawnTime;
        public bool IsDead = false;
        float TargetVelocityX = 150;

        public override void Initialize()
        {
            Layer = 0;
            Scale = new SizeF(3, 3);

            Animator = AddComponent<Animator>();
            Rigidbody = AddComponent<Rigidbody>();
            Collider = AddComponent<Collider>();
            Collider.IsEnabled = false;
            Collider.Offset = new Point(0, 2);
            Collider.Size = new Size(7, 11);
            AddComponent<SpriteRenderer>();
        }

        public override void OnCollision(Collision collision)
        {
            if (collision.Type == CollisionType.Window)
            {
                IsGrounded = true;
                Ground = collision.WindowBounds;
            }
        }

        public override void Update()
        {
            if (IsDead)
            {
                if (Scene.Time > DespawnTime)
                {
                    Scene.DestroyEntity(this);
                }

                return;
            }

            var bounds = Collider.Bounds;
            Collider.IsEnabled = Scene.Bounds.Contains(bounds);

            if (IsGrounded)
            {
                if (bounds.Left < Ground.Left + 20)
                {
                    TargetVelocityX = 150;
                }
                if (bounds.Right > Ground.Right - 20)
                {
                    TargetVelocityX = -150;
                }

                Rigidbody.Velocity = new PointF(Rigidbody.Velocity.X + (TargetVelocityX - Rigidbody.Velocity.X) * 0.05f, Rigidbody.Velocity.Y);

                if (Math.Abs(Rigidbody.Velocity.X) > 5)
                {
                    Animator.Play(Sprites.GoombaRunAnim);
                    Scale = new SizeF(Math.Abs(Scale.Width) * Math.Sign(Rigidbody.Velocity.X), Scale.Height);
                }
                else
                {
                    Animator.Play(Sprites.GoombaIdleAnim);
                }
            }
            else
            {
                Animator.Play(Sprites.GoombaFallAnim);
            }

            IsGrounded = false;
        }

        public void Die()
        {
            IsDead = true;
            Rigidbody.Velocity = new PointF(0, Rigidbody.Velocity.Y);
            Animator.Play(Sprites.GoombaDeathAnim);
            DespawnTime = Scene.Time + 2;
        }
    }
}
