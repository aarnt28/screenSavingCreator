using System;
using System.Reflection;
using System.Windows.Forms;

namespace screenSavingCreator
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Default to running screensaver mode
            string arg = args.Length > 0 ? args[0].ToLower() : "/s";
            // MessageBox.Show(string.Join("\n", Assembly.GetExecutingAssembly().GetManifestResourceNames()));
            



            switch (arg)
            {
                case "/c":
                    MessageBox.Show("No configuration options available.", "Image Screensaver");
                    break;

                case "/p":
                    // Preview mode: /p <hwnd>
                    if (args.Length >= 2 && long.TryParse(args[1], out long previewHandle))
                    {
                        Application.Run(new ScreensaverForm((IntPtr)previewHandle));
                    }
                    break;

                case "/s":
                default:
                    Application.Run(new ScreensaverForm());
                    break;
            }
            

        }


    }
}
