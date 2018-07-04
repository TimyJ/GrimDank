using System;

namespace GrimDank
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Map map = new Map();
            using (var game = new GrimDank())
                game.Run();
        }
    }
}
