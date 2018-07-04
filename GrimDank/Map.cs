using System;
using System.Collections.Generic;
using GoRogue;
using GrimDank.MObjects;


namespace GrimDank
{
    partial class Map
    { 
        // MObject layers.
        private List<ISpatialMap<MObject>> _layers;

        // Get all MObjects.
        public IEnumerable<MObject> MObjects
        {
            get
            {
                foreach (var layer in _layers)
                    foreach (var item in layer.Items)
                        yield return item;
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;

            _layers = new List<ISpatialMap<MObject>>();

            for (int i = 0; i < _layerSize; i++)
            {
                if (_layerCanHaveMultipleItems[i])
                    _layers.Add(new MultiSpatialMap<MObject>());
                else
                    _layers.Add(new SpatialMap<MObject>());
            }
        }

        // Gets read-only SpatialMap for easy interaction or enumeration for a layer
        public IReadOnlySpatialMap<MObject> GetLayer(Layer layer) => _layers[(int)layer].AsReadOnly();

        // Gets all layers in a safe way, just in case you ever need to (you probaably don't).
        public IEnumerable<IReadOnlySpatialMap<MObject>> GetLayers()
        {
            foreach (var layer in _layers)
                yield return layer.AsReadOnly();
        }

        public bool Add(MObject mObject) => Add(mObject, mObject.Position);

        // Adds given MObject at given position (Position property of MObject will be automatically updated)
        public bool Add(MObject mObject, Coord position)
        {
            if (Collides(mObject, position))
                return false;

            if (!_layers[(int)mObject.Layer].Add(mObject, position))
                return false;

            mObject.CurrentMap?.Remove(mObject);
            mObject.Position = position; // Performs no collision detecttion because its CurrentMap is guaranteed to be null because of above line

            mObject.Moved += OnMObjectMoved;
            mObject._onMapChanged(this);

            return true;
        }

        public bool Remove(MObject mObject)
        {
            if (!_layers[(int)mObject.Layer].Remove(mObject))
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

        public MObject Raycast(Coord position) => Raycast(position, (Layer)(_layerSize - 1));

        public MObject Raycast(Coord position, Layer layer) => Raycast(position, layer, a => true);


        public MObject Raycast(Coord position, Predicate<MObject> predicate) => Raycast(position, (Layer)(_layerSize - 1), predicate);

        // Raycasts from the given layer down, at the given position.  Returns the first object found for which the given
        // predicate returns true.
        public MObject Raycast(Coord position, Layer layer, Predicate<MObject> predicate)
        {
            for (int i = (int)layer; i >= 0; i--)
                foreach (var obj in _layers[i].GetItems(position))
                    if (predicate(obj))
                        return obj;

            return null;
        }

        // Used just to keep SpatialMap up to date; guaranteed to succeed since collision detection did the checks before this was ever called.
        private void OnMObjectMoved(object s, MovedArgs e)
        {
            var mObject = (MObject)s;
            _layers[(int)mObject.Layer].Move(mObject, e.NewPosition);
        }
    }
}
