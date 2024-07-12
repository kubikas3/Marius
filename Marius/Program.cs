using Marius.Engine;
using Marius.Entities;
using System;
using System.Windows.Forms;

namespace Marius
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Initialize();
            Application.ApplicationExit += Exit;
            var settings = new Settings
            {
                CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality,
                InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor,
                PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality,
                TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit,
            };
            Engine.Engine.Run(settings);
        }

        static void Exit(object sender, EventArgs e)
        {
            Fonts.Unload();
        }

        static void Initialize()
        {
            Fonts.Load();
            Scene.AddEntity(new Mario());
            Scene.AddEntity(new Dialogue());
            Scene.AddEntity(new GoombaSpawner());
        }
    }
}
