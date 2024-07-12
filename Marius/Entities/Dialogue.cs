using Marius.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Marius.Entities
{
    class Dialogue : Entity
    {
        SpriteRenderer SpriteRenderer;
        TextRenderer TextRenderer;
        Queue<string> Sentences;
        float NextSentenceTime;
        const int MaxLineWidth = 100;

        public override void Initialize()
        {
            Sentences = new Queue<string>();
            Pivot = new PointF(0, 1);
            Position = new PointF(0, 0);
            Scale = new SizeF(3, 3);
            Layer = 2;

            SpriteRenderer = AddComponent<SpriteRenderer>();
            SpriteRenderer.RenderMode = SpriteRenderMode.NineSliced;
            SpriteRenderer.Sprite = Sprites.SpeechBubble;

            TextRenderer = AddComponent<TextRenderer>();
            TextRenderer.Font = Fonts.DefaultFont;
            TextRenderer.Color = Color.Black;
            TextRenderer.Format = Fonts.DefaultFormat;
        }

        public override void OnCollision(Collision collision)
        {

        }

        public override void Update()
        {
            if (Scene.Time >= NextSentenceTime)
            {
                if (Sentences.Count > 0)
                {
                    NextSentenceTime = Scene.Time + 4;
                    var sentence = Sentences.Dequeue();

                    using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
                    {
                        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                        var size = graphics.MeasureString(sentence, TextRenderer.Font, MaxLineWidth, TextRenderer.Format);
                        TextRenderer.Bounds = new RectangleF(PointF.Empty, size);
                        TextRenderer.Text = sentence;
                        SpriteRenderer.Size = new SizeF(TextRenderer.Bounds.Size.Width + 10, TextRenderer.Bounds.Size.Height + 11);
                    }
                }
                else
                {
                    TextRenderer.IsEnabled = false;
                    SpriteRenderer.IsEnabled = false;
                }
            }
        }

        public void Start(string[] sentences)
        {
            TextRenderer.IsEnabled = true;
            SpriteRenderer.IsEnabled = true;
            Sentences.Clear();

            foreach (var sentence in sentences)
            {
                Sentences.Enqueue(sentence);
            }

            NextSentenceTime = Scene.Time;
        }
    }
}
