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

            GraphicsController graphicsController = new();
            ImguiController    imguiController    = new();

            int screenWidth  = graphicsController.ScreenWidth;
            int screenHeight = graphicsController.ScreenHeight;

            Game game = new(screenWidth, screenHeight);
            imguiController.Load(screenWidth, screenHeight);

            // ----- Main game loop ----- //

            while (!graphicsController.WindowShouldClose())
            {
                // ----- Update ----- //

                // Get the game inputs and game state.
                GameInputs gameInputs = graphicsController.GetInputs();
                GameState  gameState  = graphicsController.GetGameState();

                // Update imgui and the game.
                imguiController.Update(gameState.DeltaTime);
                game.Update(gameState, gameInputs);


                // ----- Draw ----- //

                graphicsController.BeginDrawing();
                { 
                    game.Draw(graphicsController);
                    imguiController.Draw();
                }
                graphicsController.EndDrawing();
            }


            // ----- DeInitialization ----- //

            imguiController.Dispose();
            graphicsController.Dispose();
        }
    }
}
