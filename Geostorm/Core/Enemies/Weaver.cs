using System.Numerics;
using System.Collections.Generic;

using static System.MathF;
using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;

using Geostorm.GameData;

namespace Geostorm.Core
{
    public class Weaver : Enemy
    {
        public Weaver() { }
        public Weaver(Vector2 pos, float preSpawnDelay = 0) : base(pos, new RGBA(0, 1, 0, 1), preSpawnDelay, 3) { }

        public override void DoUpdate(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {
            // Make the weaver move towards the player.
            Vector2 trueVelocity = Vector2FromPoints(Pos, gameState.PlayerPos).GetModifiedLength(VelocityLength);
            Velocity = trueVelocity;

            // Store info on the closest bullet.
            Vector2 closestBulletPos  = Vector2Create(-20, -20);
            float   closestBulletDist = 200;

            // Make the weaver avoid bullets.
            foreach (Bullet bullet in gameState.bullets)
            {
                Vector2 toBulletVec = Vector2FromPoints(Pos, bullet.Pos);
                float   bulletDist  = toBulletVec.Length();

                // Only consider bullets that are close to the weaver.
                if (bulletDist < 200 && bulletDist < closestBulletDist
                && closestBulletPos.GetDistanceFromPoint(bullet.Pos) > 20)
                {
                    // Get the angular position of the bullet from the weaver's pos.
                    float angle = toBulletVec.GetAngleWithVector(trueVelocity);

                    if (Abs(angle) < PI);
                    { 
                        // Decide which direction to doge in.
                        if (angle < 0)
                            Velocity = trueVelocity.GetNormal() * -1.75f;
                        else
                            Velocity = trueVelocity.GetNormal() *  1.75f;

                        // Update the closest bullet data.
                        closestBulletPos  = bullet.Pos;
                        closestBulletDist = bulletDist;
                    }
                }
            }

            // Move the weaver according to its velocity.
            Pos += Velocity;

            // Prevent the weaver from going out of the screen.
            if (Pos.X < 20) Pos = Vector2Create(20, Pos.Y);
            if (Pos.Y < 20) Pos = Vector2Create(Pos.X, 20);
            if (Pos.X > gameState.ScreenSize.X - 20) Pos = Vector2Create(gameState.ScreenSize.X - 20, Pos.Y);
            if (Pos.Y > gameState.ScreenSize.Y - 20) Pos = Vector2Create(Pos.X, gameState.ScreenSize.Y - 20);
        }
    }
}
