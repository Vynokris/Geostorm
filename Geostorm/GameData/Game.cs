using Geostorm.Core;
using Geostorm.Renderer;
using System.Collections.Generic;
using static MyMathLib.Geometry2D;

namespace Geostorm.GameData
{
    public class Game
    {
        public double GameDuration = 0;
        public int    Score        = 0;

        public Player       player;
        public List<Bullet> bullets = new();
        public List<Star>   stars   = new();
        public List<Enemy>  enemies = new();
        public EnemySpawner spawner;

        public readonly EntityVertices entityVertices = new();

        public Game(in int screenW, in int screenH)
        {
            player  = new(Vector2Create(screenW/2, screenH/2));
            spawner = new(screenW, screenH);

            int StarsCount = 100;
            for (int i = 0; i < StarsCount; i++)
                stars.Add(new Star(screenW, screenH));
        }

        public void Update(GameState gameState, GameInputs gameInputs)
        {
            // Update the game duration.
            GameDuration += gameState.DeltaTime;

            // Reset the gameEvents to be give it to all entitites.
            GameEvents gameEvents = new();

            // Update the game state with entity information.
            gameState.PlayerPos    = player.Pos;
            gameState.GameDuration = GameDuration;
            gameState.Score        = Score;

            // Update the entity collisions.
            Collisions.DoCollisions(ref player, ref bullets, ref enemies, entityVertices);

            // Update the stars.
            foreach (Star star in stars)
                star.Update(gameState, gameInputs, ref gameEvents);

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
            // Draw the stars in the background.
            foreach (Star star in stars)
                graphicsController.DrawEntity(star, entityVertices);

            // Draw the enemies and their spawn animations.
            foreach(Enemy  enemy  in enemies) 
                graphicsController.DrawEntity(enemy, entityVertices);

            // Draw the bullets.
            foreach(Bullet bullet in bullets) 
                graphicsController.DrawEntity(bullet, entityVertices);

            // Draw the player and its invincibility shield.
            graphicsController.DrawEntity(player, entityVertices);

            // Draw the cursor.
            graphicsController.DrawCursor(entityVertices);
        }
    }
}
