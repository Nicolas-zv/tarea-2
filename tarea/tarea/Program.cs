using System;
using OpenTK.Windowing.Desktop;

namespace OpenTK_U
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new OpenTK.Mathematics.Vector2i(800, 600),
                Title = "Letter U 3D",
            };

            using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
            {
                window.Run();
            }
        }
    }
}
