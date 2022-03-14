using System.Collections.Generic;

using static MyMathLib.Geometry2D;

using Geostorm.Core;
using Geostorm.Renderer;
using Geostorm.Utility;

namespace Geostorm.GameData
{
    public class Game : IEventListener
    {
        public int Score      { get; private set; } = 0;
        public int Multiplier { get; private set; } = 1;

        public int StarCount = 100;
        public List<Star>     stars     = new();
        public List<Particle> particles = new(); 

        public Player       player;
        public List<Bullet> bullets = new();
        public List<Geom>   geoms   = new();
        public List<Enemy>  enemies = new();

        public EnemySpawner    enemySpawner    = new();
        public ParticleSpawner particleSpawner = new();


        public readonly EntityVertices entityVertices = new();

        public Game(in int screenW, in int screenH)
        {
            player = new(Vector2Create(screenW/2, screenH/2));

            for (int i = 0; i < StarCount; i++)
                stars.Add(new Star(screenW, screenH));
        }

        public void Update(ref GameState gameState, in GameInputs gameInputs)
        {
            // Reset the gameEvents to be give it to all entitites.
            List<GameEvent> gameEvents = new();

            // Update the game state.
            gameState.Score      = Score;
            gameState.Multiplier = Multiplier;
            gameState.PlayerPos  = player.Pos;
            gameState.bullets    = bullets;

            // Update the entity collisions.
            Collisions.DoCollisions(player, bullets, enemies, entityVertices, ref gameEvents);

            // Update the stars.
            foreach (Star star in stars)
                star.Update(gameState, gameInputs, ref gameEvents);

            // Update the particles.
            foreach (Particle particle in particles)
                particle.Update(gameState, gameInputs, ref gameEvents);

            // Update the player.
            player.Update(gameState, gameInputs, ref gameEvents);

            // Update the bullets.
            foreach (Bullet bullet in bullets) 
                bullet.Update(gameState, gameInputs, ref gameEvents);

            // Update the geoms.
            foreach (Geom geom in geoms)
                geom.Update(gameState, gameInputs, ref gameEvents);

            // Update the enemies.
            foreach (Enemy enemy in enemies)
                enemy.Update(gameState, gameInputs, ref gameEvents);

            // Update the entity spawners.
            enemySpawner.Update(gameState, ref gameEvents);
            particleSpawner.Update(ref gameEvents);

            // Handle game events.
            HandleEvents(gameEvents);
            player.HandleEvents(gameEvents);
        }

        public void HandleEvents(in List<GameEvent> gameEvents)
        {
            foreach (GameEvent gameEvent in gameEvents)
            {
                switch (gameEvent)
                {
                    case PlayerKilledEvent killedEvent:
                        // TODO: GAME OVER.
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
                        break;

                    case EnemySpawnedEvent spawnEvent:
                        enemies.Add(spawnEvent.enemy);
                        break;

                    case EnemyKilledEvent killEvent:
                        enemies.Remove(killEvent.enemy);
                        Score++;

                        System.Random rng = new();
                        for (int i = 0; i < rng.Next(1, 2); i++) {
                            geoms.Add(new Geom(killEvent.enemy.Pos));
                        }
                        break;

                    case ParticleSpawnedEvent particleSpawnEvent:
                        particles.Add(particleSpawnEvent.particle);
                        break;

                    case ParticleDespawnEvent particleDespawnEvent:
                        particles.Remove(particleDespawnEvent.particle);
                        break;

                    default: 
                        break;
                }    
            }
        }

        public void Draw(in GraphicsController graphicsController)
        {
            // Draw the stars in the background.
            foreach (Star star in stars)
                graphicsController.DrawEntity(star, entityVertices);

            // Draw the particles.
            foreach (Particle particle in particles)
                graphicsController.DrawEntity(particle, entityVertices);

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

            // Draw the cursor.
            graphicsController.DrawCursor(entityVertices);
        }
    }
}
