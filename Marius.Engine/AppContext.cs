using Marius.Utils;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Marius.Engine
{
    internal class AppContext : ApplicationContext
    {
        const float TickInterval = 1f / 64f;
        Stopwatch Stopwatch;

        public AppContext()
        {
            Stopwatch = Stopwatch.StartNew();
            Application.Idle += Application_Idle;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            while (!MessageHelper.PeekMessage(out _, IntPtr.Zero, 0, 0, 0))
            {
                while (Stopwatch.Elapsed.TotalSeconds < TickInterval)
                {
                    Thread.Sleep(0);
                }

                Scene.Time += (float)Stopwatch.Elapsed.TotalSeconds;
                Stopwatch.Restart();
                Update();
            }
        }

        void Update()
        {
            UpdateRemovedEntities();
            UpdateAddedEntities();
            UpdatePhysics();

            foreach (var entity in Scene.Entities)
            {
                entity.Update();
            }

            UpdateAnimations();
            UpdateForms();
        }

        void UpdateAddedEntities()
        {
            if (Scene.AddedEntities.Count == 0)
            {
                return;
            }

            Scene.Entities.AddRange(Scene.AddedEntities);

            foreach (var entity in Scene.AddedEntities)
            {
                entity.Initialize();
                entity.Form = new LayeredForm(entity);
            }

            Scene.AddedEntities.Clear();
        }

        void UpdateRemovedEntities()
        {
            if (Scene.RemovedEntities.Count == 0)
            {
                return;
            }

            foreach (var entity in Scene.RemovedEntities)
            {
                Scene.Entities.Remove(entity);
                entity.Components.Clear();

                if (entity.Form is LayeredForm form && !form.IsDisposed)
                {
                    form.Close();
                    form.Dispose();
                }
            }

            Scene.RemovedEntities.Clear();
        }

        void UpdatePhysics()
        {
            foreach (var entity in Scene.Entities)
            {
                var rigidbody = entity.GetComponent<Rigidbody>();

                if (rigidbody == null || !rigidbody.IsEnabled)
                {
                    continue;
                }

                entity.Position += new SizeF(rigidbody.Velocity.X * TickInterval, rigidbody.Velocity.Y * TickInterval);
                rigidbody.Velocity += new SizeF(0, Scene.Gravity * TickInterval);
            }

            for (var i = 0; i < Scene.Entities.Count - 1; i++)
            {
                for (var j = i + 1; j < Scene.Entities.Count; j++)
                {
                    var entityA = Scene.Entities[i];
                    var entityB = Scene.Entities[j];
                    var colA = entityA.GetComponent<Collider>();
                    var colB = entityB.GetComponent<Collider>();

                    if (colA == null || colB == null || !colA.IsEnabled || !colB.IsEnabled)
                    {
                        continue;
                    }

                    var boundsA = colA.Bounds;
                    var boundsB = colB.Bounds;

                    if (boundsA.IntersectsWith(boundsB))
                    {
                        entityA.OnCollision(new Collision
                        {
                            Type = CollisionType.Entity,
                            Collider = colB,
                        });
                        entityB.OnCollision(new Collision
                        {
                            Type = CollisionType.Entity,
                            Collider = colA,
                        });
                    }
                }
            }

            foreach (var entity in Scene.Entities)
            {
                var collider = entity.GetComponent<Collider>();
                var rigidbody = entity.GetComponent<Rigidbody>();

                if (rigidbody == null || !rigidbody.IsEnabled || collider == null || !collider.IsEnabled)
                {
                    continue;
                }

                var bounds = collider.Bounds;

                foreach (var rect in WindowHelper.GetWindowRects())
                {
                    if (bounds.Right > rect.Left && bounds.Left < rect.Right &&
                        bounds.Bottom <= rect.Top && bounds.Bottom + rigidbody.Velocity.Y * TickInterval > rect.Top)
                    {
                        var distance = rect.Top - bounds.Bottom;
                        entity.Position = new PointF(entity.Position.X, entity.Position.Y + distance);
                        rigidbody.Velocity = new PointF(rigidbody.Velocity.X, 0);
                        entity.OnCollision(new Collision
                        {
                            Type = CollisionType.Window,
                            WindowBounds = rect,
                        });
                    }
                }

                var worldBounds = Scene.Bounds;

                if (worldBounds.Top - bounds.Top is var dTop && dTop > 0)
                {
                    entity.Position = new PointF(entity.Position.X, entity.Position.Y + dTop);
                    rigidbody.Velocity = new PointF(rigidbody.Velocity.X, 0);
                }

                if (bounds.Bottom - worldBounds.Bottom is var dBottom && dBottom > 0)
                {
                    entity.Position = new PointF(entity.Position.X, entity.Position.Y - dBottom);
                    rigidbody.Velocity = new PointF(rigidbody.Velocity.X, 0);
                }

                if (worldBounds.Left - bounds.Left is var dLeft && dLeft > 0)
                {
                    entity.Position = new PointF(entity.Position.X + dLeft, entity.Position.Y);
                    rigidbody.Velocity = new PointF(0, rigidbody.Velocity.Y);
                }

                if (bounds.Right - worldBounds.Right is var dRight && dRight > 0)
                {
                    entity.Position = new PointF(entity.Position.X - dRight, entity.Position.Y);
                    rigidbody.Velocity = new PointF(0, rigidbody.Velocity.Y);
                }
            }
        }

        private void UpdateAnimations()
        {
            foreach (var entity in Scene.Entities)
            {
                if (entity.GetComponent<Animator>() is Animator animator && animator.IsEnabled &&
                    animator.Animation is Animation anim)
                {
                    if (animator.IsPlaying)
                    {
                        if (animator.Time < anim.Length)
                        {
                            animator.Time += TickInterval;
                        }
                        else if (anim.Loop)
                        {
                            animator.Time %= anim.Length;
                        }
                        else
                        {
                            animator.Time = anim.Length;
                            animator.Pause();
                        }
                    }

                    if (entity.GetComponent<SpriteRenderer>() is SpriteRenderer renderer)
                    {
                        int frameIndex = (int)(animator.Time / anim.Length * anim.Frames.Length);
                        renderer.Sprite = anim.Frames[Math.Min(frameIndex, anim.Frames.Length - 1)];
                    }
                }
            }
        }

        void UpdateForms()
        {
            foreach (var entity in Scene.Entities)
            {
                if (entity.Form is LayeredForm form && !form.IsDisposed)
                {
                    form.Show();
                    form.ResizeBounds();
                    form.Invalidate();
                }
            }
        }
    }
}
