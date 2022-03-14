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
        public Vector2[] GeomVertices     = new Vector2[5];
        public Vector2[] WandererVertices = new Vector2[14];
        public Vector2[] GruntVertices    = new Vector2[5];
        public Vector2[] WeaverVertices   = new Vector2[10];
        public Vector2[] ParticleVertices = new Vector2[2];

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
                float preScale = 15;
                BulletVertices[0] = new Vector2(-0.3f,  0.0f) * preScale;
                BulletVertices[1] = new Vector2(-0.1f,  0.2f) * preScale;
                BulletVertices[2] = new Vector2( 0.8f,  0.0f) * preScale;
                BulletVertices[3] = new Vector2(-0.1f, -0.2f) * preScale;
                BulletVertices[4] = new Vector2(-0.3f,  0.0f) * preScale;
            }

            // Load geom vertices.
            {
                float preScale = 10;
                GeomVertices[0] = new Vector2(-1.0f, 0.0f) * preScale;
                GeomVertices[1] = new Vector2( 0.0f,-0.5f) * preScale;
                GeomVertices[2] = new Vector2( 1.0f, 0.0f) * preScale;
                GeomVertices[3] = new Vector2( 0.0f, 0.5f) * preScale;
                GeomVertices[4] = new Vector2(-1.0f, 0.0f) * preScale;
            }

            // Load wanderer vertices.
            {
                float preScale = 18;
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
                float preScale = 18;
                GruntVertices[0] = new Vector2(-1.0f, 0.0f) * preScale;
                GruntVertices[1] = new Vector2( 0.0f,-1.0f) * preScale;
                GruntVertices[2] = new Vector2( 1.0f, 0.0f) * preScale;
                GruntVertices[3] = new Vector2( 0.0f, 1.0f) * preScale;
                GruntVertices[4] = new Vector2(-1.0f, 0.0f) * preScale;
            }

            // Load weaver vertices.
            {
                float preScale = 18;
                WeaverVertices[0] = new Vector2( 1, -1) * preScale;
                WeaverVertices[1] = new Vector2(-1, -1) * preScale;
                WeaverVertices[2] = new Vector2(-1,  1) * preScale;
                WeaverVertices[3] = new Vector2( 1,  1) * preScale;
                WeaverVertices[4] = new Vector2( 1,  0) * preScale;
                WeaverVertices[5] = new Vector2( 0, -1) * preScale;
                WeaverVertices[6] = new Vector2(-1,  0) * preScale;
                WeaverVertices[7] = new Vector2( 0,  1) * preScale;
                WeaverVertices[8] = new Vector2( 1,  0) * preScale;
                WeaverVertices[9] = new Vector2( 1, -1) * preScale;
            }

            // Load particle vertices.
            {
                float preScale = 10;
                ParticleVertices[0] = new Vector2( 1, 0) * preScale;
                ParticleVertices[1] = new Vector2(-1, 0) * preScale;
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
            else if (entityType == typeof(Geom)) {
                vertices = (Vector2[])GeomVertices.Clone();
            }
            else if (entityType == typeof(Grunt)) {
                vertices = (Vector2[])GruntVertices.Clone();
            }
            else if (entityType == typeof(Wanderer)) {
                vertices = (Vector2[])WandererVertices.Clone();
            }
            else if (entityType == typeof(Weaver)) {
                vertices = (Vector2[])WeaverVertices.Clone();
            }
            else if (entityType == typeof(Particle)) {
                vertices = (Vector2[])ParticleVertices.Clone();
            }

            // Transform the entity's vertices to screen positions.
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = vertices[i].GetScaledAsPoint(entity.Scale, Vector2Zero()).GetRotatedAsPoint(entity.Rotation, Vector2Zero()) + entity.Pos;

            return vertices;
        }

        public Vector2[] GetEntitySpawnAnimation<T>(T entity, in Cooldown spawnDelay) where T : IEntity
        {
            // Get the entity's vertices and create the output vertex array.
            Vector2[] vertices = GetEntityVertices(entity);
            List<Vector2> output = new();

            // Lerp all vertices at once.
            for (int i = 0; i < vertices.Length-1; i++)
            {
                output.Add(vertices[i]);
                output.Add(Point2Lerp(1-spawnDelay.CompletionRatio(), vertices[i], vertices[i+1]));
            }

            return output.ToArray();
        }

        /*
        public RGBA GetEntityColor<T>(T entity) where T : IEntity
        {
            Type entityType = entity.GetType();
            RGBA color      = PlayerColor;

            // Get the right vertices and color in function of the entity.
            if (entityType == typeof(Bullet)) {
                color = BulletColor;
            }
            else if (entityType == typeof(Geom)) {
                color = GeomColor;
            }
            else if (entityType == typeof(Wanderer)) {
                color = WandererColor;
            }
            else if (entityType == typeof(Grunt)) {
                color = GruntColor;
            }
            else if (entityType == typeof(Weaver)) {
                color = WeaverColor;
            }
            else if (entity is Particle particle) { 
                color = particle.Color;
            }

            return color;
        }
        */
    }
}
