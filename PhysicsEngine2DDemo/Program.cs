#region Using Statements
using System;

#endregion

namespace PhysicsEngine2DDemo
{
#if WINDOWS || LINUX
    // / <summary>
    // / The main class.
    // / </summary>
    public static class Program
    {
        // / <summary>
        // / The main entry point for the application.
        // / </summary>
        [STAThread]
        private static void Main()
        {
            DemoGame game = new DemoGame();
            game.Run();
        }
    }
#endif
}
