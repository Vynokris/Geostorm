using System. Numerics;
using System.Collections.Generic;

using static MyMathLib.Arithmetic;
using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;

using Geostorm.GameData;
using Geostorm.Utility;

namespace Geostorm.Core
{
    public class Geom : Entity
    {
        public readonly Cooldown DespawnTimer = new(3);
        private float RotationSpeed;

        public Geom() { }
        public Geom(Vector2 pos) 
        {
            Pos = pos;
            System.Random rnd = new();
            Rotation = DegToRad(rnd.Next(0, 360));
            RotationSpeed = DegToRad(rnd.Next(1, 5));
            Velocity = Vector2FromAngle(Rotation, 2.5f);
            Color = new RGBA(0.54f, 0.68f, 0.33f, 1);
        }

        public override void Update(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {
            float   pickupDist  = 5;
            float   magnetDist  = 100;
            Vector2 vecToPlayer = Vector2FromPoints(Pos, gameState.PlayerPos);

            // Get picked up by the player.
            if (vecToPlayer.Length() < pickupDist)
                gameEvents.Add(new GeomPickedUpEvent(this));

            // Move towards the player when it is close.
            else if (vecToPlayer.Length() < magnetDist)
                Velocity = vecToPlayer.GetModifiedLength(10);

            // Slow down.
            else
                Velocity  *= 0.97f;
            RotationSpeed *= 0.98f;

            // Despawn.
            if (DespawnTimer.Update(gameState.DeltaTime))
                gameEvents.Add(new GeomDespawnEvent(this));

            // Bounce on screen edges.
            if (0 > Pos.X || Pos.X > gameState.ScreenSize.X)
                Velocity = new Vector2(-Velocity.X, Velocity.Y);
            if (0 > Pos.Y || Pos.Y > gameState.ScreenSize.Y)
                Velocity = new Vector2(Velocity.X, -Velocity.Y);

            // Rotate according to rotation speed.
            Rotation += RotationSpeed;

            // Move according to velocity.
            Pos += Velocity;
        }
    }
}
