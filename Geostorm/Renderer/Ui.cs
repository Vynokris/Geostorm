using System;
using System.Collections.Generic;
using System.Numerics;
using static System.MathF;
using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;
using Raylib_cs;
using Geostorm.GameData;
using Geostorm.Utility;

namespace Geostorm.Renderer
{
    public class Ui : IEventListener
    {
        private RenderTexture2D   HealthTexture;

        // Menu letters blinking.
        private readonly Cooldown BlinkCooldown = new(0.5f);
        private readonly Cooldown BlinkDuration = new(0.1f);

        // Screen shake.
        public           Vector2  ShakeOffset    = new(0, 0);
        private readonly Cooldown ShakeCooldown  = new(0);
        private          int      ShakeIntensity = 0;

        // Copy of delta time.
        private float DeltaTime = 0;

        ~Ui()
        {
            Raylib.UnloadRenderTexture(HealthTexture);
        }

        public void LoadHealthTexture(in Vector2[] playerVertices)
        {
            // Make a copy of the player vertices to be able to move them.
            Vector2[] verticesCopy = (Vector2[])playerVertices.Clone();

            // Move the player vertices to the center of the health rendertexture.
            for (int i = 0; i < verticesCopy.Length; i++)
                verticesCopy[i] = verticesCopy[i].GetRotatedAsPoint(PI/2, Vector2Zero()) + Vector2Create(20, 20);

            // Load the health rendertexture and draw the player on it.
            HealthTexture = Raylib.LoadRenderTexture(40, 40);
            Raylib.BeginTextureMode(HealthTexture);
            Raylib.ClearBackground(new Color(0, 0, 0, 0));
            Raylib.DrawLineStrip(verticesCopy, verticesCopy.Length, new Color(255, 255, 255, 255));
            Raylib.EndTextureMode();
        }

        public void UpdateScreenShake(in float deltaTime, in int intensity = 0, in float duration = 0)
        {
            Random rng = new();

            // Initiate a new screenshake.
            if (!(intensity == 0 && duration == 0))
            {
                if (ShakeCooldown.Counter < duration)
                    ShakeCooldown.ChangeDuration(duration);
                ShakeIntensity = intensity;
                return;
            }

            // Shake the screen according to the intensity.
            if (!ShakeCooldown.Update(deltaTime))
            { 
                ShakeOffset = new Vector2(rng.Next(-ShakeIntensity, ShakeIntensity), 
                                            rng.Next(-ShakeIntensity, ShakeIntensity));
            }
            else if (ShakeOffset != Vector2Zero())
            {
                ShakeOffset = Vector2Zero();
            }
        }

        public void HandleEvents(in List<GameEvent> gameEvents)
        {
            foreach (GameEvent gameEvent in gameEvents)
            {
                Type eventType = gameEvent.GetType();

                if (eventType == typeof(PlayerDashEvent))
                    UpdateScreenShake(DeltaTime, 2, 0.1f);

                if (eventType == typeof(EnemyKilledEvent))
                    UpdateScreenShake(DeltaTime, 3, 0.4f);

                if (eventType == typeof(PlayerDamagedEvent))
                    UpdateScreenShake(DeltaTime, 5, 0.6f);

                if (eventType == typeof(PlayerKilledEvent))
                    UpdateScreenShake(DeltaTime, 7, 0.8f);
            }
        }

        public void Draw(in Game game, in GameState gameState, ref List<GameEvent> gameEvents)
        {
            Vector2 menuOffset        = ((Raylib.GetMousePosition() - gameState.ScreenSize / 2) / gameState.ScreenSize) * -30;
            Vector2 screenBoundOffset = new(40, 30);
            int     titleSize         = 200;
            int     textSize          = 45;

            // Save the delta time.
            DeltaTime = gameState.DeltaTime;

            // Update the screen shake.
            UpdateScreenShake(DeltaTime);

            switch (game.currentScene)
            {
                case Scenes.MainMenu:
                { 
                    DrawMainMenuUi(gameState, ref gameEvents, menuOffset, titleSize, textSize);
                    break;
                }

                case Scenes.InGame:
                {
                    DrawInGameUi(game, gameState, screenBoundOffset, textSize);
                    break;
                }

                case Scenes.GameOver:
                { 
                    DrawGameOverUi(gameState, ref gameEvents, menuOffset, titleSize, textSize);
                    break;
                }
            }
        }

        private void DrawMainMenuUi(in GameState gameState, ref List<GameEvent> gameEvents, in Vector2 menuOffset, in int titleSize, in int textSize)
        {
            Random rng = new();

            // Draw title.
            Vector2 titlePos = new(gameState.ScreenSize.X / 2 - Raylib.MeasureText("GeoStorm", titleSize) / 2 + menuOffset.X, gameState.ScreenSize.Y / 2.5f + menuOffset.Y);
            Raylib.DrawText("Geo", (int)titlePos.X, (int)titlePos.Y, titleSize, new Color(255, 0, 0, 255));
            titlePos.X += Raylib.MeasureText("GeoS", titleSize) - Raylib.MeasureText("S", titleSize);
            Raylib.DrawText("Storm", (int)titlePos.X, (int)titlePos.Y, titleSize, Color.WHITE);

            // Get the position of the "o" in "storm".
            int widthO = Raylib.MeasureText("o", titleSize);
            titlePos.X += Raylib.MeasureText("Sto", titleSize) - widthO;
                        
            // Make the "o" in "storm" blink.
            Color blinkColor = new(255, 255, 255, 255);
            if (BlinkCooldown.Update(gameState.DeltaTime)) 
            { 
                blinkColor = new Color(50, 50, 50, 255);

                if (BlinkDuration.Update(gameState.DeltaTime)) 
                { 
                    BlinkDuration.Reset();
                    BlinkCooldown.ChangeDuration(rng.Next(30, 120) / 60f);
                    gameEvents.Add(new ParticleSpawnedEvent(20, titlePos + new Vector2(widthO/2, widthO), new RGBA(1, 1, 1, 1)));
                    UpdateScreenShake(gameState.DeltaTime, 2, 0.1f);
                }
            }
            Raylib.DrawText("o", (int)titlePos.X, (int)titlePos.Y, titleSize, blinkColor);

            // Start prompt.
            Vector2 startPos = new((int)gameState.ScreenSize.X / 2 - Raylib.MeasureText("PRESS SPACE / LEFT SHIFT", textSize) / 2 + menuOffset.X, gameState.ScreenSize.Y  - 120 + menuOffset.Y);
            Raylib.DrawText("PRESS SPACE / LEFT SHIFT", (int)startPos.X, (int)startPos.Y, textSize, Color.WHITE);

        }

        private void DrawInGameUi(in Game game, in GameState gameState, in Vector2 screenBoundOffset, in int textSize)
        {
            // Draw score.
            Raylib.DrawText($"SCORE: {gameState.Score}", (int)screenBoundOffset.X, (int)screenBoundOffset.Y, textSize, new Color(255, 255, 255, 255));

            // Draw multiplier.
            int multiplierPosX = (int)gameState.ScreenSize.X - (int)screenBoundOffset.X - Raylib.MeasureText($"x{gameState.Multiplier}", textSize);
            Raylib.DrawText($"x{gameState.Multiplier}", multiplierPosX, (int)screenBoundOffset.Y, textSize, new Color(255, 255, 255, 255));

            // Draw multiplier reset bar.
            Raylib.DrawRectangleLines((int)(gameState.ScreenSize.X - screenBoundOffset.X - 100), 
                                      (int)screenBoundOffset.Y + textSize + 20, 120, 20, Color.WHITE);
            Raylib.DrawRectangle((int)(gameState.ScreenSize.X - screenBoundOffset.X - 100), 
                                 (int)screenBoundOffset.Y + textSize + 20, 
                                 (int)(120 * game.MultiplierResetCooldown.CompletionRatio()), 20, Color.WHITE);

            // Draw game duration.
            int gameDurationI = (int)gameState.GameDuration;
            Raylib.DrawText($"{gameDurationI}s", (int)((gameState.ScreenSize.X - Raylib.MeasureText($"{gameDurationI}s", textSize)) / 2f), (int)screenBoundOffset.Y, textSize, Color.WHITE);

            // Draw player hp.
            int       healthGap   = 10;
            int       healthLeftX = (int)((gameState.ScreenSize.X - (HealthTexture.texture.width + healthGap) * game.player.Health) / 2f);

            for (int i = 0; i < game.player.Health; i++)
            {
                int healthCurX = (int)(healthLeftX + (HealthTexture.texture.width + healthGap) * i);
                Raylib.DrawTexturePro(HealthTexture.texture,
                                      new Rectangle(0, 0, 40, 40), 
                                      new Rectangle(healthCurX, screenBoundOffset.Y + textSize + 10, 40, 40),
                                      new Vector2(0, 0), 0, Color.WHITE);
            }
        }

        private void DrawGameOverUi(in GameState gameState, ref List<GameEvent> gameEvents, in Vector2 menuOffset, in int titleSize, in int textSize)
        {
            Random rng = new();

            // Draw title.
            Vector2 titlePos = new(gameState.ScreenSize.X / 2 - Raylib.MeasureText("Game Over", titleSize) / 2 + menuOffset.X, gameState.ScreenSize.Y / 3f + menuOffset.Y);
            Raylib.DrawText("Game Over", (int)titlePos.X, (int)titlePos.Y, titleSize, new Color(255, 0, 0, 255));

            // Get the position of the "a" in "game".
            int widthA = Raylib.MeasureText("o", titleSize);
            titlePos.X += Raylib.MeasureText("Ga", titleSize) - widthA;
                        
            // Make the "a" in "game" blink.
            Color blinkColor = new(255, 0, 0, 255);
            if (BlinkCooldown.Update(gameState.DeltaTime)) 
            { 
                blinkColor = new Color(50, 0, 0, 255);

                if (BlinkDuration.Update(gameState.DeltaTime)) 
                { 
                    BlinkDuration.Reset();
                    BlinkCooldown.ChangeDuration(rng.Next(30, 120) / 60f);
                    gameEvents.Add(new ParticleSpawnedEvent(20, titlePos + new Vector2(widthA/2, widthA), new RGBA(1, 0, 0, 1)));
                    UpdateScreenShake(gameState.DeltaTime, 2, 0.1f);
                }
            }
            Raylib.DrawText("a", (int)titlePos.X, (int)titlePos.Y, titleSize, blinkColor);

            // Show score.
            int scoreSize = (int)((titleSize + textSize * 3) / 4f);
            Vector2 scorePos = new((int)gameState.ScreenSize.X / 2 - Raylib.MeasureText($"SCORE: {gameState.Score}", scoreSize) / 2 + menuOffset.X, gameState.ScreenSize.Y / 1.75f + menuOffset.Y);
            Raylib.DrawText($"SCORE: {gameState.Score}", (int)scorePos.X, (int)scorePos.Y, scoreSize, Color.WHITE);

            // Show duration.
            int gameDurationI = (int)gameState.GameDuration;
            Vector2 duarionPos = new((int)gameState.ScreenSize.X / 2 - Raylib.MeasureText($"DURATION: {gameDurationI}", scoreSize) / 2 + menuOffset.X, gameState.ScreenSize.Y / 1.75f + menuOffset.Y);
            Raylib.DrawText($"DURATION: {gameDurationI}", (int)duarionPos.X, (int)duarionPos.Y + scoreSize + 10, scoreSize, Color.WHITE);

            // Restart prompt.
            Vector2 restartPos = new((int)gameState.ScreenSize.X / 2 - Raylib.MeasureText("PRESS SPACE / LEFT SHIFT", textSize) / 2 + menuOffset.X, gameState.ScreenSize.Y - 120 + menuOffset.Y);
            Raylib.DrawText("PRESS SPACE / LEFT SHIFT", (int)restartPos.X, (int)restartPos.Y, textSize, Color.WHITE);
        }
    }
}
