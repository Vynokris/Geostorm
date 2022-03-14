using Geostorm.Renderer;
using Geostorm.GameData;
using Geostorm.Utility;

namespace Geostorm
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            // ----- Initialization ----- //

            GraphicsController graphicsController = new();
            ImguiController    imguiController    = new();
            CheatMenu          cheatMenu          = new();

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

                // Update imgui.
                imguiController.Update(gameState.DeltaTime);

                // Update the cheat menu.
                cheatMenu.UpdateAndDraw(ref game, gameState, gameInputs);
                if (gameInputs.CheatMenu)
                    graphicsController.mouseCursorHidden = !cheatMenu.Shown;

                // Update the game if the cheat menu isn't open.
                if (!cheatMenu.Shown)
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
