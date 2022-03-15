using System.Collections.Generic;

using static MyMathLib.Geometry2D;

using Geostorm.Core;
using Geostorm.Renderer;
using Geostorm.Utility;

namespace Geostorm.GameData
{
    public enum Scenes
    {
        MainMenu = 0,
        InGame   = 1,
        GameOver = 2,
    }

    public class Game : IEventListener
    {
        public int  Score      { get; private set; } = 0;
        public int  Multiplier { get; private set; } = 1;
        public Cooldown MultiplierResetCooldown = new(100);

        public List<GameEvent> GameEvents = new();

        public int StarCount = 100;
        public List<Star>     stars       = new();
        public List<Particle> particles   = new(); 
        public WarpingGrid    warpingGrid;

        public Player       player;
        public List<Bullet> bullets = new();
        public List<Geom>   geoms   = new();
        public List<Enemy>  enemies = new();

        public EnemySpawner    enemySpawner    = new();
        public ParticleSpawner particleSpawner = new();

        public Ui ui = new();
        public Scenes currentScene = Scenes.MainMenu;


        public readonly EntityVertices entityVertices = new();

        public Game(in int screenW, in int screenH)
        {
            player = new(Vector2Create(screenW/2, screenH/2));
            ui.LoadHealthTexture(entityVertices.PlayerVertices);

            for (int i = 0; i < StarCount; i++)
                stars.Add(new Star(screenW, screenH));
        }

        public void Update(ref GameState gameState, in GameInputs gameInputs)
        {
            // Update the game state.
            gameState.Score      = Score;
            gameState.Multiplier = Multiplier;
            gameState.PlayerPos  = player.Pos;
            gameState.bullets    = bullets;

            // Update the stars.
            foreach (Star star in stars)
                star.Update(gameState, gameInputs, ref GameEvents);

            // Update the particles.
            foreach (Particle particle in particles)
                particle.Update(gameState, gameInputs, ref GameEvents);

            switch (currentScene)
            { 
                // ----- Main menu update ----- //
                case Scenes.MainMenu:
                    if (gameInputs.Dash)
                        currentScene = Scenes.InGame;
                    break;

                // ----- Game over update ----- //
                case Scenes.GameOver:
                    if (gameInputs.Dash) 
                    { 
                        player = new Player(gameState.ScreenSize / 2);
                        bullets.Clear();
                        geoms.Clear();
                        enemies.Clear();
                        Score = 0;
                        Multiplier = 1;
                        gameState.GameDuration = 0;
                        MultiplierResetCooldown.ChangeDuration(100);
                        currentScene = Scenes.InGame;
                    }
                    break;

                // ----- In game update ----- //
                case Scenes.InGame:

                    // Update the game duration.
                    gameState.GameDuration += gameState.DeltaTime;

                    // Update the multiplier reset cooldown.
                    if (MultiplierResetCooldown.Update(gameState.DeltaTime)) {
                        Multiplier = 1;
                        MultiplierResetCooldown.ChangeDuration(100);
                        player.Weapon.Upgrade(Multiplier);
                    }

                    // Update the entity collisions.
                    Collisions.DoCollisions(player, bullets, enemies, entityVertices, ref GameEvents);

                    // Update the player.
                    player.Update(gameState, gameInputs, ref GameEvents);

                    // Update the bullets.
                    foreach (Bullet bullet in bullets) 
                        bullet.Update(gameState, gameInputs, ref GameEvents);

                    // Update the geoms.
                    foreach (Geom geom in geoms)
                        geom.Update(gameState, gameInputs, ref GameEvents);

                    // Update the enemies.
                    foreach (Enemy enemy in enemies)
                        enemy.Update(gameState, gameInputs, ref GameEvents);

                    // Update the entity spawners.
                    enemySpawner.Update(gameState, ref GameEvents);
                    particleSpawner.Update(ref GameEvents);
                    break;
            }
        }

        public void HandleEvents(in List<GameEvent> gameEvents)
        {
            foreach (GameEvent gameEvent in gameEvents)
            {
                switch (gameEvent)
                {
                    case PlayerDamagedEvent damageEvent:
                        Multiplier = 1;
                        MultiplierResetCooldown.ChangeDuration(100);
                        player.Weapon.Upgrade(Multiplier);
                        break;

                    case PlayerKilledEvent killedEvent:
                        currentScene = Scenes.GameOver;
                        break;

                    case BulletShotEvent shootEvent:
                        bullets.Add(shootEvent.bullet);
                        break;

                    case BulletDestroyedEvent destroyEvent:
                        bullets.Remove(destroyEvent.bullet);
                        break;

                    case GeomDespawnEvent despawnEvent:
                        geoms.Remove(despawnEvent.geom);
                        break;

                    case GeomPickedUpEvent pickupEvent:
                        geoms.Remove(pickupEvent.geom);
                        Multiplier++;
                        player.Weapon.Upgrade(Multiplier);
                        MultiplierResetCooldown.ChangeDuration(100f / Multiplier);
                        break;

                    case EnemySpawnedEvent spawnEvent:
                        enemies.Add(spawnEvent.enemy);
                        break;

                    case EnemyKilledEvent killEvent:
                        enemies.Remove(killEvent.enemy);
                        Score += Multiplier;

                        System.Random rng = new();
                        for (int i = 0; i < rng.Next(1, 2); i++) {
                            geoms.Add(new Geom(killEvent.enemy.Pos));
                        }
                        break;

                    case ParticleSpawnedEvent particleSpawnEvent:
                        for (int i = 0; i < particleSpawnEvent.Count; i++)
                            particles.Add(new Particle(particleSpawnEvent.Pos, particleSpawnEvent.Color));
                        break;

                    case ParticleDespawnEvent particleDespawnEvent:
                        particles.Remove(particleDespawnEvent.particle);
                        break;

                    default: 
                        break;
                }    
            }

            // Handle events for the player and ui.
            player.HandleEvents(GameEvents);
            ui.HandleEvents(GameEvents);
        }

        public void Draw(in GraphicsController graphicsController)
        {
            // Update the graphic contoller's shake offset.
            graphicsController.SetShakeOffset(ui.ShakeOffset);

            // Draw the stars in the background.
            foreach (Star star in stars)
                graphicsController.DrawEntity(star, entityVertices);

            // Draw the particles.
            foreach (Particle particle in particles)
                graphicsController.DrawEntity(particle, entityVertices);

            if (currentScene == Scenes.InGame)
            { 
                // Draw the enemies and their spawn animations.
                foreach (Enemy  enemy  in enemies) 
                    graphicsController.DrawEntity(enemy, entityVertices);

                // Draw the bullets.
                foreach (Bullet bullet in bullets) 
                    graphicsController.DrawEntity(bullet, entityVertices);

                // Draw the geoms.
                foreach (Geom geom in geoms)
                    graphicsController.DrawEntity(geom, entityVertices);

                // Draw the player and its invincibility shield.
                graphicsController.DrawEntity(player, entityVertices);
            }

            // Draw the cursor.
            graphicsController.DrawCursor(entityVertices);
        }
    }
}
