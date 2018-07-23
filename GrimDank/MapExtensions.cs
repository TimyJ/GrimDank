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
        public bool EnemyStatusToggle;
        public Targetting Targetter;

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

        public void SetupFOV(Coord playerPos)
        {
            fov = new FOV(ResistanceMap);
            fov.Calculate(playerPos, 23); // TODO: Just flagging magic constant (FOV Radius)
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
            bool handledSomething = false;

            Direction dirToMove = Direction.NONE;
            EnemyStatusToggle = false;

            foreach (int key in state.GetPressedKeys())
            {
                handledSomething = true;
                switch (key)
                {
                    case (int)Keys.NumPad6:
                    case (int)Keys.L:
                        dirToMove = Direction.RIGHT;
                        break;
                    case (int)Keys.NumPad4:
                    case (int)Keys.H:
                        dirToMove = Direction.LEFT;
                        break;
                    case (int)Keys.NumPad8:
                    case (int)Keys.K:
                        dirToMove = Direction.UP;
                        break;
                    case (int)Keys.NumPad2:
                    case (int)Keys.J:
                        dirToMove = Direction.DOWN;
                        break;
                    case (int)Keys.T:
                        if (Targetter == null)
                        {
                            Targetter = new Targetting(GrimDank.Instance.Player.Position, c => MessageLog.Write($"Targeted {c}"));
                            InputStack.Add(Targetter);
                        }
                        break;
                    case (int)Keys.LeftAlt:
                        EnemyStatusToggle = true;
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
                if(!GrimDank.Instance.Player.MoveIn(dirToMove))
                {
                    MObjects.MObject mobject = Raycast(GrimDank.Instance.Player.Position + dirToMove);
                    if (mobject is MObjects.Creature mob) // Nice shorthand -- if mobjet is Creature, call it mob and let me work with it
                    {
                        mob.TakeDamage(10);
                    }
                }
                // Prolly should hook be a thing that happens as an eventHandler to Player.Moved, where
                // it can simply call calculate for Player's current map.
                GrimDank.Instance.TestLevel.fov.Calculate(GrimDank.Instance.Player.Position, 23);

                foreach (var pos in GrimDank.Instance.TestLevel.fov.NewlySeen)
                {
                    GrimDank.Instance.TestLevel.SetExplored(true, pos);
                }
                // Ditto above -- hook player move event prolly preferable.
                GrimDank.Instance.MapRenderer.Camera.Area = GrimDank.Instance.MapRenderer.Camera.Area.CenterOn(GrimDank.Instance.Player.Position);
            }

            return handledSomething;
        }
    }
}
