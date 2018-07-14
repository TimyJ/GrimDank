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
        public static GrimDank Game { get; private set; }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Game = new GrimDank())
                Game.Run();
        } 
    }
}
