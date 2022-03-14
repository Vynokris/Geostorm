using System.Numerics;
using System.Collections.Generic;

using static MyMathLib.Arithmetic;
using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;

using Geostorm.Utility;
using Geostorm.GameData;

namespace Geostorm.Core
{
    public class Player : Entity, IEventListener
    {
        public           int      Health        = 3;
        public  readonly Weapon   Weapon        = new();
        private readonly Cooldown DashCooldown  = new(0.30f);
        private readonly Cooldown DashingFrames = new(0.25f);
        public  readonly Cooldown Invincibility = new(3);
        private readonly int      MaxVelocity   = 10;
        private readonly int      DashVelocity  = 50;

        public Player(Vector2 pos)                                   : base(pos, Vector2Zero(), 0,        new RGBA(1, 1, 1, 1)) { }
        public Player(Vector2 pos, Vector2 velocity, float rotation) : base(pos, velocity,      rotation, new RGBA(1, 1, 1, 1)) { }

        public override void Update(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {
            // Update the player's cooldowns.
            DashCooldown .Update(gameState.DeltaTime);
            DashingFrames.Update(gameState.DeltaTime);
            Invincibility.Update(gameState.DeltaTime);

            // -- Accelerate -- //
            if (gameInputs.Movement != Vector2Zero())
            {
                // Update the game events.
                gameEvents.Add(new PlayerMoveEvent(Pos));

                // Get the direction of the joystick.
                float dirAngle = gameInputs.Movement.GetAngle();

                // Initiate a dash by going at dashing velocity in the moving dir.
                if (DashingFrames.CompletionRatio() >= 0.9f)
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
            if (gameInputs.ShootDir.Length() > 0f)
            {
                Rotation = gameInputs.ShootDir.GetAngle();
            }
            else if (gameInputs.ShootTarget != Vector2Create(-1, -1))
            {
                Rotation = Vector2FromPoints(Pos, gameInputs.ShootTarget).GetAngle();
            }

            // -- Shoot -- //
            Weapon.Update(Rotation, gameState, gameInputs, ref gameEvents);

            // -- Dash event -- //
            if (gameInputs.Dash && DashCooldown.HasEnded())
            {
                gameEvents.Add(new PlayerDashEvent());
                DashCooldown.Reset();
                DashingFrames.Reset();
            }

            // Move the player according to its velocity.
            Pos += Velocity;

            // -- Stop at screen borders -- //
            if (Pos.X < 25)
            {
                Pos      = new Vector2(25, Pos.Y);
                Velocity = new Vector2(ClampAbove(Velocity.X, 0), Velocity.Y);
            }
            if (Pos.X > gameState.ScreenSize.X-25) 
            {
                Pos      = new Vector2(gameState.ScreenSize.X-25, Pos.Y);
                Velocity = new Vector2(ClampUnder(Velocity.X, 0), Velocity.Y);
            }
            if (Pos.Y < 25)
            {
                Pos      = new Vector2(Pos.X, 25);
                Velocity = new Vector2(Velocity.X, ClampAbove(Velocity.Y, 0));
            }
            if (Pos.Y > gameState.ScreenSize.Y-25)
            {
                Pos      = new Vector2(Pos.X, gameState.ScreenSize.Y-25);
                Velocity = new Vector2(Velocity.X, ClampUnder(Velocity.Y, 0));
            }
        }

        public void HandleEvents(in List<GameEvent> gameEvents)
        {
            foreach (GameEvent Event in gameEvents)
            {
                if (Event.GetType() == typeof(PlayerDamagedEvent)) {
                    Health -= 1;
                    Invincibility.Reset();
                    break;
                }
            }
        }
    }
}
