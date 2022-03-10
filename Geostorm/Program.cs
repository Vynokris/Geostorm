using Geostorm.Core;
using Geostorm.Renderer;
using Geostorm.GameData;

namespace Geostorm
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            // ----- Initialization ----- //

            const int screenWidth  = 1920;
            const int screenHeight = 1080;

            Game               game             = new(screenWidth, screenHeight);
            GraphicsController graphicsController = new(screenWidth, screenHeight);
            ImguiController    imguiController  = new();
            imguiController.Load(screenWidth, screenHeight);

            game.enemies.Add(new Grunt   (new System.Numerics.Vector2(screenWidth - 100, screenHeight/2), 0));
            game.enemies.Add(new Wanderer(new System.Numerics.Vector2(              100, screenHeight/2), 0));

            // ----- Main game loop ----- //

            while (!graphicsController.WindowShouldClose())
            {
                // ----- Update ----- //

                // Get the game inputs from Raylib and update ImGui.
                GameInputs gameInputs = graphicsController.GetInputs();
                GameState  gameState = graphicsController.GetGameState();
                imguiController.Update(gameState.DeltaTime);

                // Update the game.
                game.Update(gameState, gameInputs);


                // ----- Draw ----- //

                graphicsController.BeginDrawing();

                game.Draw(graphicsController);

                imguiController.Draw();

                graphicsController.EndDrawing();
            }


            // ----- DeInitialization ----- //

            imguiController.Dispose();
            graphicsController.Dispose();
        }
    }
}
