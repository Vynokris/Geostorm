using System.Numerics;
using System.Collections.Generic;

using static System.MathF;
using static MyMathLib.Geometry2D;

using Geostorm.GameData;
using Geostorm.Utility;

namespace Geostorm.Core
{
    public class Weapon
    {
        public readonly Cooldown ShootCooldown = new(0.10f);
        public int   BulletsPerShot = 2;
        public float ShootDist      = 21f;
        public float SpreadDist     = 15f;
        public float SpreadAngle    = 0f;

        public Weapon() { }

        public Weapon(float shootCooldown, int bulletsPerShot, float shootDist, float spreadDist, float spreadAngle)
        {
            ShootCooldown.ChangeDuration(shootCooldown);
            BulletsPerShot = bulletsPerShot;
            ShootDist      = shootDist;
            SpreadDist     = spreadDist;
            SpreadAngle    = spreadAngle;
        }

        public void Update(float playerRotation, in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {
            if (ShootCooldown.Update(gameState.DeltaTime) && gameInputs.Shoot)
            {
                float totalDist = SpreadDist * BulletsPerShot;
                float firstDist = totalDist * -0.5f + SpreadDist / 2;

                float totalAngle = SpreadAngle * BulletsPerShot;
                float firstAngle = totalAngle * -0.5f + SpreadAngle / 2;

                for (int i = 0; i < BulletsPerShot; i++)
                {
                    float curDist  = firstDist  + SpreadDist  * i;
                    float curAngle = firstAngle + SpreadAngle * i;

                    Vector2 curPos = gameState.PlayerPos;
                    
                    curPos += Vector2FromAngle(playerRotation, ShootDist);
                    curPos.RotateAsPoint(curAngle, gameState.PlayerPos);
                    curPos += Vector2FromAngle(playerRotation, 1).GetNormal() * curDist;

                    gameEvents.Add(new BulletShotEvent(new Bullet(curPos, playerRotation + curAngle)));
                }

                ShootCooldown.Reset();
            }
        }


        // ----- Load weapon presets ----- //

        public void LoadDefault()
        {
            ShootCooldown.ChangeDuration(0.10f);
            BulletsPerShot = 2;
            ShootDist      = 21f;
            SpreadDist     = 15f;
            SpreadAngle    = 0f;
        }

        public void LoadDestroyerOfWorlds()
        {
            ShootCooldown.ChangeDuration(0.01f);
            BulletsPerShot = 40;
            ShootDist      = 21f;
            SpreadDist     = 0f;
            SpreadAngle    = PI / 20;
        }
    }
}
