using System.Numerics;
using System.Collections.Generic;
using static MyMathLib.Arithmetic;
using static MyMathLib.Geometry2D;
using Geostorm.Utility;
using Geostorm.GameData;

namespace Geostorm.Core
{
    public class Player : Entity
    {
        public int Score { get; private set; }  = 0;
        private readonly Cooldown ShootCooldown = new(5);
        private readonly Cooldown DashCooldown  = new(60);
        private readonly Cooldown DashingFrames = new(15);
        public  readonly Cooldown Invincibility = new(60*3);
        private readonly int MaxVelocity        = 10;
        private readonly int DashVelocity       = 50;

        public Player() { DashingFrames.Counter = 0; }
        public Player(Vector2 pos, Vector2 velocity, float rotation) : base(pos, velocity, rotation, 3) { }

        public override void Update(in GameState gameState, in GameInputs gameInputs, ref GameEvents gameEvents)
        {
            // Update the player's cooldowns.
            ShootCooldown.Update();
            DashCooldown.Update();
            DashingFrames.Update();
            Invincibility.Update();

            // -- Accelerate -- //
            if (gameInputs.Movement != Vector2Zero())
            {
                // Update the game events.
                gameEvents.PlayerMoving = true;

                // Get the direction of the joystick.
                float dirAngle = gameInputs.Movement.GetAngle();

                // Initiate a dash by going at dashing velocity in the moving dir.
                if (DashingFrames.CompletionRatio() == 1)
                    Velocity = Vector2FromAngle(dirAngle, DashVelocity);

                // If the player is under the maximum velocity, make him accelerate.
                else if (Velocity.Length() < MaxVelocity)
                    Velocity += Vector2FromAngle(dirAngle, 0.6f);

                // If the player is at maximum velocity, stop accelerating.
                else if (DashingFrames.HasEnded())
                    Velocity = Velocity * 0.7f + Vector2FromAngle(dirAngle, 0.3f);
            }

            // -- Slow down -- //
            else if (Velocity.Length() > 0)
            {
                Velocity *= 0.97f;
            }

            // -- Dash slow down -- //
            if (!DashingFrames.HasEnded() && Velocity.Length() > MaxVelocity)
            {
                Velocity = Velocity.GetModifiedLength(ClampAbove(DashVelocity * DashingFrames.CompletionRatio(), MaxVelocity));
            }

            // -- Turn -- //
            if (gameInputs.ShootDir != Vector2Zero())
            {
                Rotation = gameInputs.ShootDir.GetAngle();
            }
            else
            {
                Rotation = Vector2FromPoints(Pos, gameInputs.ShootTarget).GetAngle();
            }

            // -- Shoot -- //
            if (gameInputs.Shoot)
            {
                gameEvents.PlayerShooting = true;
            }

            // -- Dash -- //
            if (gameInputs.Dash)
            {
                gameEvents.PlayerDashing = true;
                Dash();
            }

            // Move the player according to its velocity.
            Pos += Velocity;
        }

        public void Shoot(ref List<Bullet> bullets)
        {
            if (ShootCooldown.HasEnded())
            {
                ShootCooldown.Reset();
                bullets.Add(new Bullet(Pos + Vector2FromAngle(Rotation, -6).GetNormal() + Vector2FromAngle(Rotation, 21), Rotation));
                bullets.Add(new Bullet(Pos + Vector2FromAngle(Rotation,  6).GetNormal() + Vector2FromAngle(Rotation, 21), Rotation));
            }
        }

        public void Dash()
        {
            if (DashCooldown.HasEnded())
            {
                DashCooldown.Reset();
                DashingFrames.Reset();
            }
        }
    }
}
