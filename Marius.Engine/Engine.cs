using System.Windows.Forms;

namespace Marius.Engine
{
    public static class Engine
    {
        public static Settings Settings { get; private set; }

        public static void Run(Settings settings)
        {
            Settings = settings;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AppContext());
        }
    }
}
