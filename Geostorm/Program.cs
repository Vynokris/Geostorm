using Geostorm.Core;
using Geostorm.Renderer;
using Geostorm.GameData;

using static MyMathLib.Geometry2D;

namespace Geostorm
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            // ----- Initialization ----- //

            GraphicsController graphicsController = new();
            ImguiController    imguiController    = new();

            int screenW = graphicsController.ScreenWidth;
            int screenH = graphicsController.ScreenHeight;

            Game      game      = new(screenW, screenH);
            GameState gameState = new(screenW,  screenH);

            imguiController.Load(screenW, screenH);

            // ----- Main game loop ----- //

            while (!graphicsController.WindowShouldClose())
            {
                // ----- Update ----- //

                // Get the game inputs and game state.
                GameInputs gameInputs = graphicsController.GetInputs();
                graphicsController.UpdateGameState(ref gameState);

                // Update imgui and the game.
                imguiController.Update(gameState.DeltaTime);
                game.Update(ref gameState, gameInputs);


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
