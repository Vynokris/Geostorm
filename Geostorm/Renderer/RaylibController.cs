﻿using System;
using System.Numerics;

using Raylib_cs;
using static MyMathLib.Arithmetic;
using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;

using Geostorm.Core;
using Geostorm.GameData;


namespace Geostorm.Renderer
{
    public class RaylibController : IDisposable
    {
        public int ScreenWidth  { get; }
        public int ScreenHeight { get; }


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


        // ---------- Game state ---------- //

        public GameState GetGameState()
        {
            GameState gameState = new();

            // Get the screensize.
            gameState.ScreenSize = Vector2Create(ScreenWidth, ScreenHeight);

            // Get the delta time.
            gameState.DeltaTime = Raylib.GetFrameTime();

            return gameState;
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

        public void DrawEntity<T>(in T entity, in EntityVertices entityVertices) where T : IEntity
        {
            Vector2[] vertices    = entityVertices.GetEntityVertices(entity);
            int       vertexCount = vertices.Length;
            RGBA      rgba        = entityVertices.GetEntityColor   (entity);
            Color     color       = new(RoundInt(rgba.R*255), RoundInt(rgba.G*255), RoundInt(rgba.B*255), RoundInt(rgba.A*255)); 

            // Loop on the entity's vertices and draw lines between them.
            for (int i = 0; i < vertexCount; i++)
            {
                Raylib.DrawLine((int)vertices[i].X,                 (int)vertices[i].Y,
                                (int)vertices[(i+1)%vertexCount].X, (int)vertices[(i+1)%vertexCount].Y,
                                color);
            }
        }

        public void DrawPlayerShield(Vector2 pos, float remainingFrames)
        {
            if (remainingFrames > 90 || (int)(remainingFrames / 10) % 3 == 0)
                Raylib.DrawCircleLines((int)pos.X, (int)pos.Y, 25, Color.WHITE);
        }
    }
}
