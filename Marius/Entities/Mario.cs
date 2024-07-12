using Marius.Engine;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Marius.Entities
{
    class Mario : Entity
    {
        Animator Animator;
        Rigidbody Rigidbody;
        Collider Collider;
        SpriteRenderer SpriteRenderer;
        Dialogue Dialogue;

        float MaxSpeed = 300;
        bool HasLandedFirstTime;
        bool IsGrounded;
        bool IsDead;

        public override void Initialize()
        {
            Layer = 1;
            var workingArea = Screen.PrimaryScreen.WorkingArea;
            Position = new PointF(workingArea.Width / 2f, workingArea.Height / 2f);
            Scale = new SizeF(3, 3);

            Animator = AddComponent<Animator>();
            Rigidbody = AddComponent<Rigidbody>();
            Collider = AddComponent<Collider>();
            Collider.Offset = new PointF(0, 1);
            Collider.Size = new SizeF(6, 18);
            SpriteRenderer = AddComponent<SpriteRenderer>();
            Dialogue = Scene.FindEntity<Dialogue>();
        }

        public override void Update()
        {
            if (Input.IsKeyDown(Keys.Escape))
            {
                Application.Exit();
            }

            if (IsDead)
            {
                return;
            }

            bool isStopping = true;
            float TargetVelocityX = 0;

            if (Input.IsKeyDown(Keys.A))
            {
                if (Rigidbody.Velocity.X < -5)
                {
                    isStopping = false;
                    Scale = new SizeF(-Math.Abs(Scale.Width), Scale.Height);
                }

                TargetVelocityX = -MaxSpeed;
            }

            if (Input.IsKeyDown(Keys.D))
            {
                if (Rigidbody.Velocity.X > 5)
                {
                    isStopping = false;
                    Scale = new SizeF(Math.Abs(Scale.Width), Scale.Height);
                }

                TargetVelocityX = MaxSpeed;
            }

            if (Input.IsKeyDown(Keys.Space) && IsGrounded)
            {
                Rigidbody.Velocity = new PointF(Rigidbody.Velocity.X, -500);
            }

            Rigidbody.Velocity = new PointF(Rigidbody.Velocity.X + (TargetVelocityX - Rigidbody.Velocity.X) * 0.06f, Rigidbody.Velocity.Y);

            if (IsGrounded)
            {
                if (Math.Abs(Rigidbody.Velocity.X) > 5)
                {
                    if (isStopping)
                    {
                        Animator.Play(Sprites.MarioStoppingAnim);
                    }
                    else
                    {
                        Animator.Play(Sprites.MarioRunAnim);
                    }
                }
                else
                {
                    Animator.Play(Sprites.MarioIdleAnim);
                }
            }
            else
            {
                Animator.Play(null);

                if (Rigidbody.Velocity.Y < -100)
                {
                    SpriteRenderer.Sprite = Sprites.MarioJumpFrames[0];
                }
                else if (Rigidbody.Velocity.Y <= 100)
                {
                    SpriteRenderer.Sprite = Sprites.MarioJumpFrames[1];
                }
                else
                {
                    SpriteRenderer.Sprite = Sprites.MarioJumpFrames[2];
                }
            }

            Dialogue.Position = new PointF(Position.X - 14, Position.Y - 40);
            IsGrounded = false;
        }

        public override void OnCollision(Collision collision)
        {
            if (collision.Type == CollisionType.Window)
            {
                IsGrounded = true;

                if (!HasLandedFirstTime)
                {
                    HasLandedFirstTime = true;
                    Dialogue.Start(new string[]
                    {
                        "Sup!",
                        "You can use A, D and Space keys to move me around.",
                        "If you want to end the game just hit the Escape key."
                    });
                }
            }
            else if (collision.Type == CollisionType.Entity &&
                collision.Collider.Entity is Goomba goomba && !goomba.IsDead && !IsDead)
            {
                var bounds = Collider.Bounds;
                var goombaBounds = goomba.GetComponent<Collider>().Bounds;
                var goombaCy = goombaBounds.Y + goombaBounds.Height / 2f;

                if (bounds.Bottom < goombaCy)
                {
                    var intersect = RectangleF.Intersect(bounds, goombaBounds);
                    Scene.AddEntity(new HitParticle()
                    {
                        Position = new PointF(intersect.X + intersect.Width / 2f, intersect.Y + intersect.Height / 2f),
                    });
                    Rigidbody.Velocity = new PointF(Rigidbody.Velocity.X, -300);
                    goomba.Die();
                }
                else
                {
                    Die();
                }
            }
        }

        public void Die()
        {
            IsDead = true;
            Collider.IsEnabled = false;
            Rigidbody.Velocity = new Point(0, -500);
            Animator.Play(Sprites.MarioDeathAnim);
        }
    }
}
