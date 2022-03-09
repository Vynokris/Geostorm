using System;
using System.Numerics;
using static System.Math;
using System.Diagnostics;
using static MyMathLib.Geometry2D;
using static MyMathLib.Arithmetic;

namespace MyMathLib
{
    // ---------- Geometry3D ---------- //

    public struct Vertex
    {
        Vector3 pos;
        Vector3 normal;
        Color   color;
        Vector2 uv;
    }

    public static class Geometry3D
    {
        // Returns the coordinates of a point on a sphere of radius r, using the given angles.
        public static Vector3 GetSphericalCoords(float r, float theta, float phi)
        {
            return new Vector3(r * (float)Sin(theta) * (float)Cos(phi),
                               r * (float)Cos(theta),
                               r * (float)Sin(theta) * (float)Sin(phi));
        }


        // ---- Vector3 ---- //

        // Null vector.
        public static Vector3 Vector3Zero() { return new Vector3(0, 0, 0); }

        // Vector from 3 coordinates.
        public static Vector3 Vector3Create(float x, float y, float z) { return new Vector3(x, y, z); }

        // Vector from points.
        public static Vector3 Vector3FromPoints(Vector3 p0, Vector3 p1) { return new Vector3(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z); }

        // Vector from angle.
        public static Vector3 Vector3FromAngle(float theta, float phi, float length)
        {
            return new Vector3(length * (float)Sin(theta) * (float)Cos(phi),
                               length * (float)Cos(theta),
                               length * (float)Sin(theta) * (float)Sin(phi));
        }

        // Vector from segment.
        // TODO.

        // Vector dot product.
        public static float Dot(this Vector3 v0, Vector3 v1) { return (v0.X * v1.X) + (v0.Y * v1.Z) + (v0.Z * v1.Z); }

        // Vector cross product.
        public static Vector3 Cross(this Vector3 v0, Vector3 v1) { return new Vector3((v0.Y * v1.Z - v0.Z * v1.Y), (v0.Z * v1.X - v0.X * v1.Z), (v0.X * v1.Y - v0.Y * v1.X)); }

        // Normalizes the vector so that its length is 1.
        public static void Normalize(this ref Vector3 v) { v /= v.Length(); }

        // Returns a normalized copy of the vector.
        public static Vector3 GetNormalized(this Vector3 v) { return v / v.Length(); }

        // Sets the length of the vector to the given value.
        public static void SetLength(this ref Vector3 v, float length) { v = v.GetNormalized() * length; }

        // Sets the destination's sign to those of the source.
        public static void CopySign(this ref Vector3 dest, Vector3 source) { dest = dest.GetCopiedSign(source); }

        // Returns a new vector width the destination's size and the source's signs.
        public static Vector3 GetCopiedSign(this Vector3 dest, Vector3 source)
        {
            return new Vector3((float)Math.CopySign(dest.X, source.X),
                               (float)Math.CopySign(dest.Y, source.Y),
                               (float)Math.CopySign(dest.Z, source.Z));
        }

        // Interprets the vector as a point and returns the distance to another point.
        public static float GetDistanceFromPoint(this Vector3 p0, Vector3 p1) { return Vector3FromPoints(p0, p1).Length(); }

        // Returns the angle (in radians) of the given vector.
        public static float GetAngleTheta(this Vector3 v)
        {
            return (2*(float)PI - Vector2Create(v.X, v.Z).GetAngle() + (float)PI/2) % 2*(float)PI;
        }
        public static float GetAnglePhi  (this Vector3 v) 
        {
            return (2 * (float)PI - Vector2Create(Vector2Create(v.X, v.Z).Length(), v.Y).GetAngle()) - 2 * (float)PI;
        }

        // Returns the angle (in radians) between two vectors.
        public static float GetAngleThetaWithVec3(this Vector3 v0, Vector3 v1)
        {
            float v0angle = v0.GetAngleTheta();
            float v1angle = v1.GetAngleTheta();
            return (v0angle >= v1angle ? (v0angle - v1angle) : (v1angle - v0angle));
        }
        public static float GetAnglePhiWithVec3(this Vector3 v0, Vector3 v1)
        {
            float v0angle = v0.GetAnglePhi();
            float v1angle = v1.GetAnglePhi();
            return (v0angle >= v1angle? (v0angle - v1angle) : (v1angle - v0angle));
        }

        // Rotates the given vector by the given angle.
        public static void Rotate(this ref Vector3 v, float theta, float phi) 
        {
            v = Vector3FromAngle(v.GetAngleTheta() + theta, v.GetAnglePhi() + phi, v.Length());
        }

        // Creates a Vector4 from this vector.
        public static Vector4 ToVec4(this Vector3 v) { return Vector4Create(v.X, v.Y, v.Z, 1); }

        // Calculates linear interpolation for a value from a start point to an end point.
        public static Vector3 Point3Lerp(float val, Vector3 start, Vector3 end)
        {
        return new Vector3(Lerp(val, start.X, end.X),
                           Lerp(val, start.Y, end.Y),
                           Lerp(val, start.Z, end.Z));
        }


        // ---- Vector4 ---- //

        // Null vector.
        public static Vector4 Vecto4Zero() { return new Vector4(0, 0, 0, 1); }

        // Vector from coordinates.
        public static Vector4 Vector4Create(float x, float y, float z, float w) { return new Vector4(x, y, z, w); }

        // Vector from 2 points.
        public static Vector4 Vector4FromPoints(Vector4 p0, Vector4 p1, float w) { return new Vector4(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z, w); }

        // Vector4 from vector3.
        public static Vector4 Vector4FromVec3(Vector3 v, float w) { return new Vector4(v.X, v.Y, v.Z, w); }

        // Vector from segment.
        // TODO.

        // Vector from angle.
        public static Vector4 Vector4FromAngle(float theta, float phi, float length, float w)
        {
            return new Vector4(length * (float)Sin(theta) * (float)Cos(phi),
                               length * (float)Cos(theta),
                               length * (float)Sin(theta) * (float)Sin(phi),
                               w);
        }

        // Vector dot product.
        public static float Dot(this Vector4 v0, Vector4 v1) { return v0.ToVec3(false).Dot(v1.ToVec3(false)); }

        // Vector cross product.
        public static Vector3 Cross(this Vector4 v0, Vector4 v1) { return v0.ToVec3(false).Cross(v1.ToVec3(false)); }

        // Homogenizes the vector4 by dividing it by w.
        public static Vector4 Homogenize(this Vector4 v)
        {
            return new Vector4(v.X / v.W, v.Y / v.W, v.Z / v.W, 1f);
        }
        
        // Normalizes the given vector so that its length is 1.
        public static Vector4 Normalize(this Vector4 v) 
        {
            return new Vector4(v.X / v.Length(), v.Y / v.Length(),
                               v.Z / v.Length(), v.W / v.Length());
        }

        // Negates both of the coordinates of the given vector.
        public static Vector4 Negate(this Vector4 v) { return new Vector4(-v.X, -v.Y, -v.Z, v.W); }

        // Copies the signs from the source vector to the destination vector.
        public static Vector4 Copysign(this Vector4 dest, Vector4 src) 
        {
            return new Vector4((float)Math.CopySign(dest.X, src.X), (float)Math.CopySign(dest.Y, src.Y),
                               (float)Math.CopySign(dest.Z, src.Z), (float)Math.CopySign(dest.W, src.W));
        }

        // Interprets the vector as a point and returns the distance to another point.
        public static float GetDistanceFromPoint(this Vector4 p0, Vector4 p1)
        {
            return new Vector4(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z, p0.W).Length();
        }

        // Returns the angle (in radians) of the given vector.
        public static float GetAngleTheta(this Vector4 v) { return (float)Acos(v.Z / v.Length()); }

        public static float GetAnglePhi  (this Vector4 v)
        { 
            if (v.X < 0) return (float)Atan(v.Y / v.X);
            if (v.X > 0) return (float)Atan(v.Y / v.X) + (float)PI;
            return (float)PI / 2;
        }

        // Returns the angle (in radians) between two vectors.
        public static float GetAngleThetaWithVec4(this Vector4 v1, Vector4 v2)
        {
            float v1Angle = v1.GetAngleTheta();
            float v2Angle = v2.GetAngleTheta();
            return (v1Angle >= v2Angle ? (v1Angle - v2Angle) : (v2Angle - v1Angle));
        }

        public static float GetAnglePhiWithVec4(this Vector4 v1, Vector4 v2)
        {
            float v1Angle = v1.GetAnglePhi();
            float v2Angle = v2.GetAnglePhi();
            return (v1Angle >= v2Angle ? (v1Angle - v2Angle) : (v2Angle - v1Angle));
        }

        // Rotates the given vector by the given angle.
        public static void Rotate(this ref Vector4 v, float theta, float phi)
        {
            v = Vector4FromAngle(theta,  phi, v.Length(), v.W);
        }

        // Creates a Vector3 from this vector.
        public static Vector3 ToVec3(this Vector4 v, bool homogenizeVec)
        {
            if (homogenizeVec)
                return Vector3Create(v.X/v.W, v.Y/v.W, v.Z/v.W);
            else
                return Vector3Create(v.X, v.Y, v.Z);
        }


        // ---- Vertex ---- //
        public class Vertex
        {
            public Vector3 Pos, Normal;
            public Color   Col;
            public Vector2 UV;

            // Null vertex.
            public Vertex() { Pos = Normal = Vector3Zero(); Col = new Color(); UV = Vector2Zero(); }

            // Vertex from position, normal, color and UV.
            public Vertex(Vector3 pos, Vector3 normal, Color color, Vector2 uv) { Pos = pos; Normal = normal; Col = color; UV = uv; }
        }


        // ---- Segment3 ---- //
        public class Segment3
        {
            public Vertex A, B;

            // Null Segment3.
            public Segment3() { A = B = new Vertex(); }

            // Segement3 from points.
            public Segment3(Vertex a, Vertex b) { A = a;  B = b; }

            // Segment3 from point and vector.
            public Segment3(Vertex O, Vector3 v) { A = O; B = new Vertex(O.Pos + v, O.Normal, O.Col, O.UV); }

            // Returns the center of mass of the Segment3.
            Vertex GetCenterOfMass()
            {
                return new Vertex( 
                    new Vector3( (A.Pos.X + B.Pos.X) / 2, (A.Pos.Y + B.Pos.Y) / 2, (A.Pos.Z + B.Pos.Z) / 2 ), 
                    Point3Lerp(0.5f, A.Normal, B.Normal),
                    ColorLerp(0.5f, A.Col, B.Col),
                    Point2Lerp(0.5f, A.UV, B.UV)
                );
            }

            // Moves the Segment3 by the given vector.
            public void Move(Vector3 v) { A.Pos += v; B.Pos += v; }

        }


        // ---- Triangle3 ---- //
        public class Triangle3
        {
            public Vertex A, B, C;

            // Null triangle.
            public Triangle3() { A = B = C = new Vertex(); }

            // Triangle from 3 points.
            public Triangle3(Vertex a, Vertex b, Vertex c) { A = a; B = b; C = c; }

            // Returns the center of mass of the triangle.
            public Vertex GetCenterOfMass() 
            { 
                return new Vertex(
                    new Vector3 ( (A.Pos.X    + B.Pos.X    + C.Pos.X)    / 3, (A.Pos.Y    + B.Pos.Y    + C.Pos.Y)    / 3, (A.Pos.Z    + B.Pos.Z    + C.Pos.Z)    / 3 ),
                    new Vector3 ( (A.Normal.X + B.Normal.X + C.Normal.X) / 3, (A.Normal.Y + B.Normal.Y + C.Normal.Y) / 3, (A.Normal.Z + B.Normal.Z + C.Normal.Z) / 3 ),
                    new Color   ( (A.Col.R    + B.Col.R    + C.Col.R)    / 3, (A.Col.G    + B.Col.G    + C.Col.G)    / 3, (A.Col.B    + B.Col.B    + C.Col.B)    / 3, (A.Col.A + B.Col.A + C.Col.A) / 3 ),
                    new Vector2 ( (A.UV.X     + B.UV.X     + C.UV.X)     / 3, (A.UV.Y     + B.UV.Y     + C.UV.Y)     / 3 )
                ); 
            }

            // Returns the side of the triangle that corresponds to the given index.
            public Segment3 GetSide(int index)
            {
                Debug.Assert (0 <= index && index < 3);

                return index switch
                {
                    0 => new Segment3(A, B),
                    1 => new Segment3(B, C),
                    2 => new Segment3(C, A),
                    _ => new Segment3(),
                };
            }

            // Returns the vertex of the triangle that corresponds to the given index.
            public Vertex GetVertex(int index)
            {
                Debug.Assert (0 <= index && index < 3);

                return index switch
                {
                    0 => A,
                    1 => B,
                    2 => C,
                    _ => new Vertex(),
                };
            }

            // Moves the triangle by the given vector.
            public void Move(Vector3 v) { A.Pos += v; B.Pos += v; C.Pos += v; }
        }
    }
}
