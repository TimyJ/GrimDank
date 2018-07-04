using System;
using System.Collections.Generic;
using System.Diagnostics;
using GoRogue;
using GrimDank.MObjects;


namespace GrimDank
{
    class Map
    {
        // Right now you CANNOT assign these enum values to start at any integer other than 0 or things break.  Depends on integer conversion
        // being 0,...,Layer.Size - 1
        public enum Layer
        { ITEMS, CREATURES }


        // Array saying whether each of the layers in the Layers enum can have multiple items on that layer
        // or not (ones like creature that never should shouldn't allow it for optimization reasons.  Should always be the same size as the Layers array
        // and it will fail horribly at runtime if its not.
        private static readonly bool[] _layerCanHaveMultipleItems =
        { false, true };

        private static readonly int _layerSize = Enum.GetNames(typeof(Layer)).Length;

        // MObject layers.
        private List<ISpatialMap<MObject>> _mObjects;

        public Map()
        {
            _mObjects = new List<ISpatialMap<MObject>>();

            for (int i = 0; i < _layerSize; i++)
            {
                if (_layerCanHaveMultipleItems[i])
                    _mObjects.Add(new MultiSpatialMap<MObject>());
                else
                    _mObjects.Add(new SpatialMap<MObject>());
            }
        }

        public IReadOnlySpatialMap<MObject> GetLayer(Layer layer) => _mObjects[(int)layer];

        public bool Add(MObject mObject) => Add(mObject, mObject.Position);

        public bool Add(MObject mObject, Coord position)
        {
            if (Collides(mObject, position))
                return false;

            if (!_mObjects[(int)mObject.Layer].Add(mObject, position))
                return false;

            mObject.CurrentMap?.Remove(mObject);
            mObject.Position = position; // Performs no collision detecttion because its CurrentMap is guaranteed to be null because of above line

            mObject.Moved += OnMObjectMoved;
            mObject._onMapChanged(this);

            return true;
        }

        public bool Remove(MObject mObject)
        {
            if (!_mObjects[(int)mObject.Layer].Remove(mObject))
                return false;

            mObject.Moved -= OnMObjectMoved;
            mObject._onMapChanged(null);

            return true;
        }

        public bool Collides(MObject mObject) => Collides(mObject, mObject.Position);

        // TODO: Need to add to this function to allow for terrain, once it's added.  I didn't know how we wanted to handle it so I left it.
        // Whether or not the given mObject would collide at the given position
        public bool Collides(MObject mObject, Coord position)
        {
            // We occupy a single-item-per-position layer, and there's already something there.
            if (!_layerCanHaveMultipleItems[(int)mObject.Layer] && GetLayer(mObject.Layer).Contains(position))
                return true;

            // We can't collide so we're good
            if (mObject.IsWalkable)
                return false;

            // There is a non-walkable thing
            if (Raycast(position, obj => !obj.IsWalkable) != null)
                return true;

            return false;
        }

        public MObject Raycast(Coord position) => Raycast(position, (Layer)_layerSize);

        public MObject Raycast(Coord position, Layer layer) => Raycast(position, layer, a => true);


        public MObject Raycast(Coord position, Predicate<MObject> predicate) => Raycast(position, (Layer)_layerSize, predicate);

        // Raycasts from the given layer down, at the given position.  Returns the first object found for which the given
        // predicate returns true.
        public MObject Raycast(Coord position, Layer layer, Predicate<MObject> predicate)
        {
            for (int i = (int)layer; i >= 0; i--)
                foreach (var obj in _mObjects[i].GetItems(position))
                    if (predicate(obj))
                        return obj;

            return null;
        }

        // Need to keep SpatialMap up to date; guaranteed to succeed since collision detection did the checks before this was ever called.
        private void OnMObjectMoved(object s, MovedArgs e)
        {
            var mObject = (MObject)s;
            _mObjects[(int)mObject.Layer].Move(mObject, e.NewPosition);
        }

        // Debug-only checks to make sure our array matches size of Layers.  Guaranteed to be run before the first Map instance is created, and gets us
        // sensible error messages if we screw up. Don't really need to touch this.
        static Map()
        {
            Debug.Assert(_layerCanHaveMultipleItems.Length == _layerSize);
        }
        // 

    }
}
