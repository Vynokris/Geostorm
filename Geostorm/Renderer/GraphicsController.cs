using System;
using System.Numerics;
using System.Collections.Generic;
using static System.MathF;

using Raylib_cs;
using static MyMathLib.Arithmetic;
using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;

using Geostorm.Core;
using Geostorm.GameData;

// NOTE: if the bloom shader doesn't work, set Project > Properties > Debug > Working Directory to $(ProjectDir).

namespace Geostorm.Renderer
{
    public class GraphicsController : IDisposable
    {
        public int ScreenWidth  { get; private set; } = 1920;
        public int ScreenHeight { get; private set; } = 1080;

        private Shader GaussianBlurShader;
        private Shader NonBlackMaskShader;
        private Shader ChromaticAberrationShader;
        private readonly int BlurDirLocation;
        public  readonly int BloomIntensity = 5;

        private RenderTexture2D   RenderTexture;
        private RenderTexture2D[] BlurTextures;


        // ---------- Constructor & destructor ---------- //

        public unsafe GraphicsController()
        {
            // Create a new raylib window.
            Raylib.SetTraceLogCallback(&Logging.LogConsole);
            Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_WINDOW_UNDECORATED);
            Raylib.InitWindow(ScreenWidth, ScreenHeight, "Geostorm");
            Raylib.SetTargetFPS(60);

            // Get the monitor width and set the window size to it.
            ScreenWidth  = Raylib.GetMonitorWidth(0);
            ScreenHeight = Raylib.GetMonitorHeight(0);
            Raylib.SetWindowSize(ScreenWidth, ScreenHeight+1);
            Raylib.SetWindowPosition(0, -1);

            // Adjust the bloom intensity accrding to screen size.
            BloomIntensity = (int)(BloomIntensity * ScreenWidth / 2560f);

            // Load shaders.
            GaussianBlurShader        = Raylib.LoadShader(null, "Shaders/GaussianBlur.fs");
            NonBlackMaskShader        = Raylib.LoadShader(null, "Shaders/NonBlackPixels.fs");
            ChromaticAberrationShader = Raylib.LoadShader(null, "Shaders/ChromaticAberration.fs");
            RenderTexture             = Raylib.LoadRenderTexture(ScreenWidth, ScreenHeight);
            BlurTextures              = new RenderTexture2D[] { Raylib.LoadRenderTexture(ScreenWidth, ScreenHeight), 
                                                                Raylib.LoadRenderTexture(ScreenWidth, ScreenHeight) };
            BlurDirLocation = Raylib.GetShaderLocation(GaussianBlurShader, "isVertical");

            // Set gaussian blur shader screen size.
            { 
                int screenWidthLocation = Raylib.GetShaderLocation(GaussianBlurShader, "screenWidth");
                Raylib.SetShaderValue(GaussianBlurShader, screenWidthLocation, ScreenWidth, ShaderUniformDataType.SHADER_UNIFORM_INT);
                int screenHeightLocation = Raylib.GetShaderLocation(GaussianBlurShader, "screenHeight");
                Raylib.SetShaderValue(GaussianBlurShader, screenHeightLocation, ScreenHeight, ShaderUniformDataType.SHADER_UNIFORM_INT);
            }

            // Set chromatic aberration shader screen size.
            { 
                int screenWidthLocation = Raylib.GetShaderLocation(ChromaticAberrationShader, "screenWidth");
                Raylib.SetShaderValue(ChromaticAberrationShader, screenWidthLocation, ScreenWidth, ShaderUniformDataType.SHADER_UNIFORM_INT);
                int screenHeightLocation = Raylib.GetShaderLocation(ChromaticAberrationShader, "screenHeight");
                Raylib.SetShaderValue(ChromaticAberrationShader, screenHeightLocation, ScreenHeight, ShaderUniformDataType.SHADER_UNIFORM_INT);
            }

            Raylib.InitAudioDevice();
        }

        public void Dispose()
        {
            Raylib.UnloadShader(GaussianBlurShader);
            Raylib.UnloadShader(NonBlackMaskShader);
            Raylib.UnloadShader(ChromaticAberrationShader);
            Raylib.UnloadRenderTexture(RenderTexture);
            Raylib.UnloadRenderTexture(BlurTextures[0]);
            Raylib.UnloadRenderTexture(BlurTextures[1]);
            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }


        // ---------- Miscelaneous ---------- //

        public bool WindowShouldClose() { return Raylib.WindowShouldClose(); }

        public static Color RGBAtoRayCol(RGBA rgba) 
        {
            return new Color((int)(rgba.R*255.0), 
                             (int)(rgba.G*255.0), 
                             (int)(rgba.B*255.0), 
                             (int)(rgba.A*255.0)); 
        }


        // ---------- Game state ---------- //

        public void UpdateGameState(ref GameState gameState)
        {
            // Get the delta time.
            gameState.DeltaTime = Raylib.GetFrameTime();

            // Update the game duration.
            gameState.GameDuration += gameState.DeltaTime;
        }


        // ---------- Keyboard input ---------- //

        public GameInputs GetInputs()
        {
            GameInputs inputs = new();
            
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
                inputs.Dash  = Raylib.IsGamepadButtonPressed(0, GamepadButton.GAMEPAD_BUTTON_LEFT_TRIGGER_1) ||
                               Raylib.IsGamepadButtonPressed(0, GamepadButton.GAMEPAD_BUTTON_LEFT_TRIGGER_2);

                // Get Shooting input.
                inputs.Shoot = Raylib.IsGamepadButtonDown(0, GamepadButton.GAMEPAD_BUTTON_RIGHT_TRIGGER_1) ||
                               Raylib.IsGamepadButtonDown(0, GamepadButton.GAMEPAD_BUTTON_RIGHT_TRIGGER_2);

                // Get The shooting direction.
                inputs.ShootTarget = Vector2Create(-1, -1);
                inputs.ShootDir    = Vector2Create(Raylib.GetGamepadAxisMovement(0, GamepadAxis.GAMEPAD_AXIS_RIGHT_X),
                                                   Raylib.GetGamepadAxisMovement(0, GamepadAxis.GAMEPAD_AXIS_RIGHT_Y));
                inputs.ShootDir.Normalize();
            }

            return inputs;
        }


        // ---------- Drawing functions ---------- //

        public void BeginDrawing()
        {
            Raylib.HideCursor();
            Raylib.BeginTextureMode(RenderTexture);
            Raylib.ClearBackground(Color.BLACK);
        }

        private void ApplyBloom()
        {
            for (int i = 0; i < BloomIntensity; i++)
            {
                // Draw the render texture on the first blurring texture.
                Raylib.BeginTextureMode(BlurTextures[0]);
                {
                    if (i == 0)
                        Raylib.ClearBackground(Color.BLACK);
                    Raylib.BeginShaderMode(NonBlackMaskShader);
                    Raylib.DrawTextureRec(RenderTexture.texture,
                                          new Rectangle(0, 0, ScreenWidth, -ScreenHeight),
                                          new Vector2  (0, 0),
                                          Color.WHITE);
                    Raylib.EndShaderMode();
                }
                Raylib.EndTextureMode();

                // Blur the 2 blurring render textures (ping pong texturing).
                for (int j = 0; j < 2; j++)
                { 
                    Raylib.BeginTextureMode(BlurTextures[(j+1)%2]);
                    {
                        Raylib.ClearBackground(Color.BLACK);

                        Raylib.BeginShaderMode(GaussianBlurShader);
                        Raylib.SetShaderValue(GaussianBlurShader, BlurDirLocation, j, ShaderUniformDataType.SHADER_UNIFORM_INT);
                        Raylib.DrawTextureRec(BlurTextures[j].texture,
                                              new Rectangle(0, 0, ScreenWidth, -ScreenHeight),
                                              new Vector2  (0, 0),
                                              Color.WHITE);
                        Raylib.EndShaderMode();
                    }
                    Raylib.EndTextureMode();
                }
            }
        }

        public void EndDrawing()
        {
            Raylib.EndTextureMode();

            ApplyBloom();

            // Draw the render texture and the blurred texture on the screen.
            Raylib.BeginDrawing();
            {
                Raylib.ClearBackground(Color.BLACK);
                
                Raylib.BeginShaderMode(ChromaticAberrationShader);
                { 
                    Raylib.DrawTextureRec(BlurTextures[0].texture,
                                            new Rectangle(0, 0, ScreenWidth, -ScreenHeight),
                                            new Vector2  (0, 0),
                                            Color.WHITE);
                
                    Raylib.BeginShaderMode(NonBlackMaskShader);
                    { 
                        Raylib.DrawTextureRec(RenderTexture.texture,
                                                new Rectangle(0, 0, ScreenWidth, -ScreenHeight),
                                                new Vector2  (0, 0),
                                                Color.WHITE);
                    }
                    Raylib.EndShaderMode();
                }
                Raylib.EndShaderMode();
            }
            Raylib.EndDrawing();
        }

        public void DrawLines(in Vector2[] vertices, RGBA color, bool detached = false)
        {
            // Raylib.DrawLineStrip(vertices, vertices.Length, RGBAtoRayCol(color));
            for (int i = 0; i < vertices.Length-1; i += (detached ? 2 : 1))
                Raylib.DrawLineEx(vertices[i], vertices[i+1], 1, RGBAtoRayCol(color));
        }

        public void DrawEntity<T>(in T entity, in EntityVertices entityVertices) where T : IEntity
        {
            Vector2[] vertices    = entityVertices.GetEntityVertices(entity);
            int       vertexCount = vertices.Length;
            RGBA      rgbaColor   = entityVertices.GetEntityColor(entity);
            Color     color       = RGBAtoRayCol(rgbaColor);

            // Draw the player's shield.
            if (entity is Player player)
            {
                if (!player.Invincibility.HasEnded())
                {
                    // Make the shield blink 3 times in the last 100 frames of invincivility.
                    Color shieldColor = Color.WHITE;
                    if (player.Invincibility.Counter * 60 <= 100 && (int)(player.Invincibility.Counter * 6 - 1) % 3 == 1)
                        shieldColor.a = 255 / 4;

                    // Draw the shield.
                    Raylib.DrawCircleLines((int)entity.Pos.X, (int)entity.Pos.Y, 25, shieldColor);
                }
            }

            // Make the geoms blink 3 times in 100 frames before they despawn.
            if (entity is Geom geom)
            {
                if (geom.DespawnTimer.Counter * 60 <= 40 && (int)(geom.DespawnTimer.Counter * 6) % 2 == 1)
                        color.a = 255 / 4;
            }

            // Draw the spawn animation for enemies.
            if (entity is Enemy enemy && !enemy.SpawnDelay.HasEnded())
            {
                if (enemy.PreSpawnDelay == 0)
                    DrawLines(entityVertices.GetEntitySpawnAnimation(enemy, enemy.SpawnDelay), rgbaColor, true);
                return;
            }

            // Draw circles for the stars.
            if (entity is Star star)
            { 
                Raylib.DrawCircle((int)star.Pos.X, (int)star.Pos.Y, star.Radius, RGBAtoRayCol(star.Color));
                return;
            }

            // Loop on the entity's vertices and draw lines between them.
            Raylib.DrawLineStrip(vertices, vertexCount, color);
        }

        public void DrawCursor(in EntityVertices entityVertices)
        {
            if (!Raylib.IsGamepadAvailable(0))
            {
                for (int i = 0; i < 8; i += 2)
                {
                    Raylib.DrawLineEx(entityVertices.CursorVertices[i]   + Raylib.GetMousePosition(), 
                                      entityVertices.CursorVertices[i+1] + Raylib.GetMousePosition(), 
                                      1, Color.WHITE);
                }
            }
        }
    }
}
