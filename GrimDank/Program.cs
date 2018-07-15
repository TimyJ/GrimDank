using System;
using GoRogue.MapGeneration.Generators;
using GoRogue.MapViews;
using GrimDank.MObjects;
using System.Diagnostics;
using GoRogue;

namespace GrimDank
{
    /// <summary>
    /// The main class.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new GrimDank())
                game.Run();
        } 
    }
}
