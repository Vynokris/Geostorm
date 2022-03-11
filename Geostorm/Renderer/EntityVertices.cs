using System;
using System.Numerics;
using System.Collections.Generic;

using static MyMathLib.Colors;
using static MyMathLib.Geometry2D;

using Geostorm.Core;
using Geostorm.Utility;

namespace Geostorm.Renderer
{
    public class EntityVertices
    {
        public Vector2[] CursorVertices   = new Vector2[8];
        public Vector2[] PlayerVertices   = new Vector2[9];
        public Vector2[] BulletVertices   = new Vector2[5];
        public Vector2[] WandererVertices = new Vector2[14];
        public Vector2[] GruntVertices    = new Vector2[5];

        public EntityVertices()
        {
            // Load cursor vertices (used to shoot with mouse).
            {
                float preScale = 15;
                CursorVertices[0] = new Vector2( 0.0f,  0.3f) * preScale;
                CursorVertices[1] = new Vector2( 0.0f,  1.0f) * preScale;
                CursorVertices[2] = new Vector2( 0.0f, -0.3f) * preScale;
                CursorVertices[3] = new Vector2( 0.0f, -1.0f) * preScale;
                CursorVertices[4] = new Vector2( 0.3f,  0.0f) * preScale;
                CursorVertices[5] = new Vector2( 1.0f,  0.0f) * preScale;
                CursorVertices[6] = new Vector2(-0.3f,  0.0f) * preScale;
                CursorVertices[7] = new Vector2(-1.0f,  0.0f) * preScale;
            }

            // Load player vertices.
            { 
                float preScale = 20;
                PlayerVertices[0] = new Vector2(-0.3f,  0.0f)  * preScale;
                PlayerVertices[1] = new Vector2( 0.0f, -0.55f) * preScale;
                PlayerVertices[2] = new Vector2( 0.8f, -0.3f)  * preScale;
                PlayerVertices[3] = new Vector2(-0.2f, -0.8f)  * preScale;
                PlayerVertices[4] = new Vector2(-0.8f,  0.0f)  * preScale;
                PlayerVertices[5] = new Vector2(-0.2f,  0.8f)  * preScale;
                PlayerVertices[6] = new Vector2( 0.8f,  0.3f)  * preScale;
                PlayerVertices[7] = new Vector2( 0.0f,  0.55f) * preScale;
                PlayerVertices[8] = new Vector2(-0.3f,  0.0f)  * preScale;
            }

            // Load bullet vertices.
            { 
                float preScale = 15.0f;
                BulletVertices[0] = new Vector2(-0.3f,  0.0f) * preScale;
                BulletVertices[1] = new Vector2(-0.1f,  0.2f) * preScale;
                BulletVertices[2] = new Vector2( 0.8f,  0.0f) * preScale;
                BulletVertices[3] = new Vector2(-0.1f, -0.2f) * preScale;
                BulletVertices[4] = new Vector2(-0.3f,  0.0f) * preScale;
            }

            // Load wanderer vertices.
            {
                float preScale = 15.0f;
                WandererVertices[0]  = new Vector2( 0,  0) * preScale;
                WandererVertices[1]  = new Vector2( 0, -1) * preScale;
                WandererVertices[2]  = new Vector2(-1, -1) * preScale;
                WandererVertices[3]  = new Vector2( 0,  0) * preScale;
                WandererVertices[4]  = new Vector2(-1,  0) * preScale;
                WandererVertices[5]  = new Vector2(-1,  1) * preScale;
                WandererVertices[6]  = new Vector2( 0,  0) * preScale;
                WandererVertices[7]  = new Vector2( 0,  1) * preScale;
                WandererVertices[8]  = new Vector2( 1,  1) * preScale;
                WandererVertices[9]  = new Vector2( 0,  0) * preScale;
                WandererVertices[10] = new Vector2( 1,  0) * preScale;
                WandererVertices[11] = new Vector2( 1, -1) * preScale;
                WandererVertices[12] = new Vector2( 0,  0) * preScale;
                WandererVertices[13]  = new Vector2( 0,  0) * preScale;
            }

            // Load grunt vertices.
            {
                float preScale = 18.0f;
                GruntVertices[0] = new Vector2(-1.0f, 0.0f) * preScale;
                GruntVertices[1] = new Vector2(-0.0f,-1.0f) * preScale;
                GruntVertices[2] = new Vector2( 1.0f, 0.0f) * preScale;
                GruntVertices[3] = new Vector2(-0.0f, 1.0f) * preScale;
                GruntVertices[4] = new Vector2(-1.0f, 0.0f) * preScale;
            }
        }

        public Vector2[] GetEntityVertices<T>(T entity) where T : IEntity
        {
            Type entityType = entity.GetType();
            Vector2[] vertices = Array.Empty<Vector2>();

            // Get the right vertices and color in function of the entity.
            if      (entityType == typeof(Player)) {
                vertices = (Vector2[])PlayerVertices.Clone();
            }
            else if (entityType == typeof(Bullet)) {
                vertices = (Vector2[])BulletVertices.Clone();
            }
            else if (entityType == typeof(Grunt)) {
                vertices = (Vector2[])GruntVertices.Clone();
            }
            else if (entityType == typeof(Wanderer)) {
                vertices = (Vector2[])WandererVertices.Clone();
            }

            // Transform the entity's vertices to screen positions.
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = vertices[i].GetRotatedAsPoint(entity.Rotation, Vector2Zero()) + entity.Pos;

            return vertices;
        }

        public Vector2[] GetEntitySpawnAnimation<T>(T entity, in Cooldown spawnDelay) where T : IEntity
        {
            // Get the entity's vertices and create the output vertex array.
            Vector2[] vertices = GetEntityVertices(entity);
            List<Vector2> output = new();

            // Compute a bunch of variables to determine the state of the animation.
            int   lineCount     = vertices.Length - 1;
            float linesPerFrame = (float)lineCount / spawnDelay.Duration;
            float framesPerLine = 1.0f / linesPerFrame;
            int   finishedLines = (int)((spawnDelay.Duration - spawnDelay.Counter) * linesPerFrame);
            float lerpFactor = ((spawnDelay.Duration-spawnDelay.Counter) - finishedLines * framesPerLine) / framesPerLine;

            // Add all the lines that have already finished their animation.
            for (int i = 0; i < finishedLines+1; i++)
                output.Add(vertices[i]);

            // Draw the line that is currently animated.
            if (finishedLines < lineCount)
            { 
                Vector2 curVertex = Point2Lerp(lerpFactor, vertices[finishedLines], vertices[finishedLines+1]);
                output.Add(curVertex);
                Console.Write($"{lerpFactor}, {curVertex}\n");
            }

            return output.ToArray();
        }

        public RGBA GetEntityColor<T>(T entity) where T : IEntity
        {
            Type entityType = entity.GetType();
            RGBA color = new();

            // Get the right vertices and color in function of the entity.
            if      (entityType == typeof(Player)) {
                color = new RGBA(1, 1, 1, 1);
            }
            else if (entityType == typeof(Bullet)) {
                color = new RGBA(1, 1, 0, 1);
            }
            else if (entityType == typeof(Grunt)) {
                color = new RGBA(0, 1, 1, 1);
            }
            else if (entityType == typeof(Wanderer)) {
                color = new RGBA(1, 0, 1, 1);
            }

            return color;
        }
    }
}
