using System;
using System.Diagnostics;

namespace GrimDank
{
    // I define this stuff in a separate C# file even though it is literally part of the same class (Map).  Just because this stuff rarely
    // ever gets touched and is mostly back-end related, so less clutter in the Map.cs file.
    partial class Map
    {
        // You CANNOT assign these enum values to start at any integer other than 0 or things break.  Depends on the first item in the enum
        // converting to the integer 0, the second to 1, and so-on. 
        public enum Layer
        { ITEMS, CREATURES }

        // Array saying whether each of the layers in the Layers enum can have multiple items on that layer
        // or not (ones like creature that never should allow it, should say so, for optimization reasons.  Should always be the same size as
        // the Layers array and it will fail horribly at runtime if its not.
        private static readonly bool[] _layerCanHaveMultipleItems =
        { true, false };

        private static readonly int _layerSize;

        // Pretty much just does some debug-checks to make sure we didn't screw up the association between layers and arrays and ints. Might
        // use CodeContract later, but for now this is guaranteed at run-time to be run before the first Map instance is made so it works.
        // Also this function literally only ever is compiled if we are in Debug mode so no release performance cost.
        static Map()
        {
            _layerSize = Enum.GetNames(typeof(Layer)).Length;

            // Debug call only
            TestAssociations();
        }

        [Conditional("DEBUG")]
        private static void TestAssociations()
        {
            // If this fails, the length of the boolean array isn't corresponding 1 to 1 to the Layers enum.
            Debug.Assert(_layerCanHaveMultipleItems.Length == _layerSize);

            // If one of these fails, enum values aren't default (you've done something like SOMELAYER = 4 in the Layer declaration and that will
            // break things
            for (int i = 0; i < _layerSize; i++)
                Debug.Assert(Enum.IsDefined(typeof(Layer), i));
        }
    }
}
