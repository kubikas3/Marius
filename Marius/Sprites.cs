using Marius.Engine;
using Marius.Properties;
using System.Drawing;
using System.Drawing.Imaging;

namespace Marius
{
    static class Sprites
    {
        public static Animation MarioIdleAnim = new Animation
        {
            Frames = new Bitmap[]
            {
                Resources.MarioSpritesheet.Clone(new Rectangle(0, 0, 20, 20), PixelFormat.Format32bppArgb),
                Resources.MarioSpritesheet.Clone(new Rectangle(20, 0, 20, 20), PixelFormat.Format32bppArgb)
            },
            Length = 1f
        };
        public static Animation MarioRunAnim = new Animation
        {
            Frames = new Bitmap[]
            {
                Resources.MarioSpritesheet.Clone(new Rectangle(0, 20, 20, 20), PixelFormat.Format32bppArgb),
                Resources.MarioSpritesheet.Clone(new Rectangle(20, 20, 20, 20), PixelFormat.Format32bppArgb),
                Resources.MarioSpritesheet.Clone(new Rectangle(40, 20, 20, 20), PixelFormat.Format32bppArgb)
            },
            Length = 0.2f
        };
        public static Animation MarioStoppingAnim = new Animation
        {
            Frames = new Bitmap[]
            {
                Resources.MarioSpritesheet.Clone(new Rectangle(0, 40, 20, 20), PixelFormat.Format32bppArgb)
            }
        };
        public static Bitmap[] MarioJumpFrames = new Bitmap[]
        {
            Resources.MarioSpritesheet.Clone(new Rectangle(0, 60, 20, 20), PixelFormat.Format32bppArgb),
            Resources.MarioSpritesheet.Clone(new Rectangle(20, 60, 20, 20), PixelFormat.Format32bppArgb),
            Resources.MarioSpritesheet.Clone(new Rectangle(40, 60, 20, 20), PixelFormat.Format32bppArgb)
        };
        public static Animation MarioDeathAnim = new Animation
        {
            Frames = new Bitmap[]
            {
                Resources.MarioSpritesheet.Clone(new Rectangle(0, 80, 20, 20), PixelFormat.Format32bppArgb)
            }
        };

        public static Animation GoombaIdleAnim = new Animation
        {
            Frames = new Bitmap[]
            {
                Resources.GoombaSpritesheet.Clone(new Rectangle(0, 0, 15, 15), PixelFormat.Format32bppArgb)
            }
        };
        public static Animation GoombaFallAnim = new Animation
        {
            Frames = new Bitmap[]
            {
                Resources.GoombaSpritesheet.Clone(new Rectangle(0, 15, 15, 15), PixelFormat.Format32bppArgb)
            }
        };
        public static Animation GoombaRunAnim = new Animation
        {
            Frames = new Bitmap[]
            {
                Resources.GoombaSpritesheet.Clone(new Rectangle(0, 30, 15, 15), PixelFormat.Format32bppArgb),
                Resources.GoombaSpritesheet.Clone(new Rectangle(15, 30, 15, 15), PixelFormat.Format32bppArgb),
                Resources.GoombaSpritesheet.Clone(new Rectangle(30, 30, 15, 15), PixelFormat.Format32bppArgb),
            },
            Length = 0.2f,
        };
        public static Animation GoombaDeathAnim = new Animation
        {
            Frames = new Bitmap[]
            {
                Resources.GoombaSpritesheet.Clone(new Rectangle(0, 45, 15, 15), PixelFormat.Format32bppArgb),
                Resources.GoombaSpritesheet.Clone(new Rectangle(15, 45, 15, 15), PixelFormat.Format32bppArgb),
            },
            Length = 0.1f,
            Loop = false,
        };

        public static Animation HitAnim = new Animation
        {
            Frames = new Image[]
            {
                Resources.ParticleSpritesheet.Clone(new Rectangle(0, 0, 10, 10), PixelFormat.Format32bppArgb),
                Resources.ParticleSpritesheet.Clone(new Rectangle(10, 0, 10, 10), PixelFormat.Format32bppArgb),
                Resources.ParticleSpritesheet.Clone(new Rectangle(20, 0, 10, 10), PixelFormat.Format32bppArgb),
            },
            Length = 0.3f,
            Loop = false,
        };

        public static Image SpeechBubble = Resources.SpeechBubbleSprite;
    }
}
