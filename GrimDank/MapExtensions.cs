using GoRogue.MapViews;
using GoRogue;
using GrimDank.Terrains;
using System.Collections.Generic;
using GoRogue.MapGeneration;
using Microsoft.Xna.Framework.Input;

//Fuck you chris for making me finally learn how to use git properly
namespace GrimDank
{
    partial class Map
    {
        private Terrain[,] _terrain;
        private bool[,] _explored;
        private FOV _fov;
        public ResistanceProvider ResistanceMap { get; private set; }
        public WalkabilityProvider WalkabilityMap { get; private set; }
        public bool EnemyStatusToggle;
        public Targeting Targeter;

        public IEnumerable<Coord> CurrentFOV { get => _fov.CurrentFOV; }
        public IEnumerable<Coord> NewlySeen { get => _fov.NewlySeen; }
        public IEnumerable<Coord> NewlyUnseen { get => _fov.NewlyUnseen; }

        public void GenerateMap()
        {
            ArrayMap<bool> genMap = new ArrayMap<bool>(Width, Height);
            GoRogue.MapGeneration.Generators.CellularAutomataGenerator.Generate(genMap);

            // Can switch to this map gen for perf tests, since FPS is dependent on number of cells, we need a static map to perf-test
            // GoRogue.MapGeneration.Generators.RectangleMapGenerator.Generate(genMap);

            foreach(var pos in genMap.Positions())
            {
                if (genMap[pos])
                {
                    _terrain[pos.X, pos.Y] = Terrain.FLOOR;
                } else { _terrain[pos.X, pos.Y] = Terrain.WALL; }
            }
        }

        public void SpawnPunchingBags(int amountToSpawn)
        {
            for(int i = 0; i <= amountToSpawn; ++i)
            {
                Coord pos = WalkabilityMap.RandomPosition(true);
                MObjects.Creature mob = new MObjects.Creature(pos, 10, 0, "1d1", 0);
                Add(mob);
            }
        }

        public void SetupFOV(Coord playerPos, int initialRadius)
        {
            _fov = new FOV(ResistanceMap);
            RecalculateFOV(playerPos, initialRadius);
        }

        public void RecalculateFOV(Coord center, int radius)
        {
            _fov.Calculate(center, radius);

            foreach (var pos in _fov.NewlySeen)
                _explored[pos.X, pos.Y] = true;
        }

        public double FOVAt(Coord position) => _fov[position];
        public double FOVAt(int x, int y) => _fov[x, y];

        // Returns the terrain at a given location, or null if no terrain has been set.
        public Terrain GetTerrain(Coord position) => _terrain[position.X, position.Y];

        // Sets terrain to the given terrain, overwriting any existing terrain, providing no MObject would collide.
        public bool SetTerrain(Terrain terrain, Coord position)
        {
            if (!terrain.IsWalkable && Raycast(position, m => !m.IsWalkable) != null)
                return false;

            _terrain[position.X, position.Y] = terrain;
            return true;
        }
    }
}
