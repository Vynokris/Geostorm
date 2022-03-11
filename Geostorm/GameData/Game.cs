using Geostorm.Core;
using Geostorm.Renderer;
using System.Collections.Generic;
using static MyMathLib.Geometry2D;

namespace Geostorm.GameData
{
    public class Game
    {
        public double GameDuration = 0;
        public Player player;
        public List<Enemy> enemies  = new();
        public List<Bullet> bullets = new();
        public EnemySpawner spawner;
        public readonly EntityVertices entityVertices = new();

        public Game(in int screenW, in int screenH)
        {
            player  = new(Vector2Create(screenW/2, screenH/2));
            spawner = new(screenW, screenH);
        }

        public void Update(GameState gameState, GameInputs gameInputs)
        {
            // Update the game duration.
            GameDuration += gameState.DeltaTime;

            // Reset the gameEvents to be give it to all entitites.
            GameEvents gameEvents = new();

            // Update the game state with entity information.
            gameState.PlayerPos = player.Pos;

            // Update the entity collisions.
            Collisions.DoCollisions(ref player, ref bullets, ref enemies, entityVertices);

            // Update the player.
            player.Update(gameState, gameInputs, ref gameEvents);

            // Add a bullet if the player is shooting.
            if (gameEvents.PlayerShooting)
                player.Shoot(ref bullets);

            // Update the bullets.
            for (int i = bullets.Count-1; i >= 0; i--) 
            {
                bullets[i].Update(gameState, gameInputs, ref gameEvents);
                if (bullets[i].Health <= 0)
                    bullets.Remove(bullets[i]);
            }

            // Update the enemy spawner.
            spawner.Update(ref enemies, GameDuration);

            // Update the enemies.
            for (int i = enemies.Count-1; i >= 0; i--)
            {
                enemies[i].Update(gameState, gameInputs, ref gameEvents);
                if (enemies[i].Health <= 0)
                    enemies.Remove(enemies[i]);
            }
        }

        public void Draw(in GraphicsController graphicsController)
        {
            // Draw the enemies and bullets.
            foreach(Enemy  enemy  in enemies) 
            {
                if (enemy.SpawnDelay.Counter <= 0)
                    graphicsController.DrawEntity(enemy, entityVertices);
                else
                    graphicsController.DrawLines(entityVertices.GetEntitySpawnAnimation(enemy, enemy.SpawnDelay), entityVertices.GetEntityColor(enemy));
            }
            foreach(Bullet bullet in bullets) graphicsController.DrawEntity(bullet, entityVertices);

            // Draw the player and its invincibility shield.
            graphicsController.DrawEntity(player, entityVertices);
            graphicsController.DrawPlayerShield(player.Pos, player.Invincibility.Counter);
            graphicsController.DrawCursor(entityVertices, enemies);
        }
    }
}
