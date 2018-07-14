using GoRogue.MapViews;
using GoRogue;
using GrimDank.Terrains;
using GoRogue.MapGeneration;
//Fuck you chris for making me finally learn how to use git properly
namespace GrimDank
{
    partial class Map
    {
        private Terrain[,] _terrain;
        private bool[,] _explored;
        public FOV fov;
        public ResistanceProvider ResistanceMap { get; private set; }
        public WalkabilityProvider WalkabilityMap { get; private set; }

        public void GenerateMap()
        {
            ArrayMap<bool> genMap = new ArrayMap<bool>(Width, Height);
            GoRogue.MapGeneration.Generators.CellularAutomataGenerator.Generate(genMap);
            foreach(var pos in genMap.Positions())
            {
                if (genMap[pos])
                {
                    _terrain[pos.X, pos.Y] = Terrain.FLOOR;
                } else { _terrain[pos.X, pos.Y] = Terrain.WALL; }
            }
        }

        public void SetupFOV(Coord playerPos)
        {
            fov = new FOV(ResistanceMap);
            fov.Calculate(playerPos, 23);
            foreach (var pos in fov.CurrentFOV)
            {
                SetExplored(true, pos);
            }
        }

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
