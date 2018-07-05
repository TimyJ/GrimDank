using System.Diagnostics;

namespace GrimDank
{
    // Completely temp. Just here to allow us to easily ignore an actual message log for as long as
    // we want without having it be a pain to find all the places we want to print stuff.
    internal class MessageLog
    {
        public static void Write(string message) => Debug.WriteLine(message);
    }
}