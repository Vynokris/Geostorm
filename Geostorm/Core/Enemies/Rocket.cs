using System.Numerics;
using System.Collections.Generic;

using static System.MathF;
using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;

using Geostorm.GameData;

namespace Geostorm.Core
{
    public class Rocket : Enemy
    {
        public Rocket() { }
        public Rocket(Vector2 pos, float preSpawnDelay = 0) : base(pos, new RGBA(1, 0.8f, 0.34f, 1), preSpawnDelay) 
        { 
            System.Random rng = new();
            Rotation = rng.Next(0, 4) * PI / 2;
            Velocity = Vector2Create(10, 0).GetRotated(Rotation);
        }

        public override void DoUpdate(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {
            // Move the rocket according to its velocity.
            Pos += Velocity;

            // Bounce on screen edges.
            if (0 > Pos.X || Pos.X > gameState.ScreenSize.X) { 
                Velocity = new Vector2(-Velocity.X, Velocity.Y);
                Rotation = (Rotation+PI) % (2*PI);
            }
            if (0 > Pos.Y || Pos.Y > gameState.ScreenSize.Y) { 
                Velocity = new Vector2(Velocity.X, -Velocity.Y);
                Rotation = (Rotation+PI) % (2*PI);
            }
        }
    }
}
