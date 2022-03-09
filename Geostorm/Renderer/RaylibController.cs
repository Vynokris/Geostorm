using System;
using System.Numerics;

using Raylib_cs;
using static MyMathLib.Geometry2D;

using Geostorm.Core;


namespace Geostorm.Renderer
{
    public class RaylibController : IDisposable
    {
        public int ScreenWidth  { get; }
        public int ScreenHeight { get; }
        public EntityVertices entityVertices { get; } = new();


        // ---------- Constructor & destructor ---------- //

        public unsafe RaylibController(in int screenW, in int screenH)
        {
            ScreenWidth  = screenW;
            ScreenHeight = screenH;

            Raylib.SetTraceLogCallback(&Logging.LogConsole);
            Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.InitWindow(ScreenWidth, ScreenHeight, "ImGui demo");
            Raylib.SetTargetFPS(60);

            Raylib.InitAudioDevice();
        }

        public void Dispose()
        {
            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }


        // ---------- Miscelaneous ---------- //

        public bool WindowShouldClose() { return Raylib.WindowShouldClose(); }


        // ---------- Keyboard input ---------- //

        public GameInputs HandleInputs()
        {
            GameInputs inputs = new();

            // Get the screensize.
            inputs.screenSize = Vector2Create(ScreenWidth, ScreenHeight);

            // Get the delta time.
            inputs.DeltaTime = Raylib.GetFrameTime();
            
            if (!Raylib.IsGamepadAvailable(0))
            { 
                // Get player movement.
                if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
                    inputs.Movement.Y -= 1;
                if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
                    inputs.Movement.Y += 1;
                if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
                    inputs.Movement.X -= 1;
                if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
                    inputs.Movement.X += 1;
            
                // Get the dashing input.
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT_SHIFT) ||
                    Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                    inputs.Dash = true;

                // Get the shooting input.
                if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
                    inputs.Shoot = true;

                // Get the shooting target.
                inputs.ShootTarget = Raylib.GetMousePosition();
                inputs.ShootDir    = Vector2Zero();
            }
            else
            { 
                // Get player movement.
                inputs.Movement = Vector2Create(Raylib.GetGamepadAxisMovement(0, GamepadAxis.GAMEPAD_AXIS_LEFT_X),
                                                Raylib.GetGamepadAxisMovement(0, GamepadAxis.GAMEPAD_AXIS_LEFT_Y));

                // Get dashing input.
                inputs.Dash = Raylib.IsGamepadButtonPressed(0, GamepadButton.GAMEPAD_BUTTON_LEFT_TRIGGER_2);

                // Get Shooting input.
                inputs.Dash = Raylib.IsGamepadButtonPressed(0, GamepadButton.GAMEPAD_BUTTON_LEFT_TRIGGER_1);

                // Get The shooting direction.
                inputs.ShootTarget = Vector2Create(-1, -1);
                inputs.ShootDir    = Vector2Create(Raylib.GetGamepadAxisMovement(0, GamepadAxis.GAMEPAD_AXIS_RIGHT_X),
                                                   Raylib.GetGamepadAxisMovement(0, GamepadAxis.GAMEPAD_AXIS_RIGHT_X));
                inputs.ShootDir.Normalize();
            }

            return inputs;
        }


        // ---------- Drawing functions ---------- //

        public void BeginDrawing()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);
        }

        public void EndDrawing()
        {
            Raylib.EndDrawing();
        }

        public void DrawEntity<T>(in T entity) where T : IEntity
        {
            Vector2[] vertices;
            Type entityType = entity.GetType();

            // Get the right vertices in function of the entity.
            if      (entityType == new Player().GetType())
                vertices = entityVertices.Player;
            else if (entityType == new Bullet().GetType())
                vertices = entityVertices.Bullet;
            else if (entityType == new Grunt().GetType())
                vertices = entityVertices.Grunt;
            else return;

            // Create the player's transformed vertices.
            Vector2 firstVertex = vertices[0].GetRotatedAsPoint(entity.Rotation, Vector2Zero()) + entity.Pos;
            Vector2 curVertex   = firstVertex;
            Vector2 prevVertex;

            // Loop on the player's vertices, transform them and draw lines between them.
            foreach(Vector2 vertex in vertices)
            {
                prevVertex = curVertex;
                curVertex  = vertex.GetRotatedAsPoint(entity.Rotation, Vector2Zero()) + entity.Pos;

                if (prevVertex != curVertex)
                { 
                    Raylib.DrawLine((int)curVertex.X,  (int)curVertex.Y,
                                    (int)prevVertex.X, (int)prevVertex.Y,
                                    Color.WHITE);
                }
            }
            
            // Draw the last line.
            Raylib.DrawLine((int)curVertex.X,  (int)curVertex.Y,
                            (int)firstVertex.X, (int)firstVertex.Y,
                            Color.WHITE);
        }
    }
}
