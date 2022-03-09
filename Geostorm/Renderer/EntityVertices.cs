using System.Numerics;

namespace Geostorm.Renderer
{
    public class EntityVertices
    {
        public Vector2[] Player = new Vector2[8];
        public Vector2[] Bullet = new Vector2[4];
        public Vector2[] Grunt  = new Vector2[4];

        public EntityVertices()
        {
            // Load player vertices.
            { 
                float preScale = 20;
                Player[0] = new Vector2(-0.5f,  0.0f)  * preScale;
                Player[1] = new Vector2(-0.2f, -0.55f) * preScale;
                Player[2] = new Vector2( 0.6f, -0.3f)  * preScale;
                Player[3] = new Vector2(-0.4f, -0.8f)  * preScale;
                Player[4] = new Vector2(-1.0f,  0.0f)  * preScale;
                Player[5] = new Vector2(-0.4f,  0.8f)  * preScale;
                Player[6] = new Vector2( 0.6f,  0.3f)  * preScale;
                Player[7] = new Vector2(-0.2f,  0.55f) * preScale;
            }

            // Load bullet vertices.
            { 
                float preScale = 15.0f;
                Bullet[0] = new Vector2(-0.3f,  0.0f) * preScale;
                Bullet[1] = new Vector2(-0.1f,  0.2f) * preScale;
                Bullet[2] = new Vector2( 0.8f,  0.0f) * preScale;
                Bullet[3] = new Vector2(-0.1f, -0.2f) * preScale;
            }

            // Load grunt vertices.
            {
                float preScale = 18.0f;
                Grunt[0] = new Vector2(-1.0f, 0.0f) * preScale;
                Grunt[1] = new Vector2(-0.0f,-1.0f) * preScale;
                Grunt[2] = new Vector2( 1.0f, 0.0f) * preScale;
                Grunt[3] = new Vector2(-0.0f, 1.0f) * preScale;
            }
        }
    }
}
