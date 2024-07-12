using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Marius.Engine
{
    public class Input
    {
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        public static bool IsKeyDown(Keys key)
        {
            return Convert.ToBoolean(GetKeyState((int)key) & 0x8000);
        }
    }
}
