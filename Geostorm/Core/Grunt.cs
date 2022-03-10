﻿using System.Numerics;
using static MyMathLib.Geometry2D;
using Geostorm.GameData;

namespace Geostorm.Core
{
    public class Grunt : Enemy
    {
        public Grunt() { }
        public Grunt(Vector2 pos, int spawnDelay) : base(pos, 1, spawnDelay) { }

        public override void DoUpdate(in GameState gameState, in GameInputs gameInputs, ref GameEvents gameEvents)
        {
            // Make the grunt move towards the player.
            Velocity = Velocity.GetModifiedAngle(Vector2FromPoints(Pos, gameState.PlayerPos).GetAngle());

            // Move the grunt according to its velocity.
            Pos += Velocity;
        }
    }
}