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

            Game             game             = new(screenWidth, screenHeight);
            RaylibController raylibController = new(screenWidth, screenHeight);
            ImguiController  imguiController  = new();
            imguiController.Load(screenWidth, screenHeight);

            game.enemies.Add(new Grunt(new System.Numerics.Vector2(screenWidth/2 + 100, screenHeight/2), 0));

            // ----- Main game loop ----- //

            while (!raylibController.WindowShouldClose())
            {
                // ----- Update ----- //

                // Get the game inputs from Raylib and update ImGui.
                GameInputs gameInputs = raylibController.GetInputs();
                GameState  gameState = raylibController.GetGameState();
                imguiController.Update(gameState.DeltaTime);

                // Update the game.
                game.Update(gameState, gameInputs);


                // ----- Draw ----- //

                raylibController.BeginDrawing();

                game.Draw(raylibController);

                imguiController.Draw();

                raylibController.EndDrawing();
            }


            // ----- DeInitialization ----- //

            imguiController.Dispose();
            raylibController.Dispose();
        }
    }
}
