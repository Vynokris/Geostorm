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
        public GameEvents gameEvents;

        public void Update(GameInputs inputs)
        {
            // Reset the gameEvents to be give it to all entitites.
            gameEvents = new();

            // Update all entities.
            player.Update(inputs, ref gameEvents);
            foreach(Enemy  enemy  in enemies)  enemy.Update(inputs, ref gameEvents);
            for (int i = bullets.Count-1; i >= 0; i--) 
            {
                bullets[i].Update(inputs, ref gameEvents);
                if (bullets[i].destroyed)
                    bullets.Remove(bullets[i]);
            }

            // Add a bullet if the player is shooting.
            if (gameEvents.playerShooting)
                player.Shoot(ref bullets);
        }

        public void Draw(in RaylibController raylibController)
        {
            raylibController.DrawEntity(player);
            foreach(Enemy  enemy  in enemies) raylibController.DrawEntity(enemy);
            foreach(Bullet bullet in bullets) raylibController.DrawEntity(bullet);
        }
    }
}
