using Geostorm.Core;
using Geostorm.GameData;
using static ImGuiNET.ImGui;

using System.Numerics;
using static MyMathLib.Geometry2D;

namespace Geostorm.Utility
{
    public class DebugMenu
    {
        public bool Shown = false;
        public Rectangle2 Window { get; private set; }

        public DebugMenu(in int screenW, in int screenH)
        {
            Window = new Rectangle2(5, screenH - 140, 200, 135);
        }

        public void UpdateAndDraw(in Game game, in GameState gameState, in GameInputs gameInputs)
        {
            if (gameInputs.DebugMenu)
                Shown = !Shown;

            if (Shown)
            {
                SetNextWindowPos (Window.O);
                SetNextWindowSize(new Vector2(Window.W, Window.H));
                if (Begin("Debug Menu"))
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
}
