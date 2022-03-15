using Geostorm.Core;
using Geostorm.GameData;
using static ImGuiNET.ImGui;

using System.Numerics;
using static MyMathLib.Arithmetic;
using static MyMathLib.Geometry2D;

namespace Geostorm.Utility
{
    public class CheatMenu
    {
        public bool Shown = false;
        public Rectangle2 Window { get; private set; }

        private int ItemWidth = 80;
        
        private float PlayerInvincibilityDuration = 3;
        private float PlayerDashDuration          = 0.25f;
        private float PlayerShootCooldown         = 0.1f;
        private float PlayerShootSpreadAngle      = 0f;
        private int   EnemySpawnChancesSum        = 100;

        public CheatMenu(in int screenW, in int screenH)
        {
            Window = new Rectangle2(screenW - 405, 5, 400, screenH - 10);
        }

        public void UpdateAndDraw(ref Game game, in GameState gameState, in GameInputs gameInputs)
        {
            if (game.currentScene == Scenes.InGame && gameInputs.CheatMenu)
                Shown = !Shown;

            if (game.currentScene == Scenes.InGame && Shown)
            { 
                SetNextWindowPos (Window.O);
                SetNextWindowSize(new Vector2(Window.W, Window.H));
                if (Begin("Cheat Menu"))
                { 
                    System.Console.WriteLine($"{GetWindowWidth()}, {GetWindowHeight()}");
                    PushItemWidth(ItemWidth);

                    // ---- Player ---- //
                    if (CollapsingHeader("Player"))
                    {
                        if (Button("Health + 1"))
                            game.player.Health++;

                        if (PlayerInvincibilityDuration != game.player.Invincibility.Duration)
                            PlayerInvincibilityDuration  = game.player.Invincibility.Duration;

                        if (DragFloat("Shield duration", ref PlayerInvincibilityDuration, 0.01f, 0f)) {
                            PlayerInvincibilityDuration = ClampAbove(PlayerInvincibilityDuration, 0);
                            game.player.Invincibility.ChangeDuration(PlayerInvincibilityDuration);
                        }

                        if (PlayerDashDuration != game.player.DashDuration.Duration)
                            PlayerDashDuration  = game.player.DashDuration.Duration;

                        if (DragFloat("Dash duration", ref PlayerDashDuration, 0.001f, 0f)) {
                            PlayerDashDuration = ClampAbove(PlayerDashDuration, 0);
                            game.player.DashDuration.ChangeDuration(PlayerDashDuration);
                        }
                    }

                    // ---- Weapon ---- //
                    if (CollapsingHeader("Weapon"))
                    {
                        // Toggle weapon upgrades.
                        if (Button((game.player.Weapon.DoUpgrades ? "Disable upgrades" : "Enable upgrades")))
                            game.player.Weapon.DoUpgrades = !game.player.Weapon.DoUpgrades;

                        // Weapon parameters.
                        if (PlayerShootCooldown != game.player.Weapon.ShootCooldown.Duration)
                            PlayerShootCooldown  = game.player.Weapon.ShootCooldown.Duration;
                        if (DragFloat("Shoot cooldown", ref PlayerShootCooldown, 0.001f, 0f))
                            game.player.Weapon.ShootCooldown.ChangeDuration(PlayerShootCooldown);

                        DragInt("Bullets per shot", ref game.player.Weapon.BulletsPerShot, 0.1f, 0);

                        DragFloat("Forward offset", ref game.player.Weapon.FwdOffset, 0.1f, 0f);

                        DragFloat("Spread distance", ref game.player.Weapon.SpreadDist, 0.1f, 0f);

                        if (PlayerShootSpreadAngle != RadToDeg(game.player.Weapon.SpreadAngle))
                            PlayerShootSpreadAngle  = RadToDeg(game.player.Weapon.SpreadAngle);
                        if (DragFloat("Spread angle", ref PlayerShootSpreadAngle, 0.1f, 0f))
                            game.player.Weapon.SpreadAngle = DegToRad(PlayerShootSpreadAngle);

                        DragFloat("Spread forward", ref game.player.Weapon.SpreadFwd, 0.1f, 0f);

                        // Weapon presets.
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
                        if (Button("Spear Of Justice"))
                            game.player.Weapon.LoadSpearOfJustice();
                    
                        if (Button("Golden Falcon"))
                            game.player.Weapon.LoadGoldenFalcon();

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
                        // Toggle adaptative wave cooldown.
                        if (Button(game.enemySpawner.AdaptativeSpawnSpeed ? "Disable adaptative wave cooldown" : "Enable adaptative wave cooldown"))
                            game.enemySpawner.AdaptativeSpawnSpeed = !game.enemySpawner.AdaptativeSpawnSpeed;

                        // Time between 2 enemy spawning waves.
                        if (DragFloat("Time between enemy waves", ref game.enemySpawner.TimeBetweenWaves, 0.01f))
                            game.enemySpawner.TimeBetweenWaves = ClampAbove(game.enemySpawner.TimeBetweenWaves, -2f);

                        // Killing all enemies.
                        if (Button("Kill all enemies"))
                            foreach (Enemy enemy in game.enemies)
                                game.GameEvents.Add(new EnemyKilledEvent(enemy));

                        // Snake size.
                        Text("\nSnake size: ");
                        if (DragInt("Minimum", ref game.enemySpawner.SnakeMinLen, 0.1f, 0)) {
                            game.enemySpawner.SnakeMinLen = (int)Clamp(game.enemySpawner.SnakeMinLen, 1, game.enemySpawner.SnakeMaxLen);
                        }
                        if (DragInt("Maximum", ref game.enemySpawner.SnakeMaxLen, 0.1f, 0)) {
                            game.enemySpawner.SnakeMaxLen = (int)ClampAbove(game.enemySpawner.SnakeMaxLen, game.enemySpawner.SnakeMinLen);
                        }

                        // Enemy spawn probablitites.
                        bool probabilitiesChanged = false;
                        Text("\nEnemy spawning probabilities:");

                        if (DragInt("Wanderer ", ref game.enemySpawner.EnemyChances[0], 0.1f, 0)) { probabilitiesChanged = true; } SameLine();
                        if (Button("Only spawn wanderers")) { SpawnSingularEnemyType(0, ref game); }

                        if (DragInt("Rocket   ", ref game.enemySpawner.EnemyChances[1], 0.1f, 0)) { probabilitiesChanged = true; } SameLine();
                        if (Button("Only spawn rockets")) { SpawnSingularEnemyType(1, ref game); }

                        if (DragInt("Grunt    ", ref game.enemySpawner.EnemyChances[2], 0.1f, 0)) { probabilitiesChanged = true; } SameLine();
                        if (Button("Only spawn grunts")) { SpawnSingularEnemyType(2, ref game); }

                        if (DragInt("Weaver   ", ref game.enemySpawner.EnemyChances[3], 0.1f, 0)) { probabilitiesChanged = true; } SameLine();
                        if (Button("Only spawn weavers")) { SpawnSingularEnemyType(3, ref game); }

                        if (DragInt("Snake    ", ref game.enemySpawner.EnemyChances[4], 0.1f, 0)) { probabilitiesChanged = true; } SameLine();
                        if (Button("Only spawn snakes")) { SpawnSingularEnemyType(4, ref game); }

                        // Reset probablities.
                        if (Button("Reset probabilities"))
                        {
                            game.enemySpawner.EnemyChances = new int[] { 30, 20, 25, 15, 10 };
                            probabilitiesChanged = true;
                        }

                        // Make sure the sum of probabilities is 100.
                        if (probabilitiesChanged)
                        {
                            EnemySpawnChancesSum = 0;
                            for (int i = 0; i < 5; i++) {
                                game.enemySpawner.EnemyChances[i] = (int)ClampAbove(game.enemySpawner.EnemyChances[i], 0);
                                EnemySpawnChancesSum += game.enemySpawner.EnemyChances[i];
                            }
                        }

                        // Warn the user about probabilities not adding to 100.
                        if (EnemySpawnChancesSum != 100) { 
                            Text($"WARNING:\nEnemy spawn probabilities add up to {EnemySpawnChancesSum} and not 100.");
                        }
                    }

                    // ---- Cosmetic ---- //
                    if (CollapsingHeader("Cosmetic"))
                    {
                        // Number of stars.
                        if (DragInt("Number of stars", ref game.StarCount, 1f, 0))
                        {
                            game.StarCount = (int)ClampAbove(game.StarCount, 0);
                            game.stars.Clear();
                            for (int i = 0; i < game.StarCount; i++)
                                game.stars.Add(new Star((int)gameState.ScreenSize.X, (int)gameState.ScreenSize.Y));
                        }

                        // Numer of particles per kill.
                        if (DragInt("Particles per enemy killed", ref game.particleSpawner.ParticlesPerKill, 1f, 0))
                            game.particleSpawner.ParticlesPerKill = (int)ClampAbove(game.particleSpawner.ParticlesPerKill, 0);
                    }
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
