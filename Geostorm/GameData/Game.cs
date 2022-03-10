using Geostorm.Core;
using Geostorm.Renderer;
using System;
using System.Collections.Generic;

namespace Geostorm.GameData
{
    public class Game
    {
        public Player player        = new();
        public List<Enemy> enemies  = new();
        public List<Bullet> bullets = new();
        public readonly EntityVertices entityVertices = new();

        public void Update(GameState gameState, GameInputs gameInputs)
        {
            // Reset the gameEvents to be give it to all entitites.
            GameEvents gameEvents = new();

            // Update the game state with entity information.
            gameState.PlayerPos = player.Pos;

            // Update the entity collisions.
            Collisions.DoCollisions(ref player, ref bullets, ref enemies, entityVertices);

            // Update the player.
            player.Update(gameState, gameInputs, ref gameEvents);

            // Update the bullets.
            for (int i = bullets.Count-1; i >= 0; i--) 
            {
                bullets[i].Update(gameState, gameInputs, ref gameEvents);
                if (bullets[i].Health <= 0)
                    bullets.Remove(bullets[i]);
            }

            // Update the enemies.
            for (int i = enemies.Count-1; i >= 0; i--)
            {
                enemies[i].Update(gameState, gameInputs, ref gameEvents);
                if (enemies[i].Health <= 0)
                    enemies.Remove(enemies[i]);
            }

            // Add a bullet if the player is shooting.
            if (gameEvents.PlayerShooting)
                player.Shoot(ref bullets);
        }

        public void Draw(in RaylibController raylibController)
        {
            raylibController.DrawEntity(player, entityVertices);
            if (!player.Invincibility.HasEnded())
                raylibController.DrawPlayerShield(player.Pos, player.Invincibility.Counter);
            foreach(Enemy  enemy  in enemies) raylibController.DrawEntity(enemy, entityVertices);
            foreach(Bullet bullet in bullets) raylibController.DrawEntity(bullet, entityVertices);
        }
    }
}
