using Geostorm.Core;
using Geostorm.Renderer;
using System.Collections.Generic;
using System;

namespace Geostorm.GameData
{
    public class Game
    {
        public Player player        = new();
        public List<Enemy> enemies  = new();
        public List<Bullet> bullets = new();

        public void Update(GameInputs inputs)
        {
            player.Update(inputs);
        }

        public void Draw(in RaylibController raylibController)
        {
            raylibController.DrawEntity(player);
            raylibController.DrawEntity(bullets[0]);
            raylibController.DrawEntity(enemies[0]);
        }
    }
}
