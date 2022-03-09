using Geostorm.Core;
using Geostorm.Renderer;

namespace Geostorm
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            // ----- Initialization ----- //

            const int screenWidth = 1920;
            const int screenHeight = 1080;

            GameData.Game    game             = new();
            RaylibController raylibController = new(screenWidth, screenHeight);
            ImguiController  imguiController  = new();
            imguiController.Load(screenWidth, screenHeight);

            game.bullets.Add(new Bullet());
            game.bullets[0].pos = new System.Numerics.Vector2(screenWidth/2 - 100, screenHeight/2);
            game.bullets[0].rotation = 0;

            game.enemies.Add(new Grunt());
            game.enemies[0].pos = new System.Numerics.Vector2(screenWidth/2 + 100, screenHeight/2);
            game.enemies[0].rotation = 0;

            // ----- Main game loop ----- //

            while (!raylibController.WindowShouldClose())
            {
                // ----- Update ----- //

                // Get the game inputs from Raylib and update ImGui.
                GameInputs inputs = raylibController.HandleInputs();
                imguiController.Update(inputs.DeltaTime);

                // Update the game.
                game.Update(inputs);


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
