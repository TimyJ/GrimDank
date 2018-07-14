﻿using GoRogue.MapViews;
using GoRogue;
using GrimDank.Terrains;
using GoRogue.MapGeneration;
using Microsoft.Xna.Framework.Input;

//Fuck you chris for making me finally learn how to use git properly
namespace GrimDank
{
    partial class Map : IInputHandler
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

        public bool HandleKeyboard(KeyboardState state)
        {
            bool handledSomething = true;

            Direction dirToMove = Direction.NONE;

            foreach (int key in state.GetPressedKeys())
            {
                switch (key)
                {
                    case (int)Keys.NumPad6:
                        dirToMove = Direction.RIGHT;
                        break;
                    case (int)Keys.NumPad4:
                        dirToMove = Direction.LEFT;
                        break;
                    case (int)Keys.NumPad8:
                        dirToMove = Direction.UP;
                        break;
                    case (int)Keys.NumPad2:
                        dirToMove = Direction.DOWN;
                        break;
                    default:
                        handledSomething = false;
                        break;
                }

                if (handledSomething)
                    break;
            }

            if (dirToMove != Direction.NONE)
            {
                GrimDank.Player.TakeDamage(5);
                GrimDank.Player.MoveIn(dirToMove);
                // Prolly should hook be a thing that happens as an eventHandler to Player.Moved, where
                // it can simply call calculate for Player's current map.
                GrimDank.TestLevel.fov.Calculate(GrimDank.Player.Position, 23);

                foreach (var pos in GrimDank.TestLevel.fov.NewlySeen)
                {
                    GrimDank.TestLevel.SetExplored(true, pos);
                }
                // Ditto above -- hook player move event prolly preferable.
                GrimDank.MapRenderer.Camera.Area = GrimDank.MapRenderer.Camera.Area.CenterOn(GrimDank.Player.Position);
            }

            if (handledSomething)
                return true;

            return false;
        }
    }
}
