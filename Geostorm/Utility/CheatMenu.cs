using Geostorm.Core;
using Geostorm.GameData;
using static ImGuiNET.ImGui;
using static MyMathLib.Arithmetic;

namespace Geostorm.Utility
{
    public class CheatMenu
    {
        public bool Shown = false;

        private int itemWidth = 80;
        
        private float playerInvincibilityDuration = 3;
        private float playerDashDuration          = 0.25f;
        private float playerShootCooldown         = 0.1f;
        private float playerShootSpreadAngle      = 0f;
        private bool  enemySpawnChancesAddTo100   = true;

        public void UpdateAndDraw(ref Game game, in GameState gameState, in GameInputs gameInputs)
        {
            if (gameInputs.CheatMenu)
                Shown = !Shown;

            if (Shown && Begin("Cheat Menu"))
            { 
                PushItemWidth(itemWidth);

                // ---- Player ---- //
                if (CollapsingHeader("Player"))
                {
                    if (Button("Health + 1"))
                        game.player.Health++;
                    

                    if (DragFloat("Shield duration", ref playerInvincibilityDuration, 0.01f, 0f))
                        game.player.Invincibility.ChangeDuration(playerInvincibilityDuration);

                    if (DragFloat("Dash duration", ref playerDashDuration, 0.001f, 0f))
                        game.player.DashDuration.ChangeDuration(playerDashDuration);
                }

                // ---- Weapon ---- //
                if (CollapsingHeader("Weapon"))
                {
                    playerShootCooldown = game.player.Weapon.ShootCooldown.Duration;
                    if (DragFloat("Shoot cooldown", ref playerShootCooldown, 0.001f, 0f))
                        game.player.Weapon.ShootCooldown.ChangeDuration(playerShootCooldown);

                    DragInt("Bullets per shot", ref game.player.Weapon.BulletsPerShot, 0.1f, 0);

                    DragFloat("Forward offset", ref game.player.Weapon.FwdOffset, 0.1f, 0f);

                    DragFloat("Spread distance", ref game.player.Weapon.SpreadDist, 0.1f, 0f);

                    playerShootSpreadAngle = RadToDeg(game.player.Weapon.SpreadAngle);
                    if (DragFloat("Spread angle", ref playerShootSpreadAngle, 0.1f, 0f))
                        game.player.Weapon.SpreadAngle = DegToRad(playerShootSpreadAngle);

                    DragFloat("Spread forward", ref game.player.Weapon.SpreadFwd, 0.1f, 0f);

                    Text("Presets:");
                    if (Button("Default"))
                        game.player.Weapon.LoadDefault();

                    SameLine();
                    if (Button("Shotgun"))
                        game.player.Weapon.LoadShotgun();
                        
                    SameLine();
                    if (Button("Bow"))
                        game.player.Weapon.LoadBow();
                        
                    SameLine();
                    if (Button("Grenade"))
                        game.player.Weapon.LoadGrenade();
                        
                    SameLine();
                    if (Button("Neutron Star"))
                        game.player.Weapon.LoadNeutronStar();
                        
                    SameLine();
                    if (Button("Destroyer Of Worlds"))
                        game.player.Weapon.LoadDestroyerOfWorlds();
                }

                // ---- Enemy spawner ---- //
                if (CollapsingHeader("Enemy spawner"))
                {
                    DragFloat("Time between enemy waves", ref game.enemySpawner.TimeBetweenWaves, 0.01f, 0f);

                    bool probabilitiesChanged = false;
                    Text("\nEnemy spawning probabilities:");

                    if (DragInt("Wanderer", ref game.enemySpawner.EnemyChances[0], 0.1f, 0)) { probabilitiesChanged = true; }
                    SameLine();
                    if (Button("Only spawn wanderers")) { SpawnSingularEnemyType(0, ref game); }

                    if (DragInt("Rocket  ", ref game.enemySpawner.EnemyChances[1], 0.1f, 0)) { probabilitiesChanged = true; }
                    SameLine();
                    if (Button("Only spawn rockets")) { SpawnSingularEnemyType(1, ref game); }

                    if (DragInt("Grunt   ", ref game.enemySpawner.EnemyChances[2], 0.1f, 0)) { probabilitiesChanged = true; }
                    SameLine();
                    if (Button("Only spawn grunts")) { SpawnSingularEnemyType(2, ref game); }

                    if (DragInt("Weaver  ", ref game.enemySpawner.EnemyChances[3], 0.1f, 0)) { probabilitiesChanged = true; }
                    SameLine();
                    if (Button("Only spawn weavers")) { SpawnSingularEnemyType(3, ref game); }

                    if (DragInt("Snake   ", ref game.enemySpawner.EnemyChances[4], 0.1f, 0)) { probabilitiesChanged = true; }
                    SameLine();
                    if (Button("Only spawn snakes")) { SpawnSingularEnemyType(4, ref game); }

                    if (Button("Reset probabilities"))
                    {
                        game.enemySpawner.EnemyChances = new int[] { 30, 20, 25, 15, 10 };
                    }

                    // Make sure the sum of probabilities is 100.
                    if (probabilitiesChanged)
                    {
                        int totalChance = 0;
                        for (int i = 0; i < 5; i++)
                            totalChance += game.enemySpawner.EnemyChances[i];
                        if (totalChance != 100)
                            enemySpawnChancesAddTo100 = false;
                        else
                            enemySpawnChancesAddTo100 = true;
                    }

                    if (!enemySpawnChancesAddTo100) { 
                        SameLine();
                        Text("WARNING: Enemy spawn probabilities don't add up to 100.");
                    }

                    Text("\nSnake size: ");
                    if (DragInt("Minimum", ref game.enemySpawner.SnakeMinLen, 0.1f, 0))
                        if (game.enemySpawner.SnakeMinLen > game.enemySpawner.SnakeMaxLen)
                            game.enemySpawner.SnakeMinLen = game.enemySpawner.SnakeMaxLen;
                    if (DragInt("Maximum", ref game.enemySpawner.SnakeMaxLen, 0.1f, 0))
                        if (game.enemySpawner.SnakeMaxLen < game.enemySpawner.SnakeMinLen)
                            game.enemySpawner.SnakeMaxLen = game.enemySpawner.SnakeMinLen;
                }

                // ---- Cosmetic ---- //
                if (CollapsingHeader("Cosmetic"))
                {
                    if (DragInt("Number of stars", ref game.StarCount, 1f, 0))
                    {
                        game.stars.Clear();
                        for (int i = 0; i < game.StarCount; i++)
                            game.stars.Add(new Star((int)gameState.ScreenSize.X, (int)gameState.ScreenSize.Y));
                    }

                    DragInt("Particles per enemy killed", ref game.particleSpawner.ParticlesPerKill, 1f, 0);
                }
            }
        }

        private void SpawnSingularEnemyType(int type, ref Game game)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == type)
                    game.enemySpawner.EnemyChances[i] = 100;
                else
                    game.enemySpawner.EnemyChances[i] = 0;
            }
        }
    }
}
