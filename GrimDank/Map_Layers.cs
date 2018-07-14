using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GrimDank
{
    // I define this stuff in a separate C# file even though it is literally part of the same class
    // (Map). Just because this stuff rarely ever gets touched unless you are changing number/order
    // of layers and is mostly back-end related, so less clutter in the Map.cs file.
    partial class Map : IInputHandler
    {
        // Dictionary is only used above for ease of maintenance, this is used as the actual code reference.
        private static readonly bool[] _layerCanHaveMultipleItems;

        // Each layer must have an entry in this dictionary, specifying whether or not that layer
        // allows multiple items on that layer to occupy the same space (basically whether or not it
        // uses buckets). Items, for example should -- creatures, not so much. Pretty much all the
        // code that has to care about this has already been written, but if you're trying to check
        // whether its allowed at run-time, use the bool array below -- its generated based on these values
        private static readonly Dictionary<Layer, bool> _layerMultipleItemsMap = new Dictionary<Layer, bool>
        {
            { Layer.ITEMS, true },
            { Layer.CREATURES, false }
        };

        private static readonly int _layerSize;

        // Pretty much just does some debug-checks to make sure we didn't screw up the association
        // between layers and arrays and ints. Might use CodeContract later, but for now this is
        // guaranteed at run-time to be run before the first Map instance is made so it works. Also
        // this function literally only ever is compiled if we are in Debug mode so no release
        // performance cost.
        static Map()
        {
            _layerSize = Enum.GetNames(typeof(Layer)).Length;

            // Called in Debug mode only.
            TestAssociations();

            _layerCanHaveMultipleItems = new bool[_layerSize];
            foreach (var kvPair in _layerMultipleItemsMap)
                _layerCanHaveMultipleItems[(int)kvPair.Key] = kvPair.Value;
        }

        // You CANNOT assign these enum values to start at any integer other than 0 or things break.
        // Depends on the first item in the enum converting to the integer 0, the second to 1, and so-on.
        public enum Layer
        { ITEMS, CREATURES }

        [Conditional("DEBUG")]
        private static void TestAssociations()
        {
            // If this fails, the number of Dictionary entries isn't corresponding 1 to 1 to the
            // Layers enum.
            Debug.Assert(_layerMultipleItemsMap.Count == _layerSize);

            // If one of these fails, enum values aren't default (you've done something like
            // SOMELAYER = 4 in the Layer declaration and that will break things
            for (int i = 0; i < _layerSize; i++)
                Debug.Assert(Enum.IsDefined(typeof(Layer), i));
        }
    }
}