using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static Marius.Utils.WindowHelper;

namespace Marius.Engine
{
    internal class LayeredForm : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;  // WS_EX_TOOLWINDOW (0x80)
                return cp;
            }
        }

        private int Layer;

        public Entity Entity { get; private set; }

        public LayeredForm(Entity entity)
        {
            Entity = entity;
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.Aqua;
            ControlBox = false;
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            Size = new Size(0, 0);
            TransparencyKey = Color.Aqua;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_WINDOWPOSCHANGING &&
                m.GetLParam(typeof(WINDOWPOS)) is WINDOWPOS windowPos &&
                (windowPos.flags & SWP_NOZORDER) == 0)
            {
                var aboveEntity = Scene.Entities
                    .FirstOrDefault(e => e.Layer > Entity.Layer && e.Form != null && !e.Form.IsDisposed);

                if (aboveEntity != null)
                {
                    windowPos.hwndInsertAfter = GetWindow(aboveEntity.Form.Handle, GW_HWNDNEXT); // place window below
                    Marshal.StructureToPtr(windowPos, m.LParam, true);
                }
            }

            base.WndProc(ref m);
        }

        public void ResizeBounds()
        {
            float scaleX = Math.Abs(Entity.Scale.Width),
                scaleY = Math.Abs(Entity.Scale.Height),
                width = Width,
                height = Height;

            foreach (var component in Entity.Components)
            {
                if (component is SpriteRenderer renderer)
                {
                    if (renderer.RenderMode == SpriteRenderMode.Simple)
                    {
                        width = renderer.Sprite.Width * scaleX;
                        height = renderer.Sprite.Height * scaleY;
                    }
                    else if (renderer.RenderMode == SpriteRenderMode.NineSliced)
                    {
                        width = renderer.Size.Width * scaleX;
                        height = renderer.Size.Height * scaleY;
                    }
                }
            }

            float x = Entity.Position.X - width * Entity.Pivot.X,
                y = Entity.Position.Y - height * Entity.Pivot.Y;

            uint flags = SWP_NOACTIVATE;

            if ((int)x == Left && (int)y == Top)
            {
                flags |= SWP_NOMOVE;
            }
            if ((int)width == Width && (int)height == Height)
            {
                flags |= SWP_NOSIZE;
            }
            if (Entity.Layer == Layer)
            {
                flags |= SWP_NOZORDER;
            }

            Layer = Entity.Layer;
            SetWindowPos(Handle, IntPtr.Zero, (int)x, (int)y, (int)width, (int)height, flags);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.CompositingQuality = Engine.Settings.CompositingQuality;
            e.Graphics.InterpolationMode = Engine.Settings.InterpolationMode;
            e.Graphics.PixelOffsetMode = Engine.Settings.PixelOffsetMode;
            e.Graphics.TextRenderingHint = Engine.Settings.TextRenderingHint;

            var centerX = ClientSize.Width / 2f;
            var centerY = ClientSize.Height / 2f;

            e.Graphics.TranslateTransform(centerX, centerY);
            e.Graphics.ScaleTransform(Entity.Scale.Width, Entity.Scale.Height);

            foreach (var component in Entity.Components)
            {
                if (component != null && component.IsEnabled)
                {
                    if (component is SpriteRenderer spriteRenderer)
                    {
                        DrawSpriteRenderer(e.Graphics, spriteRenderer);
                    }
                    else if (component is TextRenderer textRenderer)
                    {
                        DrawTextRenderer(e.Graphics, textRenderer);
                    }
                }
            }

            //foreach (var component in Entity.Components)
            //{
            //    if (component is Collider collider)
            //    {
            //        DrawCollider(e.Graphics, collider);
            //    }
            //}
        }

        void DrawCollider(Graphics g, Collider collider)
        {
            using (var brush = new SolidBrush(Color.FromArgb(100, Color.Pink)))
            {
                var position = new PointF(collider.Offset.X - collider.Size.Width / 2f, collider.Offset.Y - collider.Size.Height / 2f);
                g.FillRectangle(brush, new RectangleF(position, collider.Size));
            }
        }

        void DrawSpriteRenderer(Graphics g, SpriteRenderer renderer)
        {
            if (renderer.Sprite is Image sprite)
            {
                if (renderer.RenderMode == SpriteRenderMode.Simple)
                {
                    g.DrawImage(sprite, -sprite.Width / 2f, -sprite.Height / 2f);
                }
                else if (renderer.RenderMode == SpriteRenderMode.NineSliced)
                {
                    DrawNineSlicedImage(g, sprite, new RectangleF(-renderer.Size.Width / 2f, -renderer.Size.Height / 2f, renderer.Size.Width, renderer.Size.Height));
                }
            }
        }

        void DrawTextRenderer(Graphics g, TextRenderer renderer)
        {
            using (var brush = new SolidBrush(renderer.Color))
            {
                var position = new PointF(
                    renderer.Bounds.X - renderer.Bounds.Width / 2f,
                    renderer.Bounds.Y - renderer.Bounds.Height / 2f);
                g.DrawString(renderer.Text, renderer.Font, brush, new RectangleF(position, renderer.Bounds.Size), renderer.Format);
            }
        }

        void DrawNineSlicedImage(Graphics g, Image image, RectangleF rect)
        {
            var imgWidth = image.Width;
            var imgHeight = image.Height;
            var sliceWidth = image.Width / 3;
            var sliceHeight = image.Height / 3;

            var topLeftSlice = new Rectangle(0, 0, sliceWidth, sliceHeight);
            var topMiddleSlice = new Rectangle(sliceWidth, 0, sliceWidth, sliceHeight);
            var topRightSlice = new Rectangle(imgWidth - sliceWidth, 0, sliceWidth, sliceHeight);

            var middleLeftSlice = new Rectangle(0, sliceHeight, sliceWidth, sliceHeight);
            var centerSlice = new Rectangle(sliceWidth, sliceHeight, sliceWidth, sliceHeight);
            var middleRightSlice = new Rectangle(imgWidth - sliceWidth, sliceHeight, sliceWidth, sliceHeight);

            var bottomLeftSlice = new Rectangle(0, imgHeight - sliceHeight, sliceWidth, sliceHeight);
            var bottomMiddleSlice = new Rectangle(sliceWidth, imgHeight - sliceHeight, sliceWidth, sliceHeight);
            var bottomRightSlice = new Rectangle(imgWidth - sliceWidth, imgHeight - sliceHeight, sliceWidth, sliceHeight);

            // edges
            g.DrawImage(image, new RectangleF(rect.Left + sliceWidth, rect.Top, rect.Width - 2 * sliceWidth, sliceHeight), topMiddleSlice, GraphicsUnit.Pixel);
            g.DrawImage(image, new RectangleF(rect.Left + sliceWidth, rect.Bottom - sliceHeight, rect.Width - 2 * sliceWidth, sliceHeight), bottomMiddleSlice, GraphicsUnit.Pixel);
            g.DrawImage(image, new RectangleF(rect.Left, rect.Top + sliceHeight, sliceWidth, rect.Height - 2 * sliceHeight), middleLeftSlice, GraphicsUnit.Pixel);
            g.DrawImage(image, new RectangleF(rect.Right - sliceWidth, rect.Top + sliceWidth, sliceWidth, rect.Height - 2 * sliceHeight), middleRightSlice, GraphicsUnit.Pixel);

            // corners
            g.DrawImage(image, new RectangleF(rect.Left, rect.Top, sliceWidth, sliceHeight), topLeftSlice, GraphicsUnit.Pixel);
            g.DrawImage(image, new RectangleF(rect.Right - sliceWidth, rect.Top, sliceWidth, sliceHeight), topRightSlice, GraphicsUnit.Pixel);
            g.DrawImage(image, new RectangleF(rect.Left, rect.Bottom - sliceHeight, sliceWidth, sliceHeight), bottomLeftSlice, GraphicsUnit.Pixel);
            g.DrawImage(image, new RectangleF(rect.Right - sliceWidth, rect.Bottom - sliceHeight, sliceWidth, sliceHeight), bottomRightSlice, GraphicsUnit.Pixel);

            // center
            g.DrawImage(image, new RectangleF(rect.Left + sliceWidth, rect.Top + sliceHeight, rect.Width - 2 * sliceWidth, rect.Height - 2 * sliceHeight), centerSlice, GraphicsUnit.Pixel);
        }
    }
}
