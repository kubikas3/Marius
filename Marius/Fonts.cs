using Marius.Properties;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace Marius
{
    static class Fonts
    {
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

        private static PrivateFontCollection privateFontCollection;

        public static Font DefaultFont { get; private set; }

        public static StringFormat DefaultFormat { get; private set; }

        public static void Load()
        {
            privateFontCollection = new PrivateFontCollection();
            LoadFontFromResource(Resources.SuperMarioBros2Font);
            DefaultFont = new Font(privateFontCollection.Families[0], 16f / 3f, FontStyle.Regular, GraphicsUnit.Pixel);
            DefaultFormat = new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
            };
        }

        static void LoadFontFromResource(byte[] fontData)
        {
            IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, fontPtr, fontData.Length);

            uint dummy = 0;
            privateFontCollection.AddMemoryFont(fontPtr, fontData.Length);
            AddFontMemResourceEx(fontPtr, (uint)fontData.Length, IntPtr.Zero, ref dummy);

            Marshal.FreeCoTaskMem(fontPtr);
        }

        public static void Unload()
        {
            DefaultFont?.Dispose();
            DefaultFormat?.Dispose();
            privateFontCollection?.Dispose();
        }
    }
}
