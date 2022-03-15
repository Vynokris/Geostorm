using System.Numerics;
using System.Collections.Generic;

using static System.MathF;
using static MyMathLib.Arithmetic;
using static MyMathLib.Geometry2D;

using Geostorm.GameData;
using Geostorm.Utility;

namespace Geostorm.Core
{
    public class Weapon
    {
        public readonly Cooldown ShootCooldown = new(0.10f);
        public int   BulletsPerShot = 2;
        public float FwdOffset      = 21f;
        public float SpreadDist     = 15f;
        public float SpreadAngle    = 0f;
        public float SpreadFwd      = 0f;
        public bool  DoUpgrades     = true;

        public Weapon() { }

        public Weapon(float shootCooldown, int bulletsPerShot, float fwdOffset, float spreadDist, float spreadAngle, float spreadFwd)
        {
            ShootCooldown.ChangeDuration(shootCooldown);
            BulletsPerShot = bulletsPerShot;
            FwdOffset      = fwdOffset;
            SpreadDist     = spreadDist;
            SpreadAngle    = spreadAngle;
            SpreadFwd      = spreadFwd;
        }

        public void Update(float playerRotation, in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {
            if (ShootCooldown.Update(gameState.DeltaTime) && gameInputs.Shoot)
            {
                float totalDist = SpreadDist * BulletsPerShot;
                float firstDist = totalDist * -0.5f + SpreadDist / 2;

                float totalAngle = SpreadAngle * BulletsPerShot;
                float firstAngle = totalAngle * -0.5f + SpreadAngle / 2;

                float totalFwd = SpreadFwd * BulletsPerShot;
                float firstFwd = totalFwd * -0.5f + SpreadFwd / 2;

                for (int i = 0; i < BulletsPerShot; i++)
                {
                    float curDist  = firstDist  + SpreadDist  * i;
                    float curAngle = firstAngle + SpreadAngle * i;
                    float curFwd;
                    if (i < BulletsPerShot / 2)
                        curFwd = firstFwd + SpreadFwd * i;
                    else
                        curFwd = firstFwd + SpreadFwd * (BulletsPerShot - i - 1);

                    Vector2 curPos = gameState.PlayerPos;
                    
                    curPos += Vector2FromAngle(playerRotation, FwdOffset + curFwd);
                    curPos.RotateAsPoint(curAngle, gameState.PlayerPos);
                    curPos += Vector2FromAngle(playerRotation, 1).GetNormal() * curDist;

                    gameEvents.Add(new BulletShotEvent(new Bullet(curPos, playerRotation + curAngle)));
                }

                ShootCooldown.Reset();
            }
        }


        // ----- Upgrades ----- //

        public void Upgrade(in int scoreMultiplier)
        {
            if (DoUpgrades)
            { 
                // Default.
                if (scoreMultiplier == 1) { 
                    LoadDefault();
                }
                else if (scoreMultiplier < 30) { 
                    BulletsPerShot = scoreMultiplier / 5 + 2;
                    SpreadDist     = Remap(ClampUnder(BulletsPerShot, 5), 2, 5, 15, 0);
                    SpreadAngle    = Remap(BulletsPerShot, 2, 6, 0, PI / 48);
                    ShootCooldown.ChangeDuration(Remap(scoreMultiplier, 1, 30, 0.1f, 0.25f));
                }

                // Golden falcon.
                else if (scoreMultiplier == 30)
                    LoadGoldenFalcon();
                else if (scoreMultiplier < 60) { 

                }

                // Neutron star.
                else if (scoreMultiplier == 60)
                    LoadNeutronStar();
                else if (scoreMultiplier < 100) { 
                    BulletsPerShot = (int)Remap(scoreMultiplier, 60, 100, 8, 40);
                    SpreadAngle    = Remap(BulletsPerShot, 8, 40, PI / 4, PI / 20);
                }

                // Destroyer of worlds.
                else if (scoreMultiplier == 100)
                    LoadDestroyerOfWorlds();
            }
        }


        // ----- Load weapon presets ----- //

        public void LoadDefault()
        {
            ShootCooldown.ChangeDuration(0.1f);
            BulletsPerShot = 2;
            FwdOffset      = 21f;
            SpreadDist     = 15f;
            SpreadAngle    = 0f;
            SpreadFwd      = 0f;
        }

        public void LoadShotgun()
        {
            ShootCooldown.ChangeDuration(0.4f);
            BulletsPerShot = 6;
            FwdOffset      = 21f;
            SpreadDist     = 0f;
            SpreadAngle    = PI / 48;
            SpreadFwd      = 0f;
        }

        public void LoadBow()
        {
            ShootCooldown.ChangeDuration(0.3f);
            BulletsPerShot = 9;
            FwdOffset      = 80f;
            SpreadDist     = 20f;
            SpreadAngle    = 0f;
            SpreadFwd      = 25f;
        }

        public void LoadGrenade()
        {
            ShootCooldown.ChangeDuration(0.5f);
            BulletsPerShot = 20;
            FwdOffset      = 21f;
            SpreadDist     = 0f;
            SpreadAngle    = PI / 10;
            SpreadFwd      = 0f;
        }

        public void LoadSpearOfJustice()
        {
            ShootCooldown.ChangeDuration(0.4f);
            BulletsPerShot = 15;
            FwdOffset      = 170f;
            SpreadDist     = 12.5f;
            SpreadAngle    = 0f;
            SpreadFwd      = 50f;
        }

        public void LoadGoldenFalcon()
        {
            ShootCooldown.ChangeDuration(0.25f);
            BulletsPerShot = 12;
            FwdOffset      = 21f;
            SpreadDist     = 18f;
            SpreadAngle    = -0.052f;
            SpreadFwd      = 15f;
        }

        public void LoadNeutronStar()
        {
            ShootCooldown.ChangeDuration(0.01f);
            BulletsPerShot = 8;
            FwdOffset      = 21f;
            SpreadDist     = 0f;
            SpreadAngle    = PI / 4;
            SpreadFwd      = 0f;
        }

        public void LoadDestroyerOfWorlds()
        {
            ShootCooldown.ChangeDuration(0.01f);
            BulletsPerShot = 40;
            FwdOffset      = 21f;
            SpreadDist     = 0f;
            SpreadAngle    = PI / 20;
            SpreadFwd      = 0f;
        }
    }
}
