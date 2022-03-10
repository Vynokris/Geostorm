using System;
using System.Numerics;
using Geostorm.Core;
using static MyMathLib.Colors;
using static MyMathLib.Geometry2D;

namespace Geostorm.Renderer
{
    public class EntityVertices
    {
        public Vector2[] PlayerVertices   = new Vector2[8];
        public Vector2[] BulletVertices   = new Vector2[4];
        public Vector2[] WandererVertices = new Vector2[13];
        public Vector2[] GruntVertices    = new Vector2[4];

        public EntityVertices()
        {
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
            }

            // Load bullet vertices.
            { 
                float preScale = 15.0f;
                BulletVertices[0] = new Vector2(-0.3f,  0.0f) * preScale;
                BulletVertices[1] = new Vector2(-0.1f,  0.2f) * preScale;
                BulletVertices[2] = new Vector2( 0.8f,  0.0f) * preScale;
                BulletVertices[3] = new Vector2(-0.1f, -0.2f) * preScale;
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
            }

            // Load grunt vertices.
            {
                float preScale = 18.0f;
                GruntVertices[0] = new Vector2(-1.0f, 0.0f) * preScale;
                GruntVertices[1] = new Vector2(-0.0f,-1.0f) * preScale;
                GruntVertices[2] = new Vector2( 1.0f, 0.0f) * preScale;
                GruntVertices[3] = new Vector2(-0.0f, 1.0f) * preScale;
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
