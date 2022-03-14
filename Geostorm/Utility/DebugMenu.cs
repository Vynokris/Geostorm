using Geostorm.Core;
using Geostorm.GameData;
using static ImGuiNET.ImGui;
using static MyMathLib.Arithmetic;

namespace Geostorm.Utility
{
    public class DebugMenu
    {
        public bool Shown = false;
        
        public void UpdateAndDraw(in Game game, in GameState gameState, in GameInputs gameInputs)
        {
            if (gameInputs.DebugMenu)
                Shown = !Shown;

            if (Shown && Begin("Debug Menu"))
            {
                Text($"FPS: {gameState.FPS}");
                Text($"Delta Time: {gameState.DeltaTime}");


                int snakeBodyCount = 0;
                foreach (Enemy enemy in game.enemies)
                    if (enemy is Snake snake)
                        snakeBodyCount += snake.BodyParts.Count;

                int entitiesCount = 1 + game.stars.Count
                                      + game.particles.Count
                                      + game.bullets.Count
                                      + game.geoms.Count
                                      + game.enemies.Count
                                      + snakeBodyCount;

                Text($"Number of entities:  {entitiesCount}");

                Text($"Number of bullets:   {game.bullets.Count}");

                Text($"Number of enemies:   {game.enemies.Count}");

                Text($"Number of particles: {game.particles.Count}");
            }
        }
    }
}
