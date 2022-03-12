using Geostorm.Core;
using Geostorm.Renderer;
using System.Collections.Generic;
using static MyMathLib.Geometry2D;

namespace Geostorm.GameData
{
    public class Game : IEventListener
    {
        public int Score      { get; private set; } = 0;
        public int Multiplier { get; private set; } = 1;

        public Player       player;
        public List<Bullet> bullets = new();
        public List<Geom>   geoms   = new();
        public List<Star>   stars   = new();
        public List<Enemy>  enemies = new();
        public EnemySpawner spawner = new();

        public readonly EntityVertices entityVertices = new();

        public Game(in int screenW, in int screenH)
        {
            player = new(Vector2Create(screenW/2, screenH/2));

            int StarsCount = 100;
            for (int i = 0; i < StarsCount; i++)
                stars.Add(new Star(screenW, screenH));
        }

        public void Update(ref GameState gameState, in GameInputs gameInputs)
        {
            // Reset the gameEvents to be give it to all entitites.
            List<GameEvent> gameEvents = new();

            // Update the game state.
            gameState.PlayerPos  = player.Pos;
            gameState.Score      = Score;
            gameState.Multiplier = Multiplier;

            // Update the entity collisions.
            Collisions.DoCollisions(player, bullets, enemies, entityVertices, ref gameEvents);

            // Update the stars.
            foreach (Star star in stars)
                star.Update(gameState, gameInputs, ref gameEvents);

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

            // Update the enemy spawner.
            spawner.Update(gameState, ref gameEvents);

            // Handle game events.
            HandleEvents(gameEvents);
            player.HandleEvents(gameEvents);
            foreach(Enemy enemy in enemies) 
                enemy.HandleEvents(gameEvents);

            // Delete game events.
            gameEvents = null;
        }

        public void HandleEvents(in List<GameEvent> gameEvents)
        {
            foreach (GameEvent Event in gameEvents)
            {
                if (Event is PlayerShootEvent shootEvent) {
                    bullets.Add(shootEvent.bullet);
                }

                else if (Event.GetType() == typeof(PlayerKilledEvent)) {
                    // TODO: GAME OVER.
                }

                else if (Event is GeomDespawnEvent despawnEvent) {
                    geoms.Remove(despawnEvent.geom);
                }

                else if (Event is GeomPickedUpEvent pickupEvent) {
                    geoms.Remove(pickupEvent.geom);
                    Multiplier++;
                }

                else if (Event is EnemySpawnedEvent spawnEvent) {
                    enemies.Add(spawnEvent.enemy);
                }

                else if (Event is EnemyDamagedEvent damageEvent) {
                    bullets.Remove(damageEvent.bullet);
                }

                else if (Event is EnemyKilledEvent killEvent) {
                    enemies.Remove(killEvent.enemy);
                    Score++;

                    System.Random rng = new();
                    for (int i = 0; i < rng.Next(1, 2); i++) {
                        geoms.Add(new Geom(killEvent.enemy.Pos));
                    }
                }
            }
        }

        public void Draw(in GraphicsController graphicsController)
        {
            // Draw the stars in the background.
            foreach (Star star in stars)
                graphicsController.DrawEntity(star, entityVertices);

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
