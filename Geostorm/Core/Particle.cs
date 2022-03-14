using System.Numerics;
using System.Collections.Generic;

using static MyMathLib.Arithmetic;
using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;

using Geostorm.GameData;
using Geostorm.Utility;

namespace Geostorm.Core
{
    public class Particle : Entity
    {
        public readonly Cooldown DespawnTimer = new(0);

        public Particle() { }
        public Particle(Vector2 pos, RGBA color = new(), float despawnTime = 0)
        {
            System.Random rng = new();

            Pos = pos;
            Rotation = DegToRad(rng.Next(0, 360));
            Velocity = Vector2FromAngle(Rotation, rng.Next(200, 400) / 10f);

            // Choose a random despawn time between 0.5 and 1 seconds.
            if (despawnTime == 0) { 
                DespawnTimer.ChangeDuration(rng.Next(30, 60) / 60f);
            }

            // Choose a random color or use the given one.
            if (color.A == 0) { 
                Color = new RGBA(rng.Next(0, 255) / 255f, 
                                 rng.Next(0, 255) / 255f, 
                                 rng.Next(0, 255) / 255f, 1f);
            }
            else { 
                Color = color;
            }
        }

        public override void Update(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {
            // Slow down and move according to velocity.
            Velocity *= 0.95f;
            Pos += Velocity;

            // Update the despawn timer.
            if (DespawnTimer.Update(gameState.DeltaTime))
                gameEvents.Add(new ParticleDespawnEvent(this));

            // Make the particle smaller according to the timer.
            Scale = Vector2Create(DespawnTimer.CompletionRatio(), DespawnTimer.CompletionRatio());

            // Bounce on screen edges.
            if (0 > Pos.X || Pos.X > gameState.ScreenSize.X)
                Velocity = new Vector2(-Velocity.X, Velocity.Y);
            if (0 > Pos.Y || Pos.Y > gameState.ScreenSize.Y)
                Velocity = new Vector2(Velocity.X, -Velocity.Y);
        }
    }
}
